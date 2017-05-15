using System.Windows;
using Caliburn.Micro;

namespace EnigmaGUI
{
    public class EnigmaBootstrapper : BootstrapperBase
    {
        public EnigmaBootstrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<EnigmaViewModel>();
        }
    }

    public class EnigmaViewModel : PropertyChangedBase
    {
        private string _name;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                NotifyOfPropertyChange(() => Name);
                NotifyOfPropertyChange(() => CanSayHello);
            }
        }

        public bool CanSayHello => !string.IsNullOrWhiteSpace(Name);

        public void SayHello()
        {
            MessageBox.Show($"Hello {Name}!");
        }
    }

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }
    }
}
