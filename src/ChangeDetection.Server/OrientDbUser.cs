using Microsoft.AspNetCore.Identity;

public class OrientDbUser
{
    public string StateDatabaseName { get; set; }
}

public class PasswordValidator : IPasswordValidator<OrientDbUser>
{
    public Task<IdentityResult> ValidateAsync(UserManager<OrientDbUser> manager, OrientDbUser user, string password)
    {
        throw new NotImplementedException();
    }
}