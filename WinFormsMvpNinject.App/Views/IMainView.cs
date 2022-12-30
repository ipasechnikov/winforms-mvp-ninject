using WinFormsMvpNinject.App.Models;

namespace WinFormsMvpNinject.App.Views
{
    public interface IMainView
    {
        IUser[] Users { get; set; }
    }
}
