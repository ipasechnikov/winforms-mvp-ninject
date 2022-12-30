using WinFormsMvpNinject.App.Views;

namespace WinFormsMvpNinject.App.Factories
{
    // Allows to create us injected forms and control dynamically at runtime
    public interface IViewFactory
    {
        ISomeRandomView CreateSomeRandomView();
    }
}
