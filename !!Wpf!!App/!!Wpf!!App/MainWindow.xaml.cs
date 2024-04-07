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
        public int userIdBack;
        private readonly EmailService _emailService;
        private const int UserId = 1; // Replace with the actual user ID
        public MainWindow(int userId)
        {
            InitializeComponent();
            string connectionString = "Data Source=notebook-server\\sqlexpress;Initial Catalog=DBForMail;User=ADMAIL;Password=Fgadu!i2u0120i93udasj!";

            // Initialize the EmailService with the database connection string
            _emailService = new EmailService(connectionString);

            _emailService.NewMessageReceived += EmailService_NewMessageReceived;

            List<string> usernames = _emailService.GetAllUsernames();

            UsernameCombobox.ItemsSource = usernames;
            UsernameCombobox.SelectedIndex = 0;

            CheckNewMessages();

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
                            UsernameTextBox.Content = user_name;
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
            OutputTextBlock.Content += "Email sent successfully.\n";
        }

        private void CheckNewMessages()
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
            var ReadMessage = new ReadMessage(newMessages, userIdBack);
            ReadMessage.Show();
            this.Hide();
            if (messageDataGrid.SelectedItem != null && messageDataGrid.SelectedItem is Email selectedEmail)
            {
                var readMessage = new ReadMessage(selectedEmail);
                readMessage.Show();
                this.Hide();
            }
        }


        private void Reload_Click(object sender, RoutedEventArgs e)
        {
            CheckNewMessages();
        }
    }
}
