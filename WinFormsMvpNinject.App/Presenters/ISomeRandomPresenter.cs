using WinFormsMvpNinject.App.Views;

namespace WinFormsMvpNinject.App.Presenters
{
    public interface ISomeRandomPresenter
    {
        ISomeRandomView? View { get; set; }
    }
}
