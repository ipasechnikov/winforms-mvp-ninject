using WinFormsMvpNinject.App.Models;

namespace WinFormsMvpNinject.App.Services
{
    public class DefaultUserService : IUserService
    {
        public Task<IUser[]> GetUsers()
        {
            return Task.FromResult(new IUser[]
            {
                new DefaultUser { Name = "Name1", Age = 10 },
                new DefaultUser { Name = "Name2", Age = 20 },
                new DefaultUser { Name = "Name3", Age = 30 },
                new DefaultUser { Name = "Name4", Age = 40 },
                new DefaultUser { Name = "Name5", Age = 50 }
            });
        }
    }
}
