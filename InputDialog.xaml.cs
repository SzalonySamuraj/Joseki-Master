using System.Windows;

namespace JosekiMaster
{
    public partial class InputDialog : Window
    {
        public enum InputType
        {
            Text,
            Password
        }

        private InputType _inputType = InputType.Text;

        public InputDialog(string question, string title, string defaultValue = "", InputType inputType = InputType.Text)
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(InputDialog_Loaded);
            txtQuestion.Text = question;
            Title = title;
            txtResponse.Text = defaultValue;
            _inputType = inputType;

            if (_inputType == InputType.Password)
            {
                txtResponse.Visibility = Visibility.Collapsed;
            }
            else
            {
                txtPasswordResponse.Visibility = Visibility.Collapsed;
            }
        }

        void InputDialog_Loaded(object sender, RoutedEventArgs e)
        {
            if (_inputType == InputType.Password)
            {
                txtPasswordResponse.Focus();
            }
            else
            {
                txtResponse.Focus();
            }
        }

        public static string Prompt(string question, string title, string defaultValue = "", InputType inputType = InputType.Text)
        {
            var inst = new InputDialog(question, title, defaultValue, inputType);
            return inst.ShowDialog() == true ? inst.ResponseText : null;
        }

        public string ResponseText
        {
            get
            {
                return _inputType == InputType.Password ? txtPasswordResponse.Password : txtResponse.Text;
            }
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
