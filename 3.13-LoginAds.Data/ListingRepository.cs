using BCrypt.Net;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace _3._13_LoginAds.Data
{
    public class ListingRepository
    {
        private readonly string _connectionString;

        public ListingRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Ad> GetAds()
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM Ads";
            connection.Open();

            var reader = cmd.ExecuteReader();
            List<Ad> ads = new();

            while(reader.Read())
            {
                ads.Add(new()
                {
                    Id = (int)reader["Id"],
                    Number = (decimal)reader["Number"],
                    Description = (string)reader["Description"],
                    UserId = (int)reader["UserId"]
                });
            }
            return ads;
        }

        public void AddAd(Ad ad)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"INSERT INTO Ads (Number, Description, UserId, Title) 
                                VALUES (@number, @desc, @userId, @title)";
            cmd.Parameters.AddWithValue("@number", ad.Number);
            cmd.Parameters.AddWithValue("@desc", ad.Description);
            cmd.Parameters.AddWithValue("@userId", ad.UserId);
            cmd.Parameters.AddWithValue("@title", ad.Title);
            connection.Open();
            cmd.ExecuteNonQuery();
        }

        public void AddUser(User user, string password)
        {
            var hash = BCrypt.Net.BCrypt.HashPassword(password);
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Users (Name, Email, PasswordHash) " +
                              "VALUES (@name, @email, @hash)";
            cmd.Parameters.AddWithValue("@name", user.Name);
            cmd.Parameters.AddWithValue("@email", user.Email);
            cmd.Parameters.AddWithValue("@hash", hash);
            connection.Open();
            cmd.ExecuteNonQuery();
        }

        public List<string> GetAllEmails()
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT Email FROM Users";
            connection.Open();

            var reader = cmd.ExecuteReader();
            List<string> emails = new();

            while(reader.Read())
            {
                emails.Add((string)reader["Email"]);
            }

            return emails;
        }

        public User Login(string email, string password)
        {
            var user = GetByEmail(email);

            if(user == null)
            {
                return null;
               
            }

            var isMatch = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            if(!isMatch)
            {
                return null;
            }

            return user;
        }

        public User GetByEmail(string email)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT TOP 1 * FROM Users WHERE Email = @email";
            cmd.Parameters.AddWithValue("@email", email);
            connection.Open();

            var reader = cmd.ExecuteReader();
            
            if(!reader.Read())
            {
                return null;
            }

            return new User
            {
                Id = (int)reader["Id"],
                Email = (string)reader["Email"],
                Name = (string)reader["Name"],
                PasswordHash = (string)reader["PasswordHash"]
            };
        }

        public List<Ad> GetAdsByUserId(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM Ads WHERE UserId = @id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();

            var reader = cmd.ExecuteReader();
            List<Ad> ads = new();

            while (reader.Read())
            {
                ads.Add(new()
                {
                    Id = (int)reader["Id"],
                    Number = (decimal)reader["Number"],
                    Description = (string)reader["Description"],
                    UserId = (int)reader["UserId"]
                });
            }
            return ads;
        }

        public void DeleteAdById(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "DELETE FROM Ads WHERE Id = @id;";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            cmd.ExecuteNonQuery();
        }
    }
}
