using System;
using Microsoft.Data.SqlClient;

namespace BankApp
{
    public class BankDatabase
    {
        private string connectionString;

        public BankDatabase(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void CreateAccount()
        {
            Console.Write("Enter your name: ");
            string name = Console.ReadLine();

            Console.Write("Enter your age: ");
            int age = Convert.ToInt32(Console.ReadLine());
            if (age < 18)
            {
                Console.WriteLine("You must be at least 18 to open an account.");
                return;
            }
            Console.Write("Enter your gender: ");
            string gender = Console.ReadLine();

            Console.Write("Enter your phone number: ");
            string phone = Console.ReadLine();
            if (phone.Length != 11)
            {
                Console.WriteLine("Phone number must be 11 digits.");
                return;
            }

            string accountNumber = phone.Substring(1);

            Console.WriteLine("This is your account number: " + accountNumber);

            Console.Write("\nSet a 4-digit PIN: ");
            string pin = Console.ReadLine();
            if (pin.Length != 4)
            {
                Console.WriteLine("PIN must be 4 digits.");
                return;
            }

            decimal balance = 100000;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "INSERT INTO BankUsers (Id, Name, Age, Gender, Phone, PIN, Balance, AccountNumber) " +
                               "VALUES (NEWID(), @Name, @Age, @Gender, @Phone, @PIN, @Balance, @AccountNumber)";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@Age", age);
                command.Parameters.AddWithValue("@Gender", gender);
                command.Parameters.AddWithValue("@Phone", phone);
                command.Parameters.AddWithValue("@PIN", pin);
                command.Parameters.AddWithValue("@Balance", balance);
                command.Parameters.AddWithValue("@AccountNumber", accountNumber);

                command.ExecuteNonQuery();

                Console.WriteLine("Account created successfully.");
            }
        }

        public void LoginAndShowBalance()
        {
            Console.Write("Enter your phone number: ");
            string phone = Console.ReadLine()!;

            Console.Write("Enter your PIN: ");
            string pin = Console.ReadLine()!;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT Name, Balance FROM BankUsers WHERE Phone = @Phone AND PIN = @PIN";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Phone", phone);
                command.Parameters.AddWithValue("@PIN", pin);

                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    string name = reader.GetString(0);
                    decimal balance = reader.GetDecimal(1);

                    Console.WriteLine($"Welcome, {name}! Your Accountbalance is : {balance}");
                }
                else
                {
                    Console.WriteLine("Invalid phone number or PIN.");
                }
            }
        }

        public void Withdraw()
        {
            Console.Write("Enter your phone number: ");
            string phone = Console.ReadLine()!;

            Console.Write("Enter your PIN: ");
            string pin = Console.ReadLine();

            Console.Write("Enter amount to withdraw: ");
            decimal amount = Convert.ToDecimal(Console.ReadLine());

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string checkQuery = "SELECT Balance FROM BankUsers WHERE Phone = @Phone AND PIN = @PIN";
                SqlCommand checkCommand = new SqlCommand(checkQuery, connection);
                checkCommand.Parameters.AddWithValue("@Phone", phone);
                checkCommand.Parameters.AddWithValue("@PIN", pin);

                object result = checkCommand.ExecuteScalar();

                if (result == null)
                {
                    Console.WriteLine("Invalid phone number or PIN.");
                    return;
                }

                decimal currentBalance = Convert.ToDecimal(result);

                if (amount > currentBalance)
                {
                    Console.WriteLine("Insufficient funds.");
                    return;
                }

                decimal newBalance = currentBalance - amount;

                string updateQuery = "UPDATE BankUsers SET Balance = @Balance WHERE Phone = @Phone AND PIN = @PIN";
                SqlCommand updateCommand = new SqlCommand(updateQuery, connection);
                updateCommand.Parameters.AddWithValue("@Balance", newBalance);
                updateCommand.Parameters.AddWithValue("@Phone", phone);
                updateCommand.Parameters.AddWithValue("@PIN", pin);

                updateCommand.ExecuteNonQuery();
                Console.WriteLine($"Withdrawal successful. New balance: {newBalance}");
            }
        }

        public void Deposit()
        {
            Console.Write("Enter your phone number: ");
            string phone = Console.ReadLine()!;

            Console.Write("Enter your PIN: ");
            string pin = Console.ReadLine()!;

            Console.Write("Enter amount to deposit: ");
            decimal amount = Convert.ToDecimal(Console.ReadLine());

            if (amount <= 0)
            {
                Console.WriteLine("Invalid deposit amount.");
                return;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string checkQuery = "SELECT Balance FROM BankUsers WHERE Phone = @Phone AND PIN = @PIN";
                SqlCommand checkCommand = new SqlCommand(checkQuery, connection);
                checkCommand.Parameters.AddWithValue("@Phone", phone);
                checkCommand.Parameters.AddWithValue("@PIN", pin);

                object result = checkCommand.ExecuteScalar();

                if (result == null)
                {
                    Console.WriteLine("Invalid phone number or PIN.");
                    return;
                }

                decimal currentBalance = Convert.ToDecimal(result);
                decimal newBalance = currentBalance + amount;

                string updateQuery = "UPDATE BankUsers SET Balance = @Balance WHERE Phone = @Phone AND PIN = @PIN";
                SqlCommand updateCommand = new SqlCommand(updateQuery, connection);
                updateCommand.Parameters.AddWithValue("@Balance", newBalance);
                updateCommand.Parameters.AddWithValue("@Phone", phone);
                updateCommand.Parameters.AddWithValue("@PIN", pin);

                updateCommand.ExecuteNonQuery();
                Console.WriteLine($"Deposit successful. New balance: {newBalance}");
            }
        }
    }
}
