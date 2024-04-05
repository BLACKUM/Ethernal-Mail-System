using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace __Wpf__App
{
    internal class DataBase
    {
        SqlConnection sqlConnection = new SqlConnection(@"Data Source=Notebook-Server\SQLEXPRESS;Initial Catalog=TODO;Persist Security Info=True;User ID=ADMAIL;Password=Fgadu!i2u0120i93udasj!");


        public void openConnection()
        {
            if (sqlConnection.State == System.Data.ConnectionState.Closed)
            {
                sqlConnection.Open();
            }
        }

        public void closeConnection()
        {
            if (sqlConnection.State == System.Data.ConnectionState.Open)
            {
                sqlConnection.Close();
            }
        }

        public SqlConnection getConnection()
        {
            return sqlConnection;
        }

    }
}