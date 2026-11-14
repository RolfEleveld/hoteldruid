using System.Security.Claims;
using HotelDruid.Api.Services;
using HotelDruid.Shared;
using Microsoft.AspNetCore.Http;

namespace HotelDruid.Api.Tests.Services;

public class RequestActorTests
{
    [Fact]
    public void SignedInName_WinsOverTheDeskHeader()
    {
        var context = new DefaultHttpContext();
        context.User = new ClaimsPrincipal(new ClaimsIdentity(
            [new Claim("preferred_username", "maria.desk")],
            authenticationType: "Bearer"));
        context.Request.Headers[DeskUserNames.HeaderName] = "Someone Else";

        Assert.Equal("maria.desk", RequestActor.KnownName(context));
        Assert.True(RequestActor.IsAuthenticated(context));
    }

    [Fact]
    public void DeskHeader_IsTheNameWhenNobodyIsSignedIn()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers[DeskUserNames.HeaderName] = " Maria Rossi ";

        Assert.Equal("Maria Rossi", RequestActor.KnownName(context));
        Assert.False(RequestActor.IsAuthenticated(context));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("<script>")]
    [InlineData("name\r\nSet-Cookie")]
    public void DeskHeader_RejectsANameThatIsNotALabel(string raw)
    {
        var context = new DefaultHttpContext();
        context.Request.Headers[DeskUserNames.HeaderName] = raw;

        Assert.Null(RequestActor.KnownName(context));
    }

    [Fact]
    public void UnknownCaller_StaysUnnamed()
    {
        Assert.Null(RequestActor.KnownName(new DefaultHttpContext()));
    }
}
