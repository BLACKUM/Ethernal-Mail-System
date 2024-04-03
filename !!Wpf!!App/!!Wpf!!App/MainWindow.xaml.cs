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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace __Wpf__App
{
    
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private EmailService _emailService;
        private System.Threading.Timer _pollTimer;

        public MainWindow()
        {
            InitializeComponent();
            string connectionString = "Data Source=yourServer;Initial Catalog=yourDatabase;Integrated Security=True";
            _emailService = new EmailService(connectionString);

            // Set up a timer to poll for new messages every 5 seconds
            _pollTimer = new System.Threading.Timer(_emailService.PollForNewMessagesCallback, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
        }
        private void PollForNewMessagesCallback(object state)
        {
            int userId = 1; // Replace with the actual user ID

            List<Email> newMessages = _emailService.PollForNewMessages(userId);

            // Update the UI with the new messages
            Dispatcher.Invoke(() =>
            {
                foreach (Email email in newMessages)
                {
                    // Add the email to the inbox UI control or display a notification
                    // ...
                }
            });

        }
    }
