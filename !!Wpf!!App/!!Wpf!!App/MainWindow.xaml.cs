using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;

namespace __Wpf__App
{
    public partial class MainWindow : Window
    {
        private readonly EmailService _emailService;
        private const int UserId = 1; // Replace with the actual user ID

        public MainWindow()
        {
            InitializeComponent();
            string connectionString = "Data Source=notebook-server\\sqlexpress;Initial Catalog=DBForMail;User=ADMAIL;Password=Fgadu!i2u0120i93udasj!";
           

            // Initialize the EmailService with the database connection string
            _emailService = new EmailService(connectionString);

            List<string> usernames = _emailService.GetAllUsernames();

            UsernameCombobox.ItemsSource= usernames;
            UsernameCombobox.SelectedIndex=0;

        }

        private void SendEmailButton_Click(object sender, RoutedEventArgs e)
        {
            Email newEmail = new Email
            {
                sender_id = UserId,
                recipient_id = 1, // Replace with the actual recipient ID
                subject = "Test Email",
                body = "This is a test email message."
            };

            _emailService.SendMessage(newEmail);
            OutputTextBlock.Text += "Email sent successfully.\n";
        }

        private void CheckNewEmailsButton_Click(object sender, RoutedEventArgs e)
        {
            List<Email> newMessages = _emailService.PollForNewMessages(UserId);

            if (newMessages.Count > 0)
            {
                OutputTextBlock.Text += $"Received {newMessages.Count} new emails:\n";
                foreach (Email email in newMessages)
                {
                    OutputTextBlock.Text += $"From: {email.sender_id}, Subject: {email.subject}\n";
                    OutputTextBlock.Text += $"{email.body}\n";
                    OutputTextBlock.Text += $"Sent at: {email.send_time}\n\n";
                }
            }
            else
            {
                OutputTextBlock.Text += "No new emails.\n";
            }
        }
    }
}