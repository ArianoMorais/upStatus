namespace UpStatus.Application.Common.Abstractions;

public interface IPasswordHasher
{
    string Hash(string plainPassword);

    bool Verify(string plainPassword, string hash);
}
