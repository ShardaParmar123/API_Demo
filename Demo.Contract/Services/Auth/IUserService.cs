using Demo.Types.Dtos.Auth;
using Demo.Types.Dtos.Shared;

namespace Demo.Contract.Services.Auth
{
    public interface IUserService
    {
        //Task<ServiceResponse<List<UserList>>> List();
        Task<ServiceResponse<List<UserList>>> List(QuerySpec pagination);
        Task<ServiceResponse<Resource>> Create(UserDto userDto);
        Task<ServiceResponse<UserDto>> Get(int id);
        Task<ServiceResponse<Resource>> Update(UserDto userDto);
        Task<ServiceResponse<Resource>> Delete(int id);
    }
}
