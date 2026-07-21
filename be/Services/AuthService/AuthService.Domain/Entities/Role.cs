namespace PRN232ASM.AuthService.Domain.Entities;

public class Role
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<User> Users { get; set; } = new List<User>();

    public const string Admin = "Admin";
    public const string Researcher = "Researcher";
    public const string Student = "Student";
}
