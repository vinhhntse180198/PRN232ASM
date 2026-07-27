using PRN232ASM.BuildingBlocks.Common.Exceptions;
using Xunit;

namespace PaperService.Tests;

public class ConflictExceptionTests
{
    [Fact]
    public void ConflictException_stores_message()
    {
        var ex = new ConflictException("Paper with DOI '10.1/x' already exists.");
        Assert.Contains("DOI", ex.Message);
    }
}
