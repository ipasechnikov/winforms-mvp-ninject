using Ninject;

using WinFormsMvpNinject.App.Factories;
using WinFormsMvpNinject.App.Models;
using WinFormsMvpNinject.App.Presenters;

namespace WinFormsMvpNinject.App.Views
{
    public partial class MainForm : Form, IMainView
    {
        private IMainPresenter? presenter;

        public MainForm()
        {
            InitializeComponent();
            Disposed += (sender, args) =>
            {
                // Resolve cyclic reference to let GC collect the objects
                if (presenter != null)
                {
                    presenter.View = null;
                    presenter = null;
                }
            };
        }

        [Inject]
        public IMainPresenter? Presenter
        {
            get => presenter;
            set
            {
                // View can have only single presenter associated with it
                if (presenter != null)
                    presenter.View = null;

                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                // Set and initialize a new presenter
                presenter = value;
                presenter.View = this;
            }
        }

        [Inject]
        public IViewFactory? ViewFactory
        {
            get; set;
        }

        public IUser[] Users
        {
            get => (IUser[])grdUsers.DataSource;
            set => grdUsers.DataSource = value;
        }

        private void btnGetUsers_Click(object sender, EventArgs e)
        {
            Presenter!.GetUsers();
        }

        private void btnOpenSomeRandomView_Click(object sender, EventArgs e)
        {
            // This is how you create an injected form or control during runtime. Amazing!
            var form = (Form)ViewFactory!.CreateSomeRandomView();
            form.Show();
        }
    }
}
