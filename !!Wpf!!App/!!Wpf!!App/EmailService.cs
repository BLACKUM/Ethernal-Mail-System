using System;
using System.Collections.Generic;
using System.Data.SqlClient;

public class Email
{
    public int message_id { get; set; }
    public int sender_id { get; set; }
    public int recipient_id { get; set; }
    public string subject { get; set; }
    public string body { get; set; }
    public DateTime send_time { get; set; }
    public bool read_status { get; set; }
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

            // Insert the email details into the Messages table
            string insertQuery = "INSERT INTO Messages (sender_id, recipient_id, subject, body, send_time, read_status) VALUES (@SenderID, @RecipientID, @Subject, @Body, @SentTime, @ReadStatus)";
            SqlCommand command = new SqlCommand(insertQuery, connection);
            command.Parameters.AddWithValue("@SenderID", email.sender_id);
            command.Parameters.AddWithValue("@RecipientID", email.recipient_id);
            command.Parameters.AddWithValue("@Subject", email.subject);
            command.Parameters.AddWithValue("@Body", email.body);
            command.Parameters.AddWithValue("@SentTime", DateTime.UtcNow);
            command.Parameters.AddWithValue("@ReadStatus", false); // Set read_status to false for a new email

            command.ExecuteNonQuery();
        }
    }

    public List<Email> PollForNewMessages(int userId)
    {
        List<Email> newMessages = new List<Email>();

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            // Get the user's last check time from the database
            string getUserLastCheckTimeQuery = "SELECT last_check_time FROM Users_Last_Check_Time WHERE user_id = @UserID";
            SqlCommand getUserLastCheckTimeCommand = new SqlCommand(getUserLastCheckTimeQuery, connection);
            getUserLastCheckTimeCommand.Parameters.AddWithValue("@UserID", userId);
            DateTime? lastCheckTime = (DateTime?)getUserLastCheckTimeCommand.ExecuteScalar();

            // Get new messages for the user since the last check time
            string getNewMessagesQuery = "SELECT * FROM Messages WHERE recipient_id = @UserID AND send_time > @LastCheckTime";
            SqlCommand getNewMessagesCommand = new SqlCommand(getNewMessagesQuery, connection);
            getNewMessagesCommand.Parameters.AddWithValue("@UserID", userId);
            getNewMessagesCommand.Parameters.AddWithValue("@LastCheckTime", lastCheckTime ?? DateTime.MinValue);

            using (SqlDataReader reader = getNewMessagesCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    Email email = new Email
                    {
                        message_id = (int)reader["message_id"],
                        sender_id = (int)reader["sender_id"],
                        recipient_id = (int)reader["recipient_id"],
                        subject = reader["subject"].ToString(),
                        body = reader["body"].ToString(),
                        send_time = (DateTime)reader["send_time"],
                        read_status = (bool)reader["read_status"]
                    };

                    newMessages.Add(email);
                }
            }

            // Update the user's last check time in the database
            string updateLastCheckTimeQuery = "UPDATE Users_Last_Check_Time SET last_check_time = @CurrentTime WHERE user_id = @UserID";
            SqlCommand updateLastCheckTimeCommand = new SqlCommand(updateLastCheckTimeQuery, connection);
            updateLastCheckTimeCommand.Parameters.AddWithValue("@UserID", userId);
            updateLastCheckTimeCommand.Parameters.AddWithValue("@CurrentTime", DateTime.UtcNow);
            updateLastCheckTimeCommand.ExecuteNonQuery();
        }

        return newMessages;
    }
}