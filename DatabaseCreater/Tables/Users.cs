using MySql.Data.MySqlClient;
using Shared;

namespace DatabaseCreater.Tables
{
    public class Users
    {
        public void Create()
        {
            using (MySqlConnection localDbConnection = new MySqlConnection(Path.toDB))
            {
                localDbConnection.Open();
                MySqlCommand command = new MySqlCommand("CREATE TABLE IF NOT EXISTS `Users`(" +
                    "`ID` INT AUTO_INCREMENT," +
                    "`NameSurname` VARCHAR(50)," +
                    "`UserName` VARCHAR(30)," +
                    "`Password` VARCHAR(30)," +
                    "`Mail` VARCHAR(30)," +
                    "`Phone` VARCHAR(20)," +
                    "PRIMARY KEY(ID));", localDbConnection);
                command.ExecuteNonQuery();
            }
        }
    }
}
