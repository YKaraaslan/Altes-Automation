using MySql.Data.MySqlClient;
using Shared;

namespace DatabaseCreater.Tables
{
    public class Confirmation
    {
        public void Create()
        {
            using (MySqlConnection localDbConnection = new MySqlConnection(Path.toDB))
            {
                localDbConnection.Open();
                MySqlCommand command = new MySqlCommand("CREATE TABLE IF NOT EXISTS `Confirmation`(" +
                    "`Password` VARCHAR(50))", localDbConnection);
                command.ExecuteNonQuery();

                MySqlCommand commandAdmin = new MySqlCommand("INSERT INTO `Confirmation`(Password) VALUES(@password)", localDbConnection);
                commandAdmin.Parameters.AddWithValue("@password", "altestrade");
                commandAdmin.ExecuteNonQuery();
            }
        }
    }
}
