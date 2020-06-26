namespace Services.HashPassword
{
    public interface IHashPasswordService
    {
        string HashPassword(string password);
    }
}