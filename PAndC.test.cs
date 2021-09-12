using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using System;
using System.IO;
using MySql.Server;
using System.Diagnostics;

namespace Torn
{
    [TestClass]
    public class PAndCTest
    {
        private static readonly string _testDatabaseName = "ng_system";

        [TestMethod]
        public void TimeSpanTest()
        {
            //Setting up and starting the server
            //This can also be done in a AssemblyInitialize method to speed up tests
            MySqlServer dbServer = MySqlServer.Instance;
            dbServer.StartServer();

            //Create a database and select it
            MySqlHelper.ExecuteNonQuery(dbServer.GetConnectionString(), string.Format("CREATE DATABASE {0};USE {0};", _testDatabaseName));

            string tableName = "ng_game_log";

            //Create a table
            MySqlHelper.ExecuteNonQuery(dbServer.GetConnectionString(_testDatabaseName), string.Format("CREATE TABLE {0} (`Event_Type` INT NOT NULL, `Time_Logged` DATETIME NOT NULL,  `CURRENT_TIMESTAMP` DATETIME NOT NULL, PRIMARY KEY (`Event_Type`)) ENGINE = MEMORY;", tableName));

            string tableName2 = "ng_registry";
            MySqlHelper.ExecuteNonQuery(dbServer.GetConnectionString(_testDatabaseName), string.Format("CREATE TABLE {0} (`Registry_ID` INT NOT NULL, `Int_Data_1` INT NOT NULL, PRIMARY KEY (`Registry_ID`)) ENGINE = MEMORY;", tableName2));
           
            //Set Mock Current Time
            MySqlHelper.ExecuteNonQuery(dbServer.GetConnectionString(_testDatabaseName), "SET TIMESTAMP = UNIX_TIMESTAMP('2021-01-01T00:30:00')");

            //Insert data (large chunks of data can of course be loaded from a file)
            MySqlHelper.ExecuteNonQuery(dbServer.GetConnectionString(_testDatabaseName), string.Format("INSERT INTO {0} (`Registry_ID`,`Int_Data_1`) VALUES (0, 50)", tableName2));
            MySqlHelper.ExecuteNonQuery(dbServer.GetConnectionString(_testDatabaseName), string.Format("INSERT INTO {0} (`Event_Type`,`Time_Logged`) VALUES (0, '2021-01-01T00:00:00')", tableName));



            PAndC pAndCServer = new PAndC(dbServer.GetConnectionString(_testDatabaseName));

             TimeSpan gameTimeElapsed = pAndCServer.GameTimeElapsed();

             TimeSpan expected = new TimeSpan(0, 30, 0);

             Assert.AreEqual(expected, gameTimeElapsed);

            //Shutdown server
            dbServer.ShutDown();
        }
    }
}