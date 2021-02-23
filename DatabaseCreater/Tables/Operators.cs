using MySql.Data.MySqlClient;
using Shared;

namespace DatabaseCreater.Tables
{
    public class Operators
    {
        public void Create()
        {
            using (MySqlConnection localDbConnection = new MySqlConnection(Path.toDB))
            {
                localDbConnection.Open();
                MySqlCommand command = new MySqlCommand("CREATE TABLE IF NOT EXISTS `Operators`(" +
                    "`ID` INT AUTO_INCREMENT," +
                    "`Operator` VARCHAR(50)," +
                    "PRIMARY KEY(ID));", localDbConnection);
                command.ExecuteNonQuery();
            }
        }
    }
}
