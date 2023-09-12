using AutoMapper;
using Demo.BLL.Services.Base;
using Demo.Contract.Repositories.Auth;
using Demo.Infrastructure.Configuration;
using Demo.Types.Entities.Auth;

namespace Demo.DAL.Repositories.Auth
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        private readonly DemoDbContext _dbContext;
        private readonly IMapper _mapper;
        public UserRepository(DemoDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public void UpdateUser(User user)
        {
            user.IsActive = false;
            user.IsDeleted = true;
            _dbContext.Update(user);
        }     
    }    
}
