namespace MVVM.Model
{
    public interface ICredential
    {
        string Username { get; }
        string Password { get; }
        string[] Description { get; }
    }
}