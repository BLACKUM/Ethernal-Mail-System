using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace __Wpf__App
{
    public class Email
    {
        public int MessageID { get; set; }
        public int SenderID { get; set; }
        public int RecipientID { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTime SentTime { get; set; }

    }

    public class EmailService
    {
        private readonly string _connectionString;

        public EmailService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void SendMessage(Email email)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                // Отправление сообщение в бд
                string insertQuery = "INSERT INTO Messages (SenderID, RecipientID, Subject, Body, SentTime) VALUES (@SenderID, @RecipientID, @Subject, @Body, @SentTime)";
                SqlCommand command = new SqlCommand(insertQuery, connection);
                command.Parameters.AddWithValue("@SenderID", email.SenderID);
                command.Parameters.AddWithValue("@RecipientID", email.RecipientID);
                command.Parameters.AddWithValue("@Subject", email.Subject);
                command.Parameters.AddWithValue("@Body", email.Body);
                command.Parameters.AddWithValue("@SentTime", DateTime.UtcNow);

                command.ExecuteNonQuery();
            }
        }

        public List<Email> PollForNewMessages(int userId)
        {
            List<Email> newMessages = new List<Email>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                // Провереям когда юзер чекал сообщения последний раз
                string getUserLastCheckTimeQuery = "SELECT LastCheckTime FROM UserLastCheckTime WHERE UserID = @UserID";
                SqlCommand getUserLastCheckTimeCommand = new SqlCommand(getUserLastCheckTimeQuery, connection);
                getUserLastCheckTimeCommand.Parameters.AddWithValue("@UserID", userId);
                DateTime? lastCheckTime = (DateTime?)getUserLastCheckTimeCommand.ExecuteScalar();

                // Получаем все новые сообщения с момента последней проверки юзера
                string getNewMessagesQuery = "SELECT * FROM Messages WHERE RecipientID = @UserID AND SentTime > @LastCheckTime";
                SqlCommand getNewMessagesCommand = new SqlCommand(getNewMessagesQuery, connection);
                getNewMessagesCommand.Parameters.AddWithValue("@UserID", userId);
                getNewMessagesCommand.Parameters.AddWithValue("@LastCheckTime", lastCheckTime ?? DateTime.MinValue);

                using (SqlDataReader reader = getNewMessagesCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Email email = new Email
                        {
                            MessageID = (int)reader["MessageID"],
                            SenderID = (int)reader["SenderID"],
                            RecipientID = (int)reader["RecipientID"],
                            Subject = reader["Subject"].ToString(),
                            Body = reader["Body"].ToString(),
                            SentTime = (DateTime)reader["SentTime"]
                        };

                        newMessages.Add(email);
                    }
                }

                // Обнавляем последний время последней проверки юзера
                string updateLastCheckTimeQuery = "UPDATE UserLastCheckTime SET LastCheckTime = @CurrentTime WHERE UserID = @UserID";
                SqlCommand updateLastCheckTimeCommand = new SqlCommand(updateLastCheckTimeQuery, connection);
                updateLastCheckTimeCommand.Parameters.AddWithValue("@UserID", userId);
                updateLastCheckTimeCommand.Parameters.AddWithValue("@CurrentTime", DateTime.UtcNow);
                updateLastCheckTimeCommand.ExecuteNonQuery();
            }

            return newMessages;
        }
    }
}
