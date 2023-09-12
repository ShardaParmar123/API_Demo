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
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILoggerService _logger;
        public UserController(IUserService userService, ILoggerService logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ServiceResponse<Resource>> Create(UserDto userDto)
        {
            var sr = new ServiceResponse<Resource>();
            try
            {
                sr = await _userService.Create(userDto);
                if(sr.Success == true)
                {
                    sr.Success = true;
                    sr.Code = sr.Code;
                    sr.Message = "User inserted successfully.";
                    _logger.LogInfo(sr.Message);
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

        [HttpGet("{id}")]
        public async Task<ServiceResponse<UserDto>> Get(int id)
        {
            var sr = new ServiceResponse<UserDto>();
            try
            {
                sr = await _userService.Get(id);
                if (sr.Data != null)
                {
                    sr.Success = true;
                    sr.Code = sr.Code;
                    sr.Message = "Record found";
                    _logger.LogInfo(sr.Message);
                }
            }
            catch (DbException ex)
            {
                sr.Code = sr.Code;
                sr.Message = ex.Message;
            }
            catch (Exception ex)
            {
                sr.Code = sr.Code;
                sr.Message = $"{ex.Message}, No record found.";
                _logger.LogError(sr.Message);
            }
            return sr;

        }

        //[HttpGet("list")]
        //public async Task<ServiceResponse<List<UserList>>> List()
        //{
        //    var sr = new ServiceResponse<List<UserList>>();
        //    try
        //    {
        //        sr = await _userService.List();
        //        if (sr.Data != null)
        //        {
        //            sr.Success = true;
        //            sr.Code = sr.Code;
        //            sr.Message = $"{sr.Data.Count()} Record found.";
        //            sr.Data = sr.Data.ToList();
        //        }
        //    }
        //    catch (DbException ex)
        //    {
        //        sr.Code = sr.Code;
        //        sr.Message = ex.Message;
        //        _logger.LogError(sr.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        sr.Code = sr.Code;
        //        sr.Message = $"{ex.Message}, No record found.";
        //        _logger.LogError(sr.Message);
        //    }
        //    return sr;

        //}

        [HttpPut]
        public async Task<ServiceResponse<Resource>> Update(UserDto userDto)
        {
            var sr = new ServiceResponse<Resource>();
            try
            {
                sr = await _userService.Update(userDto);
                if (sr.Success == true)
                {
                    sr.Success = true;
                    sr.Code = sr.Code;
                    sr.Message = "User updated successfully.";
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
                sr.Message = $"{ex.Message}, User not updated, something went wrong.";
                _logger.LogError(sr.Message);
            }
            return sr;

        }

        [HttpDelete("{id}")]
        public async Task<ServiceResponse<Resource>> Delete(int id)
        {
            var sr = new ServiceResponse<Resource>();
            try
            {
                sr = await _userService.Delete(id);
                if (sr.Success == true)
                {
                    sr.Success = true;
                    sr.Code = sr.Code;
                    sr.Message = "User deleted successfully.";
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
                sr.Message = $"{ex.Message}, User not deleted, something went wrong.";
                _logger.LogError(sr.Message);
            }
            return sr;
        }

        [HttpPost("list")]
        public async Task<ServiceResponse<List<UserList>>> List(QuerySpec querySpec)
        {
            var sr = new ServiceResponse<List<UserList>>();
            try
            {
                sr = await _userService.List(querySpec);
                if (sr.Data != null)
                {
                    sr.Success = true;
                    sr.Code = sr.Code;
                    sr.Message = $"{sr.Data.Count()} Record found.";
                    sr.Data = sr.Data;
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
                sr.Message = $"{ex.Message}, No record found.";
                _logger.LogError(sr.Message);
            }
            return sr;
        }

    }
}
