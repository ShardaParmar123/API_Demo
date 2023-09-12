using Demo.Types.Dtos.Auth;
using Demo.Types.Dtos.Shared;

namespace Demo.Contract.Services.Auth
{
    public interface IAuthService
    {
        Task<ServiceResponse<LoginResponseDto>> Login(LoginDto loginDto);
    }
}
