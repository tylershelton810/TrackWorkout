using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using TrackWorkout.Entitys;

namespace WebService.Repositories
{
    public class GetUserDataRepository
    {
        

        // USER
        public static DataSet GetUserData(UserEmail email)
        {

            // connect it to our server and database
            var connectionString = "Data Source=workoutdev.database.windows.net;" +
                                    "Initial Catalog=Land;" +
                                    "User id=tshelton;" +
                                    "Password=Ty2358165;";

            // Create the query to run our stored procedure
            var query = "EXEC Account.GetUserData '@Email', '@Password', '@PicFromService'";

            // Replace our dummy columns with our real values from our class
            query = query.Replace("@Email", email.Email);
            query = query.Replace("@Password", email.Password);
            query = query.Replace("@PicFromService", email.Photo);

            SqlConnection connection = new SqlConnection(connectionString);

            try
            {
                connection.Open();
                SqlDataAdapter Adapter = new SqlDataAdapter(query, connection);
                DataSet dataSet = new DataSet();
                Adapter.Fill(dataSet, "UserInformation");

                connection.Close();
                return (dataSet);
            }
            catch (Exception)
            {
                DataSet fakeDataSet = new DataSet();
                //throw
                return fakeDataSet;
            }
        }

        // SETTINGS
        public static DataSet UpdateUserSettings(UserInformation updatedUserInfo)
        {
            DataTable dataSetToPass = new DataTable();
            dataSetToPass.Columns.Add("UserId", typeof(int));
            //dataSetToPass.Columns.Add("CreateDate", typeof(string));
            dataSetToPass.Columns.Add("Name", typeof(string));
            dataSetToPass.Columns.Add("Email", typeof(string));
            //dataSetToPass.Columns.Add("Birthday", typeof(string));
            dataSetToPass.Columns.Add("Lbs", typeof(double));
            dataSetToPass.Columns.Add("Kg", typeof(double));
            dataSetToPass.Columns.Add("AccountType", typeof(int));
            dataSetToPass.Columns.Add("PublicCode", typeof(string));
            dataSetToPass.Columns.Add("WeightType", typeof(string));
            dataSetToPass.Columns.Add("PRData", typeof(string));

            
            dataSetToPass.Rows.Add(updatedUserInfo.UserId,
                                   //updatedUserInfo.CreateDate,
                                   updatedUserInfo.Name,
                                   updatedUserInfo.Email,
                                   //updatedUserInfo.Birthday,
                                   updatedUserInfo.Lbs,
                                   updatedUserInfo.Kg,
                                   updatedUserInfo.AccountType,
                                   updatedUserInfo.PublicCode,
                                   updatedUserInfo.WeightType,
                                   updatedUserInfo.PRData);
            

            // connect it to our server and database
            var connectionString = "Data Source=workoutdev.database.windows.net;" +
                                    "Initial Catalog=Land;" +
                                    "User id=tshelton;" +
                                    "Password=Ty2358165;";

            using (var conn = new SqlConnection(connectionString))
            {
                // Setup the SQL command
                var cmd = new SqlCommand();
                cmd.CommandText = "Account.UpdateSettings";
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Connection = conn;

                //Setup the parameter
                var param = new SqlParameter("UserData", dataSetToPass);
                param.SqlDbType = System.Data.SqlDbType.Structured;
                cmd.Parameters.Add(param);

                conn.Open();

                //Execute
                DataSet ds = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds, "UserInformation");

                conn.Close();

                return (ds);
            }
        }

        // History
        public static DataSet GetUserHistoryData(UserEmail email)
        {

            // connect it to our server and database
            var connectionString = "Data Source=workoutdev.database.windows.net;" +
                                    "Initial Catalog=Land;" +
                                    "User id=tshelton;" +
                                    "Password=Ty2358165;";

            // Create the query to run our stored procedure            
            SqlConnection connection = new SqlConnection(connectionString);

            // Create the query to run our stored procedure
            var query = "EXEC Workout.GetHistoryData '@Email'";

            query = query.Replace("@Email", email.Email);
            try
            {
                connection.Open();
                SqlDataAdapter Adapter = new SqlDataAdapter(query, connection);
                DataSet dataSet = new DataSet();
                Adapter.Fill(dataSet, "HistoryInformation");

                connection.Close();
                return (dataSet);
            }
            catch (Exception)
            {
                DataSet fakeDataSet = new DataSet();
                //throw
                return fakeDataSet;
            }
        }

