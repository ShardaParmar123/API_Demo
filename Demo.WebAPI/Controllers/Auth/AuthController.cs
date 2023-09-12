using Demo.Contract.Services.Auth;
using Demo.Contract.Services.Shared;
using Demo.Types.Dtos.Auth;
using Demo.Types.Dtos.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data.Common;

namespace Demo.WebAPI.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILoggerService _logger;
        public AuthController(IAuthService authService, ILoggerService logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public async Task<ServiceResponse<LoginResponseDto>> Login(LoginDto loginDto)
        {
            var sr = new ServiceResponse<LoginResponseDto>();
            var user = _authService.Login(loginDto);
            try
            {
                if (user != null)
                {
                    sr.Code = sr.Code;
                    sr.Success = true;
                    sr.Data = user.Result.Data;
                    sr.Data.Token = user.Result.Data.Token;
                    sr.Message = "User login successfully.";
                    _logger.LogInfo(sr.Message);
                }
                else
                {
                    sr.Code = sr.Code;
                    sr.Message = "User not found.";
                    _logger.LogError(sr.Message);
                }
            }
            catch (DbException ex)
            {
                sr.Code = sr.Code;
                sr.Message = ex.Message;
                _logger.LogError(sr.Message);
            }
            catch (Exception ex)
            {
                sr.Code = sr.Code;
                sr.Message = $"{ex.Message}, User not inserted, something went wrong.";
                _logger.LogError(sr.Message);
            }
            return sr;
        }
    }
}
