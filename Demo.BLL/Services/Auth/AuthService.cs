using AutoMapper;
using Demo.Contract.Repositories.Auth;
using Demo.Contract.Services.Auth;
using Demo.Contract.Services.Shared;
using Demo.Types.Dtos.Auth;
using Demo.Types.Dtos.Shared;
using Demo.Types.Entities.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Data.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Demo.BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly IMapper _mapper;
        private readonly ILoggerService _logger;
        private readonly IConfiguration _config;

        public AuthService(IUserRepository userRepo, IMapper mapper, ILoggerService logger, IConfiguration config)
        {
            _userRepo = userRepo;
            _mapper = mapper;
            _logger = logger;
            _config = config;
        }

        public async Task<ServiceResponse<LoginResponseDto>> Login(LoginDto loginDto)
        {
            var sr = new ServiceResponse<LoginResponseDto>();
            var User = _userRepo.FindByCondition(a => a.Email == loginDto.Email).FirstOrDefault();
            try
            {              
                if (string.IsNullOrEmpty(loginDto.Email) || string.IsNullOrEmpty(loginDto.Password))
                {
                    sr.Code = 0;
                    sr.Message = "Please enter Email and Password.";
                }
                else
                {                  
                    LoginResponseDto user = _mapper.Map<LoginResponseDto>(User);
                    if (loginDto == null)
                    {
                        sr.Code = 404;
                        sr.Message = "User not found";
                        _logger.LogError(sr.Message);
                    }
                    else
                    {
                        sr.Code = 200;
                        sr.Success = true;
                        sr.Data = user;
                        sr.Data.Token = GenerateToken(loginDto);
                        sr.Message = "User login successfully.";
                        _logger.LogInfo(sr.Message);
                    }
                }
            }
            catch (DbException ex)
            {
                sr.Data = null;
                sr.Success = true;
                sr.Code = 0;
                sr.Message = ex.Message;
                _logger.LogError(ex.Message);
            }
            catch (Exception ex)
            {
                sr.Data = null;
                sr.Success = true;
                sr.Code = 0;
                sr.Message = $"{ex.Message}, User not inserted, something went wrong.";
                _logger.LogError(sr.Message);
            }
            return sr;
        }

        private string GenerateToken(LoginDto user)
        {
            // https://www.c-sharpcorner.com/article/jwt-token-creation-authentication-and-authorization-in-asp-net-core-6-0-with-po/
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,user.Email),
            };
            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials);


            return new JwtSecurityTokenHandler().WriteToken(token);

        }
        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
    }
}
