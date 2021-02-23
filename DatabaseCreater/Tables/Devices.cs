using MySql.Data.MySqlClient;
using Shared;

namespace DatabaseCreater.Tables
{
    public class Devices
    {
        public void Create()
        {
            using (MySqlConnection localDbConnection = new MySqlConnection(Path.toDB))
            {
                localDbConnection.Open();
                MySqlCommand command = new MySqlCommand("CREATE TABLE IF NOT EXISTS `Devices`(" +
                    "`ID` INT AUTO_INCREMENT," +
                    "`Device` VARCHAR(50)," +
                    "`DeviceType` TINYINT," +
                    "`Operator` VARCHAR(50)," +
                    "`DbName` VARCHAR(15)," +
                    "`SlaveID` SMALLINT," +
                    "`Address1` INT," +
                    "`Address2` INT," +
                    "`Address3` INT," +
                    "`Address4` INT," +
                    "`Address5` INT," +
                    "`DbNameRunning` VARCHAR(15)," +
                    "`WorkingOn` VARCHAR(500)," +
                    "`WorkingOnDb` VARCHAR(15)," +
                    "`WorkingOnTime` DATETIME," +
                    "`Status` TINYINT," +
                    "PRIMARY KEY(ID));", localDbConnection);
                command.ExecuteNonQuery();
            }
        }
    }
}
