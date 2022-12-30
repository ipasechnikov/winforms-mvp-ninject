using WinFormsMvpNinject.App.Views;

namespace WinFormsMvpNinject.App.Presenters
{
    public class DefaultSomeRandomPresenter : ISomeRandomPresenter
    {
        public ISomeRandomView? View
        {
            get; set;
        }
    }
}
