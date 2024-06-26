﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;

public class Email
{
    public int message_id { get; set; }
    public int sender_id { get; set; }
    public string sender_name { get; set; }
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

    public List<Email> GetAllMessagesForUser(int userId)
    {
        List<Email> messages = new List<Email>();

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            // Get all messages for the user along with the sender's name
            string getMessagesQuery = @"SELECT m.*, u.user_name AS sender_name
                                    FROM Messages m
                                    JOIN Users u ON m.sender_id = u.user_id
                                    WHERE m.recipient_id = @UserID";
            SqlCommand getMessagesCommand = new SqlCommand(getMessagesQuery, connection);
            getMessagesCommand.Parameters.AddWithValue("@UserID", userId);

            using (SqlDataReader reader = getMessagesCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    Email email = new Email
                    {
                        message_id = (int)reader["message_id"],
                        sender_id = (int)reader["sender_id"],
                        sender_name = reader["sender_name"].ToString(), // Get the sender's name
                        recipient_id = (int)reader["recipient_id"],
                        subject = reader["subject"].ToString(),
                        body = reader["body"].ToString(),
                        send_time = (DateTime)reader["send_time"],
                        read_status = (bool)reader["read_status"]
                    };

                    messages.Add(email);
                }
            }
        }

        return messages;
    }



    public List<string> GetAllUsernames()
    {
        List<string> usernames = new List<string>();

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            string query = "SELECT user_name FROM Users";
            SqlCommand command= new SqlCommand(query, connection);

            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    string username = reader["user_name"].ToString();
                    usernames.Add(username);
                }
            }
        }
        return usernames;
    }
}