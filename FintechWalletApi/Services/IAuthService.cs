using FintechWalletApi.Models;

namespace FintechWalletApi.Services;

public interface IAuthService
{
     Task<string> Register(string email, string password);
     Task<string> Login(string email, string password);
}
