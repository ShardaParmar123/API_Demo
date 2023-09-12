using AutoMapper;
using Demo.BLL.Services.Shared;
using Demo.Contract.Repositories.Auth;
using Demo.Contract.Services.Auth;
using Demo.Contract.Services.Shared;
using Demo.Types.Dtos.Auth;
using Demo.Types.Dtos.Shared;
using Demo.Types.Entities.Auth;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace Demo.BLL.Services.Auth
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;
        private readonly IMapper _mapper;
        private readonly ILoggerService _logger;
     
        public UserService(IUserRepository userRepo, IMapper mapper, ILoggerService logger) 
        {
            _userRepo = userRepo;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ServiceResponse<Resource>> Create(UserDto userDto)
        {
            var sr = new ServiceResponse<Resource>();
            userDto.Id = 0;
            try
            {
                (bool isValid, userDto) = await IsValid(userDto);
                if (isValid)
                {
                    User user = _mapper.Map<User>(userDto);
                    user.Password = Password.Encrypt(userDto.Password);
                    if (user != null)
                    {
                        _userRepo.Create(user);
                        await _userRepo.SaveChangesAsync();
                    }
                    sr.Success = isValid;
                    sr.Code = (int)StatusCode.OK;
                    sr.Message = "User inserted successfully.";
                    _logger.LogInfo("User inserted successfully.");
                }
                else
                {
                    sr.Success = isValid;
                    sr.Code = (int)StatusCode.ExpectationFailed;
                    sr.Message = $"UserName {userDto.UserName} already exists.";
                    _logger.LogInfo(sr.Message);
                }              
            }
            catch (AutoMapperMappingException ex)
            {
                sr.Success = false;
                sr.Code = (int)StatusCode.ExpectationFailed;
                sr.Message = ex.Message;
                _logger.LogError(ex.Message);
            }
            catch(DbException ex)
            {
                sr.Success = false;
                sr.Code = (int)StatusCode.ExpectationFailed;
                sr.Message =  ex.Message;
                _logger.LogError(ex.Message);
            }
            catch (Exception ex)
            {
                sr.Success = false;
                sr.Code = (int)StatusCode.InternalServerError;
                sr.Message = $"{ex.Message}, User not inserted, something went wrong.";
                _logger.LogError(sr.Message);
            }
            return sr;
        }      

        public async Task<ServiceResponse<UserDto>> Get(int id)
        {
            var sr = new ServiceResponse<UserDto>();
            var user = await _userRepo.FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
            try
            {
                UserDto User = _mapper.Map<UserDto>(user);
                if (user != null)
                {
                    sr.Data = User;
                    sr.Success = true;
                    sr.Code = (int)StatusCode.Found;
                    sr.Message = "Record found.";
                    _logger.LogInfo(sr.Message);
                }
                else
                {                   
                    sr.Success = false;
                    sr.Code = (int)StatusCode.NotFound;
                    sr.Message = "No record found.";
                    _logger.LogError(sr.Message);
                }
            }
            catch (AutoMapperMappingException ex)
            {
                
                sr.Success = false;
                sr.Code = (int)StatusCode.ExpectationFailed;
                sr.Message = ex.Message;
                _logger.LogError(sr.Message);
            }
            catch (DbException ex)
            {
                sr.Success = false;
                sr.Code = (int)StatusCode.ExpectationFailed;
                sr.Message = ex.Message;
                _logger.LogError(sr.Message);
            }
            catch (Exception ex)
            {
                sr.Success = false;
                sr.Code = (int)StatusCode.InternalServerError;
                sr.Message = $"{ex.Message}, User not inserted, something went wrong.";
                _logger.LogError(sr.Message);
            }
            return sr;
        }

        //public async Task<ServiceResponse<List<UserList>>> List()
        //{
        //    var sr = new ServiceResponse<List<UserList>>();
        //    var list = await _userRepo.FindAll().ToListAsync();
        //    try
        //    {
        //        List<UserList> UserList = _mapper.Map<List<UserList>>(list);
        //        if (UserList != null)
        //        {
        //            sr.Success = true;
        //            sr.Code = (int)StatusCode.Found;
        //            sr.Message = $"{UserList.Count()} Record found.";
        //            sr.Data = UserList;
        //        }
        //        else
        //        {
        //            
        //            sr.Success = true;
        //            sr.Code = (int)StatusCode.NotFound;
        //            sr.Message = "No record found.";
        //            _logger.LogError(sr.Message);
        //        }
        //    }
        //    catch (AutoMapperMappingException ex)
        //    {
        //        
        //        sr.Success = false;
        //        sr.Code = (int)StatusCode.ExpectationFailed;
        //        sr.Message = ex.Message;
        //        _logger.LogError(sr.Message);
        //    }
        //    catch (DbException ex)
        //    {
        //        
        //        sr.Success = true;
        //        sr.Code = (int)StatusCode.ExpectationFailed;
        //        sr.Message = ex.Message;
        //        _logger.LogError(sr.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        
        //        sr.Success = true;
        //        sr.Code = (int)StatusCode.InternalServerError;
        //        sr.Message = $"{ex.Message}, User not inserted, something went wrong.";
        //        _logger.LogError(sr.Message);
        //    }
        //    return sr;
        //}

        public async Task<ServiceResponse<Resource>> Update(UserDto userDto)
        {
            var sr = new ServiceResponse<Resource>();
            try
            {
                (bool isValid, userDto) = await IsValid(userDto);
                if (isValid)
                {
                    var getUser = _userRepo.FindByCondition(x => x.Id == userDto.Id).FirstOrDefault();
                    User user = _mapper.Map<User>(userDto);
                    if (user != null)
                    {
                        _userRepo.Update(user);
                        await _userRepo.SaveChangesAsync();
                    }
                    sr.Success = isValid;
                    sr.Code = (int)StatusCode.OK;
                    sr.Message = "User updated successfully.";
                }
                else
                {
                    sr.Success = isValid;
                    sr.Code = (int)StatusCode.NotFound;
                    sr.Message = "User not found.";
                }              
            }
            catch (AutoMapperMappingException ex)
            {
                sr.Success = false;
                sr.Code = (int)StatusCode.ExpectationFailed;
                sr.Message = ex.Message;
                _logger.LogError(sr.Message);
            }
            catch (DbException ex)
            {
                sr.Success = false;
                sr.Code = (int)StatusCode.ExpectationFailed;
                sr.Message = ex.Message;
                _logger.LogError(sr.Message);
            }
            catch (Exception ex)
            {
                sr.Success = false;
                sr.Code = (int)StatusCode.InternalServerError;
                sr.Message = $"{ex.Message}, User not updated, something went wrong.";
                _logger.LogError(sr.Message);
            }
            return sr; ;
        }

        public async Task<ServiceResponse<Resource>> Delete(int id)
        {
            var sr = new ServiceResponse<Resource>();
            var user = _userRepo.FindByCondition(x => x.Id == id).FirstOrDefault();
            try
            {
                UserDto User = _mapper.Map<UserDto>(user);
                if (user != null)
                {
                    _userRepo.UpdateUser(user);
                    await _userRepo.SaveChangesAsync();

                    sr.Success = true;
                    sr.Code = (int)StatusCode.OK;
                    sr.Message = "User deleted successfully.";
                }
                else
                {                   
                    sr.Success = false;
                    sr.Code = (int)StatusCode.NotFound;
                    sr.Message = $"{id} User not found.";
                    _logger.LogError(sr.Message);
                }
            }
            catch (AutoMapperMappingException ex)
            {
                sr.Success = false;
                sr.Code = (int)StatusCode.ExpectationFailed;
                sr.Message = ex.Message;
                _logger.LogError(sr.Message);
            }
            catch (DbException ex)
            {
                sr.Success = false;
                sr.Code = (int)StatusCode.ExpectationFailed;
                sr.Message = ex.Message;
                _logger.LogError(sr.Message);
            }
            catch (Exception ex)
            {
                sr.Success = false;
                sr.Code = (int)StatusCode.InternalServerError;
                sr.Message = $"{ex.Message}, User not deleted, something went wrong.";
                _logger.LogError(sr.Message);
            }
            return sr;
        }     

        public async Task<ServiceResponse<List<UserList>>> List(QuerySpec querySpec)
        {
            var sr = new ServiceResponse<List<UserList>>();            
            try
            {
                var list = await _userRepo.FindAllQuerySpec(querySpec).Where(a => a.IsActive == true).ToListAsync();
                List<UserList> UserList = _mapper.Map<List<UserList>>(list);
                if (UserList != null)
                {
                    sr.Success = true;
                    sr.Code = (int)StatusCode.Found;
                    sr.Message = $"{UserList.Count()} Record found.";
                    sr.Data = UserList;
                }
            }
            catch (AutoMapperMappingException ex)
            {
                sr.Success = false;
                sr.Code = (int)StatusCode.ExpectationFailed;
                sr.Message = ex.Message;
                _logger.LogError(ex.Message);
            }
            catch (DbException ex)
            {
                sr.Success = false;
                sr.Code = (int)StatusCode.ExpectationFailed;
                sr.Message = ex.Message;
                _logger.LogError(ex.Message);
            }
            catch (Exception ex)
            {
                sr.Success = false;
                sr.Code = (int)StatusCode.InternalServerError;
                sr.Message = $"{ex.Message}, User not inserted, something went wrong.";
                _logger.LogError(sr.Message);
            }             
            return sr;
        }

        private async Task<(bool isValid, UserDto userDto)> IsValid(UserDto userDto)
        {
            bool isValid = true;
            Alert alert = new();           
            try
            {
                // 1. User Exists
                if (await _userRepo.Exists(a => a.UserName == userDto.UserName))
                {
                    alert.ErrorMessage = $"UserName {userDto.UserName} already exists";
                    isValid = false;
                }

                if (!await _userRepo.Exists(a => a.Id == userDto.Id))
                {
                    alert.ErrorMessage = $"User not found.";
                    isValid = false;
                }
            }
            catch (Exception ex)
            {
                return (isValid, userDto);
            }
            return (isValid, userDto);           
        }       
    }
}
