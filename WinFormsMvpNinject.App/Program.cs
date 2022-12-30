using Ninject;
using Ninject.Activation.Strategies;

using WinFormsMvpNinject.App.Ninject.Modules;
using WinFormsMvpNinject.App.Ninject.Strategies;
using WinFormsMvpNinject.App.Views;

namespace WinFormsMvpNinject.App
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            // Create a Ninject kernel
            var kernel = new StandardKernel(new MainModule());

            // This one line makes DI magically work with WinForms
            // It allows us to recursively inject controls inside forms or other controls
            kernel.Components.Add<IActivationStrategy, WindowsFormsStrategy>();

            // Get MainForm with injected dependencies and run application
            var mainForm = kernel.Get<MainForm>();
            Application.Run(mainForm);
        }
    }
}