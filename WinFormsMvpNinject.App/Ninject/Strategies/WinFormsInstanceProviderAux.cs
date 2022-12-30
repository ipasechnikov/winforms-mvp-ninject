using System.Diagnostics;

using Ninject;

namespace WinFormsMvpNinject.App.Ninject.Strategies
{
    /// <summary>
    /// Extension methods for Ninject IKernel used by WindowsFormsStrategy
    /// 
    /// Many thanks to this answer on StackOverflow
    /// https://stackoverflow.com/a/33928388
    /// </summary>
    static public class WinFormsInstanceProviderAux
    {
        static public void InjectDescendantOf(this IKernel kernel, ContainerControl containerControl)
        {
            var childrenControl = containerControl.Controls.Cast<Control>();
            foreach (var control in childrenControl)
                kernel.InjectUserControlOf(containerControl);
        }

        static public void InjectUserControlOf(this IKernel kernel, Control control)
        {
            // Only user controls can have properties defined as n-inject-able
            if (control is UserControl)
            {
                Trace.TraceInformation("Injecting dependencies in User Control of type {0}", control.GetType());
                kernel.Inject(control);
            }

            // A non user control can have children that are user controls and should be n-injected
            var childrenControl = control.Controls.Cast<Control>();
            foreach (var childControl in childrenControl)
                kernel.InjectUserControlOf(childControl);
        }
    }
}
