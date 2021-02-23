using MySql.Data.MySqlClient;
using Shared;

namespace DatabaseCreater.Tables
{
    public class Reports
    {
        public void Create()
        {
            using (MySqlConnection localDbConnection = new MySqlConnection(Path.toDB))
            {
                localDbConnection.Open();
                MySqlCommand command = new MySqlCommand("CREATE TABLE IF NOT EXISTS `Reports`(" +
                    "`ID` INT AUTO_INCREMENT," +
                    "`Date` DATETIME," +
                    "PRIMARY KEY(ID));", localDbConnection);
                command.ExecuteNonQuery();
            }
        }
    }
}
