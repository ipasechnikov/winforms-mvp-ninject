using System.Diagnostics;

using Ninject.Activation;
using Ninject.Activation.Strategies;

namespace WinFormsMvpNinject.App.Ninject.Strategies
{
    /// <summary>
    /// Ninject ActivationStrategy to inject user controls recursively
    /// 
    /// Many thanks to this answer on StackOverflow
    /// https://stackoverflow.com/a/33928388
    /// </summary>
    public class WindowsFormsStrategy : ActivationStrategy
    {
        // Activate is called after Kernel.Inject even for objects not created by Ninject
        // To avoid multiple "injections" in the same nested controls we put this flag to false
        private bool _activatingControls = false;

        public override void Activate(IContext context, InstanceReference reference)
        {
            reference.IfInstanceIs<UserControl>(uc =>
            {
                if (!_activatingControls)
                {
                    Trace.TraceInformation("Activate. Injecting dependencies in User control of type {0}", uc.GetType());
                    _activatingControls = true;
                    context.Kernel.InjectDescendantOf(uc);
                    _activatingControls = false;
                }
            });

            reference.IfInstanceIs<Form>(form =>
            {
                if (!_activatingControls)
                {
                    Trace.TraceInformation("Activate. Injecting dependencies in Form of type {0}", form.GetType());
                    _activatingControls = true;
                    context.Kernel.InjectDescendantOf(form);
                    _activatingControls = false;
                }
            });
        }
    }
}
