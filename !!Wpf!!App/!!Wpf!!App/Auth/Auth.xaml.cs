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
        private void Enter_Click(object sender, RoutedEventArgs e)
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
                db.openConnection();
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
                    var MainWindow = new MainWindow();
                    MainWindow.Show();
                    this.Hide();
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