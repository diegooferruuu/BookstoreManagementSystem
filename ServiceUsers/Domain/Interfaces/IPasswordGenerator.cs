namespace ServiceUsers.Domain.Interfaces
{
    public interface IPasswordGenerator
    {
        string GenerateSecurePassword();
    }
}
