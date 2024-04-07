using __Wpf__App.MailWindow;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Windows;

namespace __Wpf__App
{
    public partial class MainWindow : Window
    {

        private readonly EmailService _emailService;
        private const int UserId = 1; // Replace with the actual user ID
        private Timer _checkMessagesTimer;
        public MainWindow()
        {
            InitializeComponent();
            string connectionString = "Data Source=notebook-server\\sqlexpress;Initial Catalog=DBForMail;User=ADMAIL;Password=Fgadu!i2u0120i93udasj!";

            // Initialize the EmailService with the database connection string
            _emailService = new EmailService(connectionString);

            List<string> usernames = _emailService.GetAllUsernames();

            UsernameCombobox.ItemsSource = usernames;
            UsernameCombobox.SelectedIndex = 0;

            // Start a timer to check for new messages every 5 seconds
            _checkMessagesTimer = new Timer(CheckNewMessages, null, TimeSpan.Zero, TimeSpan.FromSeconds(1.5));
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

        private void CheckNewMessages(object state)
        {
            List<Email> newMessages = _emailService.GetAllMessagesForUser(UserId);

            if (newMessages.Count > 0)
            {
                Dispatcher.Invoke(() =>
                {
                    messageDataGrid.Items.Clear(); // Clear the existing items
                    foreach (Email email in newMessages)
                    {
                        messageDataGrid.Items.Add(email); // Add the new messages
                    }
                });
            }
        }

        public void UpdateDataGridWithNewMessages()
        {
            List<Email> newMessages = _emailService.GetAllMessagesForUser(UserId);
            //sender_id
            messageDataGrid.ItemsSource = newMessages;
        }
        /* 
        private async Task LoadDataRaspisanie()
        {
            var КурсыIds = await dbEnt.Пользователи_Курсы.ToListAsync();
            var расписание = await dbEnt.Расписание.ToListAsync();
            var назвКурса = await dbEnt.Курсы.ToListAsync();
            Dispatcher.Invoke(() =>
            {
            NameKursRaspisanie.ItemsSource = назвКурса;
            NameKursRaspisanie.DisplayMemberPath = "Название";
            NameKursRaspisanie.SelectedValuePath = "ID_Курса";
            расписаниеDataGridRaspisanie.ItemsSource = расписание;
            originalListRaspisanie = расписание;
            });
            }
        */

        private void OnMessageSelected(object sender, RoutedEventArgs e)
        {
            List<Email> newMessages = _emailService.GetAllMessagesForUser(UserId);
            var ReadMessage = new ReadMessage(newMessages);
            ReadMessage.ShowDialog();
            this.Hide();
        }

    }
}