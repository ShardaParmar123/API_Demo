using AutoMapper;
using Demo.Types.Dtos.Auth;
using Demo.Types.Entities.Auth;

namespace Demo.WebAPI.AutoMapper.Auth
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();

            CreateMap<User, LoginResponseDto>();

            CreateMap<User, UserList>();
        }
    }
}
