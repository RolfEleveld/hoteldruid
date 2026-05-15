using Xunit;

namespace HotelDruid.Api.Tests;

/// <summary>
/// Collection definition to disable parallel test execution.
/// This ensures tests run sequentially to avoid shared state issues.
/// </summary>
[CollectionDefinition("Sequential", DisableParallelization = true)]
public class SequentialCollection
{
}