        //NOTES
        public static DataSet GetNoteData(WebService.Models.UserID userID)
        {

            // connect it to our server and database
            var connectionString = "Data Source=workoutdev.database.windows.net;" +
                                    "Initial Catalog=Land;" +
                                    "User id=tshelton;" +
                                    "Password=Ty2358165;";

            // Create the query to run our stored procedure
            var query = "EXEC Account.GetNotes '@UserID'";

            // Replace our dummy columns with our real values from our class
            query = query.Replace("@UserID", userID.ID.ToString());

            SqlConnection connection = new SqlConnection(connectionString);

            try
            {
                connection.Open();
                SqlDataAdapter Adapter = new SqlDataAdapter(query, connection);
                DataSet dataSet = new DataSet();
                Adapter.Fill(dataSet, "NoteInformation");

                connection.Close();
                return (dataSet);
            }
            catch (Exception)
            {
                DataSet fakeDataSet = new DataSet();
                //throw
                return fakeDataSet;
            }
        }

        public static DataSet GetCoachList(WebService.Models.UserID userID)
        {

            // connect it to our server and database
            var connectionString = "Data Source=workoutdev.database.windows.net;" +
                                    "Initial Catalog=Land;" +
                                    "User id=tshelton;" +
                                    "Password=Ty2358165;";

            // Create the query to run our stored procedure
            var query = "EXEC Relationship.GetCoaches '@UserID'";

            // Replace our dummy columns with our real values from our class
            query = query.Replace("@UserID", userID.ID.ToString());

            SqlConnection connection = new SqlConnection(connectionString);

            try
            {
                connection.Open();
                SqlDataAdapter Adapter = new SqlDataAdapter(query, connection);
                DataSet dataSet = new DataSet();
                Adapter.Fill(dataSet, "CoachList");

                connection.Close();
                return (dataSet);
            }
            catch (Exception)
            {
                DataSet fakeDataSet = new DataSet();
                //throw
                return fakeDataSet;
            }
        }

        //CLIENTS
        public static DataSet GetClientData(WebService.Models.UserID userID)
        {

            // connect it to our server and database
            var connectionString = "Data Source=workoutdev.database.windows.net;" +
                                    "Initial Catalog=Land;" +
                                    "User id=tshelton;" +
                                    "Password=Ty2358165;";

            // Create the query to run our stored procedure
            var query = "EXEC Relationship.GetClients '@UserID'";

            // Replace our dummy columns with our real values from our class
            query = query.Replace("@UserID", userID.ID.ToString());

            SqlConnection connection = new SqlConnection(connectionString);

            try
            {
                connection.Open();
                SqlDataAdapter Adapter = new SqlDataAdapter(query, connection);
                DataSet dataSet = new DataSet();
                Adapter.Fill(dataSet, "ClientInformation");

                connection.Close();
                return (dataSet);
            }
            catch (Exception)
            {
                DataSet fakeDataSet = new DataSet();
                //throw
                return fakeDataSet;
            }
        }
        //EXERCISE LIST
        public static DataSet GetExerciseList()
        {

            // connect it to our server and database
            var connectionString = "Data Source=workoutdev.database.windows.net;" +
                                    "Initial Catalog=Land;" +
                                    "User id=tshelton;" +
                                    "Password=Ty2358165;";

            // Create the query to run our stored procedure
            var query = "EXEC Workout.LoadExerciseList";

            SqlConnection connection = new SqlConnection(connectionString);

            try
            {
                connection.Open();
                SqlDataAdapter Adapter = new SqlDataAdapter(query, connection);
                DataSet dataSet = new DataSet();
                Adapter.Fill(dataSet, "ExerciseList");

                connection.Close();
                return (dataSet);
            }
            catch (Exception)
            {
                DataSet fakeDataSet = new DataSet();
                //throw
                return fakeDataSet;
            }
        }
    }
}