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
    /// Логика взаимодействия для SendMail.xaml
    /// </summary>
    public partial class SendMail : Window
    {
        public int userIdBack;
        public SendMail(int userId)
        {
            InitializeComponent();
            userIdBack = userId;
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
        private void Test_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {

        }
        /*
        private void BackTo_Click(object sender, RoutedEventArgs e)
        {
            var MainWindow = new MainWindow(userIdBack);
            MainWindow.Show();
            this.Close();
        }
        */
    }
}
