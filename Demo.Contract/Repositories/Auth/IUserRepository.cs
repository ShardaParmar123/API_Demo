using Demo.Contract.Repositories.Base;
using Demo.Types.Entities.Auth;

namespace Demo.Contract.Repositories.Auth
{
    public interface IUserRepository : IRepositoryBase<User>
    {
        void UpdateUser(User user);
    }
}
