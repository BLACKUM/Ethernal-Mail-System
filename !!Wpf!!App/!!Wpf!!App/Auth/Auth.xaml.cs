using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

namespace __Wpf__App.Auth
{
    /// <summary>
    /// Логика взаимодействия для Auth.xaml
    /// </summary>
    public partial class Auth : Window
    {
        DataBase database = new DataBase();
        public Auth()
        {
            InitializeComponent();
        }

        private void Collapse_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Enter_Click(object sender, RoutedEventArgs e)
        {
            var LoginUser = loginBox.Text;
            var PasswordUser = passBox.Password;

            // Объявление и инициализация переменной openConnection
            SqlConnection openConnection = new SqlConnection(@"Data Source=Notebook-Server\SQLEXPRESS;Initial Catalog=TODO;Persist Security Info=True;User ID=ADMAIL;Password=Fgadu!i2u0120i93udasj!");
            openConnection.Open();

            if (LoginUser.Length >= 5 && PasswordUser.Length >= 5)
            {
                // Проверка наличия пользователя в базе данных
                string query = "SELECT COUNT(*) FROM Регистрация WHERE Логин = @login";
                SqlCommand command = new SqlCommand(query);
                command.Parameters.AddWithValue("@login", LoginUser);

                // Установка свойства Connection
                command.Connection = openConnection;

                int count = Convert.ToInt32(command.ExecuteScalar());

                if (count == 0)
                {
                    // Ошибка авторизации
                    string text = "Пользователь с таким логином не найден.";
                    Text.Content = text;
                    return;
                }

                // Проверка логина и пароля в базе данных
                query = "SELECT Уровень_доступа FROM Регистрация WHERE Логин = @login AND Пароль = @password";
                command = new SqlCommand(query);
                command.Parameters.AddWithValue("@login", LoginUser);
                command.Parameters.AddWithValue("@password", PasswordUser);

                // Установка свойства Connection
                command.Connection = openConnection;

                string accessLevel = command.ExecuteScalar()?.ToString();

                if (accessLevel == "1")
                {
                    // хз
                }
                else if (accessLevel == "2")
                {
                    // хз
                }
                else
                {
                    // Ошибка авторизации
                    string text = "Не верный логин или пароль.";
                    Text.Content = text;
                }

            }
            else
            {
                // Ошибка авторизации
                string text = "Логин и пароль должны содержать не менее 5 символов.";
                Text.Content = text;
            }

            // Закрытие соединения с базой данных
            openConnection.Close();
        }
        private void Reg_Click(object sender, RoutedEventArgs e)
        {
            /* var registration = new Registration();
            registration.Show();
            this.Hide(); */
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите закрыть окно?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                System.Windows.Application.Current.Shutdown();
            }
        }

        private void kostil_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            mw.Show();
            this.Hide();
        }

        private void kostil2_Click(object sender, RoutedEventArgs e)
        {
            /* var PrepodKursi = new PrepodKursi();
            PrepodKursi.Show();
            this.Hide(); */
        }
    }
}
