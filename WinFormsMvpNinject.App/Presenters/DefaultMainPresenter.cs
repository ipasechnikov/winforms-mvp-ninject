using WinFormsMvpNinject.App.Services;
using WinFormsMvpNinject.App.Views;

namespace WinFormsMvpNinject.App.Presenters
{
    public class DefaultMainPresenter : IMainPresenter
    {
        private readonly IUserService userService;

        public DefaultMainPresenter(IUserService userService)
        {
            this.userService = userService;
        }

        public IMainView? View
        {
            get; set;
        }

        public async Task GetUsers()
        {
            View!.Users = await userService.GetUsers();
        }
    }
}
