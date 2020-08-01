using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using TrackWorkout.Entitys;
using WebService.Models;

namespace WebService.Repositories
{
    public class AccountInformationRepository
    {
        public static int AddNewUserToDB(User newUser)
        {

            // connect it to our server and database
            var connectionString = "Data Source=workoutdev.database.windows.net;" +
                                    "Initial Catalog=Land;" +
                                    "User id=tshelton;" +
                                    "Password=Ty2358165;";

            // Create the query to run our stored procedure
            var query = "Exec Account.CreateNewUser '@Name','@Email','@Birthday','@WeightType','@Weight','@Source','@Password','@Photo'";

            // Replace our dummy columns with our real values from our class
            query = query.Replace("@Name", newUser.Name).Replace("@Email", newUser.Email).Replace("@Birthday", newUser.Birthday.ToString()).Replace("@WeightType", newUser.WeightType.ToString()).Replace("@Weight", newUser.Weight.ToString()).Replace("@Source", newUser.Source).Replace("@Password",newUser.Password).Replace("@Photo", newUser.Photo);

            SqlConnection connection = new SqlConnection(connectionString);

            try
            {
                int ReturnValue = 0;

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
                return 0;
            }
        }
    }
}