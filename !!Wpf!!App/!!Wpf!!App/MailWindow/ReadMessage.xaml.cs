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
    /// Логика взаимодействия для ReadMessage.xaml
    /// </summary>
    public partial class ReadMessage : Window
    {
        public int userIdBack;
        
        private Email _selectedEmail;

        private readonly EmailService _emailService;

        public ReadMessage(Email email, int userId, EmailService emailService)
        {
            InitializeComponent();
            userIdBack = userId;
            _selectedEmail = email;
            _emailService = emailService;

            // Update the read status for the selected email
            _emailService.UpdateReadStatus(_selectedEmail.message_id, true);

            DisplayMessage();
        }
        private void DisplayMessage()
        {
            SenderLabel.Content = $"Отправитель: {_selectedEmail.sender_name}";
            SubjectLabel.Content = $"Тема: {_selectedEmail.subject}";
            BodyTextBox.Text = _selectedEmail.body;
            DateLabel.Content= $"Дата: {_selectedEmail.send_time}";
        }

        private void Drag_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
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

        private void BackTo_Click(object sender, RoutedEventArgs e)
        {
            var MainWindow = new MainWindow(userIdBack);
            MainWindow.Show();
            this.Close();
        }
    }
}
