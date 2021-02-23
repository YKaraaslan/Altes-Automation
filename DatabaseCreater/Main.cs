using DatabaseCreater.Tables;
using MySql.Data.MySqlClient;
using Shared;
using System;

namespace DatabaseCreater
{
    class Main
    {
        public Main()
        {
            Console.WriteLine("\n\nInside OnMain(), creating AltesDB...");
            using (MySqlConnection localDbConnection = new MySqlConnection(Path.toDBForCreatingDB))
            {
                localDbConnection.Open();
                MySqlCommand command = new MySqlCommand("CREATE DATABASE IF NOT EXISTS `Unity`", localDbConnection);
                command.ExecuteNonQuery();
            }
            Console.WriteLine("AltesDB created successfully");
        }

        public void onStart()
        {
            try
            {
                Console.WriteLine("\n\nCreating Admins...");
                Admins adminsTable = new Admins();
                adminsTable.Create();
                Console.WriteLine("Admins created successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nERROR: " + ex.Message);
            }

            try
            {
                Console.WriteLine("\n\nCreating Confirmations...");
                Confirmation confirmationsTable = new Confirmation();
                confirmationsTable.Create();
                Console.WriteLine("Confirmations created successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nERROR: " + ex.Message);
            }

            try
            {
                Console.WriteLine("\n\nCreating Devices...");
                Devices deviceTables = new Devices();
                deviceTables.Create();
                Console.WriteLine("Confirmations created successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nERROR: " + ex.Message);
            }

            try
            {
                Console.WriteLine("\n\nCreating Operators...");
                Operators operatorsTable = new Operators();
                operatorsTable.Create();
                Console.WriteLine("Operators created successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nERROR: " + ex.Message);
            }

            try
            {
                Console.WriteLine("\n\nCreating Reports...");
                Reports reportsTable = new Reports();
                reportsTable.Create();
                Console.WriteLine("Reports created successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nERROR: " + ex.Message);
            }


            try
            {
                Console.WriteLine("\n\nCreating Users...");
                Users usersTable = new Users();
                usersTable.Create();
                Console.WriteLine("Users created successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nERROR: " + ex.Message);
            }
        }
    }
}
