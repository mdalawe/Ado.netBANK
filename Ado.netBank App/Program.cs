using System;

namespace BankApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string connStr = "Server=localhost;Database=BankInfoDB;Integrated Security=True;TrustServerCertificate=True;";
            BankDatabase bankDb = new BankDatabase(connStr);

            try
            {
                Console.WriteLine("1. Create Account");
                Console.WriteLine("2. Login and Show Balance");
                Console.WriteLine("3. Withdraw");
                Console.WriteLine("4. Deposit");
                Console.Write("Choose an option: ");
                string choice = Console.ReadLine()!;

                switch (choice)
                {
                    case "1":
                        bankDb.CreateAccount();
                        break;
                    case "2":
                        bankDb.LoginAndShowBalance();
                        break;
                    case "3":
                        bankDb.Withdraw();
                        break;
                    case "4":
                        bankDb.Deposit();
                        break;
                    default:
                        Console.WriteLine("Invalid option selected.");
                        break;
                }
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid input format. Please enter correct values.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unexpected error: " + ex.Message);
            }
        }
    }
}
