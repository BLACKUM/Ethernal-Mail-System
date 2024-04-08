using __Wpf__App.MailWindow;
using __Wpf__App.Auth;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Data;

namespace __Wpf__App
{
    public partial class MainWindow : Window
    {
        DataBase database = new DataBase();
        private MessageCheckerThread _messageCheckerThread;
        public int userIdBack;
        private readonly EmailService _emailService;
        public MainWindow(int userId)
        {
            InitializeComponent();
            string connectionString = "Data Source=notebook-server\\sqlexpress;Initial Catalog=DBForMail;User=ADMAIL;Password=Fgadu!i2u0120i93udasj!";

            // Initialize the EmailService with the database connection string
            _emailService = new EmailService(connectionString);

            DateTime currentTime = DateTime.Now;
            DateTime modifiedTime = currentTime.AddSeconds(-1);
            _emailService.SetLastCheckTimeForUser(userId, modifiedTime);

            _emailService.NewMessageReceived += EmailService_NewMessageReceived;

            List<string> usernames = _emailService.GetAllUsernames();

            UsernameCombobox.ItemsSource = usernames;
            UsernameCombobox.SelectedIndex = 0;

            _messageCheckerThread = new MessageCheckerThread(_emailService, userId, TimeSpan.FromSeconds(1));
            _messageCheckerThread.Start();

            DataBase db = new DataBase();
            userIdBack = userId;
            try
            {
                db.openConnection();
                string query = "SELECT user_name FROM Users WHERE user_id = @user_id";
                SqlCommand command = new SqlCommand(query, db.getConnection());
                command = new SqlCommand(query, db.getConnection());
                command.Parameters.AddWithValue("@user_id", userIdBack);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        object user_nameObj = reader["user_name"];
                        if (user_nameObj != DBNull.Value)
                        {
                            string user_name = user_nameObj.ToString();
                            UsernameTextBox.Content = "Добро пожаловать, "+user_name;
                            db.closeConnection();
                            return;
                        }
                        else
                        {
                            string text = "ОШИБКА";
                            UsernameTextBox.Content = text;
                            db.closeConnection();
                            return;
                        }
                    }
                    else
                    {
                        string text = "ОШИБКА";
                        UsernameTextBox.Content = text;
                        db.closeConnection();
                        return;
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Ошибка подключения к базе данных: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка: " + ex.Message);
            }
            finally
            {
                Mouse.OverrideCursor = null;
                if (db.getConnection().State == ConnectionState.Open)
                {
                    db.closeConnection();
                }
            }
            CheckNewMessages();
            FetchAndPopulateMessages(userId);
            
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Dispose of the MessageCheckerThread
            _messageCheckerThread.Dispose();
        }

        private void EmailService_NewMessageReceived(object sender, EmailEventArgs e)
        {
            // Отображаем уведомление с информацией о новом сообщении
            ShowNotification("Новое сообщение", $"Отправитель: {e.NewMessage.sender_name}\nТема: {e.NewMessage.subject}");
        }

        private void ShowNotification(string title, string message)
        {
            var notificationWindow = new NotificationWindow(title, message);
            notificationWindow.ShowDialog();
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
            var Auth = new Auth.Auth();
            Auth.Show();
            this.Close();
        }

        private void SendEmailButton_Click(object sender, RoutedEventArgs e)
        {

            var SendMail = new SendMail(userIdBack);
            SendMail.Show();
            this.Hide();
            
        }

        private void CheckNewMessages()
        {
            List<Email> newMessages = _emailService.GetAllMessagesForUser(userIdBack);

            if (newMessages.Count > 0)
            {
                Dispatcher.Invoke(() =>
                {
                    messageDataGrid.Items.Clear(); 
                    foreach (Email email in newMessages)
                    {
                        messageDataGrid.Items.Add(email); 
                    }
                });
            }
        }

        public void UpdateDataGridWithNewMessages()
        {
            List<Email> newMessages = _emailService.GetAllMessagesForUser(userIdBack);
            messageDataGrid.ItemsSource = newMessages;
        }
        

        private void OnMessageSelected(object sender, RoutedEventArgs e)
        {
            
            if (messageDataGrid.SelectedItem != null && messageDataGrid.SelectedItem is Email selectedEmail)
            {
                var readMessage = new ReadMessage(selectedEmail, userIdBack);
                readMessage.Show();
                this.Hide();
            }
        }


        private void Reload_Click(object sender, RoutedEventArgs e)
        {
            CheckNewMessages();
        }

        private void FetchAndPopulateMessages(int userId)
        {
            List<Email> messages = _emailService.GetAllMessagesForUser(userId);
            Dispatcher.Invoke(() =>
            {
                messageDataGrid.ItemsSource = messages;
            });
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CheckNewMessages();
        }
    }
}
