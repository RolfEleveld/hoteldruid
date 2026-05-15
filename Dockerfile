# Stage 1: Build Blazor WASM client
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS client-build
WORKDIR /src
COPY src/HotelDroid.Shared/ src/HotelDroid.Shared/
COPY src/HotelDroid.Client/ src/HotelDroid.Client/
RUN dotnet publish src/HotelDroid.Client/HotelDroid.Client.csproj \
    -c Release -o /artifacts/client

# Stage 2: Build API
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS api-build
WORKDIR /src
COPY src/HotelDroid.Shared/ src/HotelDroid.Shared/
COPY src/HotelDroid.Api/ src/HotelDroid.Api/
RUN dotnet publish src/HotelDroid.Api/HotelDroid.Api.csproj \
    -c Release -o /artifacts/api

# Stage 3: Runtime — embed Blazor WASM into API wwwroot
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=api-build /artifacts/api ./
COPY --from=client-build /artifacts/client/wwwroot ./wwwroot/

# Persistent data directory
RUN mkdir -p /data

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENV HOTELDRUID_DATAROOT=/data

EXPOSE 8080
HEALTHCHECK --interval=30s --timeout=10s --start-period=20s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "HotelDroid.Api.dll"]
