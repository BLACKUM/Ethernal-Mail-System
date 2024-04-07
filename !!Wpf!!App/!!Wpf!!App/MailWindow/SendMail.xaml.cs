using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace __Wpf__App.MailWindow
{
    /// <summary>
    /// Логика взаимодействия для SendMail.xaml
    /// </summary>
    public partial class SendMail : Window
    {
        private readonly EmailService _emailService;

        public int userIdBack;

        public SendMail(int userId)
        {
            InitializeComponent();
            userIdBack = userId;
            string connectionString = "Data Source=notebook-server\\sqlexpress;Initial Catalog=DBForMail;User=ADMAIL;Password=Fgadu!i2u0120i93udasj!";

            _emailService = new EmailService(connectionString);
            List<string> usernames = _emailService.GetAllUsernames();

            UsernameCombobox.ItemsSource = usernames;
            UsernameCombobox.SelectedIndex = 0;
        }

        

        private void Collapse_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите закрыть окно?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                System.Windows.Application.Current.Shutdown();
            }
        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            // Get the selected recipient name from the ComboBox
            string recipientName = UsernameCombobox.SelectedItem.ToString();

            // Find the recipient ID from the database using the recipient name
            int recipientId = _emailService.GetUserIdByName(recipientName);

            // Get the subject and body from the form
            string subject = SubjectTextBox.Text;
            string body = BodyTextBox.Text;

            // Create a new Email object with the recipient ID, subject, and body
            Email newEmail = new Email
            {
                sender_id = userIdBack,
                recipient_id = recipientId,
                subject = subject,
                body = body
            };

            // Send the email
            _emailService.SendMessage(newEmail);

            // Display a success message
            StatusLabel.Content = "Email sent successfully.";
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var MainWindow = new MainWindow(userIdBack);
            MainWindow.Show();
            this.Close();
        }
        private void Drag_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
    }
}
