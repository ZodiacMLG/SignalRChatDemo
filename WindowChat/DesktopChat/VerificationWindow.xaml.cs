using System.Windows;

namespace DesktopChat
{
    /// <summary>
    /// Interaction logic for VerificationWindow.xaml
    /// </summary>
    public partial class VerificationWindow : Window
    {
        public VerificationWindow()
        {
            InitializeComponent();
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(nameBox.Text)) {
                MessageBox.Show("Введите имя пользователя");
            }
            else
            {
                this.DialogResult = true;
            }
        }

        public string Name
        {
            get
            {
                return nameBox.Text;
            }
        }
    }
}
