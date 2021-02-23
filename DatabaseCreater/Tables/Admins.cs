using MySql.Data.MySqlClient;
using Shared;

namespace DatabaseCreater.Tables
{
    public class Admins
    {
        public void Create()
        {
            using (MySqlConnection localDbConnection = new MySqlConnection(Path.toDB))
            {
                localDbConnection.Open();
                MySqlCommand command = new MySqlCommand("CREATE TABLE IF NOT EXISTS `Admins`(" +
                    "`ID` INT AUTO_INCREMENT," +
                    "`UserName` VARCHAR(10)," +
                    "`Password` VARCHAR(50)," +
                    "PRIMARY KEY(ID));", localDbConnection);
                command.ExecuteNonQuery();

                MySqlCommand commandAdmin = new MySqlCommand("INSERT INTO `Admins`(ID, UserName, Password) VALUES(@id, @username, @password)", localDbConnection);
                commandAdmin.Parameters.AddWithValue("@id", 1);
                commandAdmin.Parameters.AddWithValue("@username", "unity");
                commandAdmin.Parameters.AddWithValue("@password", "1453");
                commandAdmin.ExecuteNonQuery();
            }
        }
    }
}
