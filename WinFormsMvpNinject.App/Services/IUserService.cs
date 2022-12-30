using WinFormsMvpNinject.App.Models;

namespace WinFormsMvpNinject.App.Services
{
    public interface IUserService
    {
        // Might be a long-running task that gets users from API or database or whatever other data source you have
        Task<IUser[]> GetUsers();
    }
}
