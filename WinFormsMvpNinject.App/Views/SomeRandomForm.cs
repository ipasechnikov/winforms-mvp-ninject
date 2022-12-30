using WinFormsMvpNinject.App.Presenters;

namespace WinFormsMvpNinject.App.Views
{
    public partial class SomeRandomForm : Form, ISomeRandomView
    {
        private ISomeRandomPresenter? presenter;

        public SomeRandomForm()
        {
            InitializeComponent();
            Disposed += (sender, args) =>
            {
                if (presenter != null)
                {
                    presenter.View = null;
                    presenter = null;
                }
            };
        }

        public ISomeRandomPresenter? Presenter
        {
            get => presenter;
            set
            {
                if (presenter != null)
                    presenter.View = null;

                if (value == null)
                    throw new ArgumentException(nameof(value));

                presenter = value;
                presenter.View = this;
            }
        }
    }
}
