using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.VisualBasic;

public static class SQLCommands {

    public static SqliteConnection Connect() {
        //create an sql connection and pass it to the parent
        var connection = new SqliteConnection("Data Source=~/Documents/Github/Household-Chat-Client/database.db");
        return connection;
    }

    public static void Message(SqliteConnection connection, string sender, string receiver, string message, DateTime time) {
        var command = connection.CreateCommand();
        command.CommandText = 
        $"insert into Message (\"sender\", \"message\", \"receiver\", \"time\") values (\"{sender}\", \"{message}\", \"{receiver}\", \"{time}\");";
        command.ExecuteNonQuery();
    }

    public static void Close(SqliteConnection connection) {
        connection.Close();
    }

    public static bool CreateUser(SqliteConnection connection, string username, string password) {
        var command = connection.CreateCommand();
        command.CommandText =
        $"select * from User where username = \"{username}\";";
        var result = command.ExecuteReader();

        //Check if there is already a user with that name, if so, return false so that the server knows
        if(result.HasRows) {
            return false;
        }

        command = connection.CreateCommand();
        command.CommandText =
        $"insert into User (\"{username}\", \"{password}\");";
        command.ExecuteNonQuery();
        return true;
    }

    public static bool LogIn(SqliteConnection connection, string username, string password) {
        //Check if password and username are correct, if not, tell the server
        var command = connection.CreateCommand();
        command.CommandText =
        $"SELECT * FROM User where (\"username\", \"password\") = (\"{username}\" ,\"{password}\");";
        var result = command.ExecuteReader();
        if(result.HasRows) {
            return true;
        }

        return false;
    }

    public static 

}