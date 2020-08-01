using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using TrackWorkout.Entitys;

namespace WebService.Repositories
{
    public class GetUserIDRepository
    {
        public static int GetUserIDFromDB(UserEmail userEmail)
        {

            // connect it to our server and database
            var connectionString = "Data Source=workoutdev.database.windows.net;" +
                                    "Initial Catalog=Land;" +
                                    "User id=tshelton;" +
                                    "Password=Ty2358165;";

            // Create the query to run our stored procedure
            var query = "Exec Account.GetUserID '@Email'";

            // Replace our dummy columns with our real values from our class
            query = query.Replace("@Email", userEmail.Email);

            SqlConnection connection = new SqlConnection(connectionString);

            try
            {
                int ReturnValue = -1;

                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader DataReader = command.ExecuteReader();

                while (DataReader.Read())

                    ReturnValue = DataReader.GetInt32(0);


                DataReader.Close();
                command.Dispose();
                connection.Close();
                return ReturnValue;
            }
            catch (Exception)
            {
                //throw
                return -1;
            }
        }
    }
}