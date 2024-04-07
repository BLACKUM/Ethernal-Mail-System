using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Data;
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
        public int userIdBack;
        DataBase database = new DataBase();
        public Auth()
        {
            InitializeComponent();
            Application.Current.MainWindow.ForceCursor = true;
            Cursor = Cursors.AppStarting;
            Loaded += (s, e) =>
            {
                Application.Current.MainWindow.ForceCursor = false;
                Cursor = Cursors.Arrow;
            };
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
        private async Task Enter_ClickAsync(object sender, RoutedEventArgs e)
        {
            var LoginUser = loginBox.Text;
            var PasswordUser = passBox.Password;
            if (LoginUser.Length < 5 || PasswordUser.Length < 5)
            {
                string text = "Логин и пароль должны содержать не менее 5 символов.";
                Text.Content = text;
                return;
            }
            DataBase db = new DataBase();
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                await db.openConnectionAsync();
                string query = "SELECT COUNT(*) FROM Users WHERE user_name = @user_name";
                SqlCommand command = new SqlCommand(query, db.getConnection());
                command.Parameters.AddWithValue("@user_name", LoginUser);
                int count = Convert.ToInt32(command.ExecuteScalar());
                if (count == 0)
                {
                    string text = "Пользователь с таким логином не найден.";
                    Text.Content = text;
                    db.closeConnection();
                    return;
                }
                query = "SELECT user_id FROM Users WHERE user_name = @user_name AND user_password = @user_password";
                command = new SqlCommand(query, db.getConnection());
                command.Parameters.AddWithValue("@user_name", LoginUser);
                command.Parameters.AddWithValue("@user_password", PasswordUser);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        int userId = reader.GetInt32(reader.GetOrdinal("user_id"));
                        userIdBack = userId;
                        var MainWindow = new MainWindow(userIdBack);
                        MainWindow.Show();
                        this.Hide();
                    }
                    else
                    {
                        string text = "Не верный логин или пароль.";
                        Text.Content = text;
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
        private void Reg_Click(object sender, RoutedEventArgs e)
        {
            var registration = new Reg();
            registration.Show();
            this.Hide();
        }
    }
}