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

            // Таймер для пула сообщений каждые 5 сек
            _pollTimer = new System.Threading.Timer(PollForNewMessagesCallback, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
        }
        private void PollForNewMessagesCallback(object state)
        {
            int userId = 1; //Нужно брать из настоящий айдишник 

            List<Email> newMessages = _emailService.PollForNewMessages(userId);

            // Тут закидываем сообщения на форму
            Dispatcher.Invoke(() =>
            {
                foreach (Email email in newMessages)
                {
                    // ...
                }
            });

        }
    }
}
