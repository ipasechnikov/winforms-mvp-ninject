using Ninject;

using WinFormsMvpNinject.App.Ninject.Modules;
using WinFormsMvpNinject.App.Views;

namespace WinFormsMvpNinject.Tests
{
    [TestFixture]
    internal class MemoryLeaksTest
    {
        private StandardKernel kernel = null!;

        [SetUp]
        public void SetUp()
        {
            // Use the same kernel as an application project
            // Creating a new kernel only for testing purposes might be a good idea but
            // it's a sample project. We don't care much about this in our case
            kernel = new StandardKernel(new MainModule());
        }

        [Test]
        public void MainForm_Presenter_IsNull_OnDispose()
        {
            var mainForm = kernel.Get<MainForm>();

            // Make sure that presenter was curretly injected
            Assert.IsNotNull(mainForm.Presenter);

            // Dispose form as if it was closed by the user or something
            mainForm.Dispose();

            // Make sure that presenter was curretly set to null by Disposed event handler
            // This means that mainForm will be collected by GC because cyclic references are resolved
            Assert.IsNull(mainForm.Presenter);
        }
    }
}
