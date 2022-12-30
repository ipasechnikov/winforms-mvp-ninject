using WinFormsMvpNinject.App.Views;

namespace WinFormsMvpNinject.App.Presenters
{
    public interface IMainPresenter
    {
        IMainView? View
        {
            get; set;
        }

        // Might be a long-running (a few seconds) task to get data from remote data source
        // We want to be it an asynchronous task so that UI thread is kept running while we get data in the background
        Task GetUsers();
    }
}
