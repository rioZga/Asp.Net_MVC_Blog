using BlogWebApi.Dtos;

namespace BlogWebApi.Interfaces
{
    public interface IAuthService
    {
        Task<Response> Login(LoginDto loginDto);
        Task<Response> Register(RegisterDto registerDto);
    }
}
