using PRN232ASM.AuthService.Domain.Entities;
using Xunit;

namespace AuthService.Tests;

public class RoleTests
{
    [Fact]
    public void Role_constants_match_expected_names()
    {
        Assert.Equal("Admin", Role.Admin);
        Assert.Equal("Researcher", Role.Researcher);
        Assert.Equal("Student", Role.Student);
    }
}
