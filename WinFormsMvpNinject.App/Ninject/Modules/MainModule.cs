using Ninject.Extensions.Factory;
using Ninject.Modules;

using WinFormsMvpNinject.App.Factories;
using WinFormsMvpNinject.App.Presenters;
using WinFormsMvpNinject.App.Services;
using WinFormsMvpNinject.App.Views;

namespace WinFormsMvpNinject.App.Ninject.Modules
{
    public class MainModule : NinjectModule
    {
        public override void Load()
        {
            BindModels();
            BindViews();
            BindPresenters();
            BindServices();
            BindFactories();
        }

        private void BindModels()
        {
            // Models are not required to be bound, as they are never injected but created as concrete instances by services, respositories and so on
        }

        private void BindViews()
        {
            // It's not required to bind IMainView because we don't inject it anywhere
            // MainForm is injected at the program launch as a concrete class
            // More info here: https://github.com/ninject/Ninject/wiki/Dependency-Injection-With-Ninject#skipping-the-type-binding-bit--implicit-self-binding-of-concrete-types
            // This line can be commented out without any consequences
            Bind<IMainView>().To<MainForm>();

            // This one is required to be bound otherwise IViewFactory won't be able to create a class that is bound to ISomeRandomView interface
            Bind<ISomeRandomView>().To<SomeRandomForm>();
        }

        private void BindPresenters()
        {
            Bind<IMainPresenter>().To<DefaultMainPresenter>();
        }

        private void BindServices()
        {
            Bind<IUserService>().To<DefaultUserService>();
        }

        private void BindFactories()
        {
            // A neat Ninject extension that in our case allows us to create injected forms/control dynamically
            // https://github.com/ninject/Ninject.Extensions.Factory/wiki
            Bind<IViewFactory>().ToFactory();
        }
    }
}
