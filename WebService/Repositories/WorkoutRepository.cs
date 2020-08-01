using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using TrackWorkout.Entitys;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

using System.Threading.Tasks;
using Microsoft.SqlServer.Server;

namespace WebService.Repositories
{
    public class WorkoutRepository
    {
        public static DataSet AddNewRoutine(List<RoutineList> routineList)
        {

            DataTable dataSetToPass = new DataTable();
            dataSetToPass.Columns.Add("RoutineID", typeof(int));
            dataSetToPass.Columns.Add("RoutineName", typeof(string));
            dataSetToPass.Columns.Add("ExerciseDescription", typeof(string));
            dataSetToPass.Columns.Add("ExerciseNumber", typeof(int));
            dataSetToPass.Columns.Add("SetNumber", typeof(int));
            dataSetToPass.Columns.Add("Weight", typeof(string));
            dataSetToPass.Columns.Add("Reps", typeof(string));
            dataSetToPass.Columns.Add("RestTimeAfterSet", typeof(int));
            dataSetToPass.Columns.Add("Notes", typeof(string));
            dataSetToPass.Columns.Add("ProcedureType", typeof(int));
            dataSetToPass.Columns.Add("UserID", typeof(string));
            dataSetToPass.Columns.Add("CoachID", typeof(int));
            dataSetToPass.Columns.Add("Locked", typeof(bool));
            dataSetToPass.Columns.Add("RoutineDetail", typeof(string));
            dataSetToPass.Columns.Add("PRData", typeof(string));

            for (int i = 0; i < routineList.Count; i++)
            {
                dataSetToPass.Rows.Add(routineList[i].RoutineID,
                                      routineList[i].RoutineName,
                                      routineList[i].ExerciseDescription,
                                      routineList[i].ExerciseNumber,
                                      routineList[i].SetNumber,
                                      routineList[i].Weight,
                                      routineList[i].Reps,
                                      routineList[i].RestTimeAfterSet,
                                      routineList[i].Notes,
                                      routineList[i].ProcedureType,
                                      routineList[i].UserID,
                                      routineList[i].CoachID,
                                      routineList[i].Locked,
                                      routineList[i].RoutineDetail,
                                      routineList[i].PRData);
            }
                     
            // connect it to our server and database
            var connectionString = "Data Source=workoutdev.database.windows.net;" +
                                    "Initial Catalog=Land;" +
                                    "User id=tshelton;" +
                                    "Password=Ty2358165;";

            using (var conn = new SqlConnection(connectionString))
            {         
                // Setup the SQL command
                var cmd = new SqlCommand();
                cmd.CommandText = "Workout.AddRoutine";
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Connection = conn;

                //Setup the parameter
                var param = new SqlParameter("Objects", dataSetToPass);
                param.SqlDbType = System.Data.SqlDbType.Structured;
                cmd.Parameters.Add(param);

                conn.Open();

                //Execute
                DataSet ds = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds, "RoutineList");

                conn.Close();                

                return (ds);
            }  
                       
        }

        public static DataSet CompleteWorkout(RoutineList routineList)
        {

            DataTable dataSetToPass = new DataTable();
            dataSetToPass.Columns.Add("RoutineID", typeof(int));
            dataSetToPass.Columns.Add("RoutineName", typeof(string));
            dataSetToPass.Columns.Add("ExerciseDescription", typeof(string));
            dataSetToPass.Columns.Add("ExerciseNumber", typeof(int));
            dataSetToPass.Columns.Add("SetNumber", typeof(int));
            dataSetToPass.Columns.Add("Weight", typeof(string));
            dataSetToPass.Columns.Add("Reps", typeof(string));
            dataSetToPass.Columns.Add("RestTimeAfterSet", typeof(int));
            dataSetToPass.Columns.Add("Notes", typeof(string));
            dataSetToPass.Columns.Add("ProcedureType", typeof(int));
            dataSetToPass.Columns.Add("UserID", typeof(string));
            dataSetToPass.Columns.Add("CoachID", typeof(int));
            dataSetToPass.Columns.Add("Locked", typeof(bool));
            dataSetToPass.Columns.Add("RoutineDetail", typeof(string));
            dataSetToPass.Columns.Add("PRData", typeof(string));


            dataSetToPass.Rows.Add(routineList.RoutineID,
                                   routineList.RoutineName,
                                   routineList.ExerciseDescription,
                                   routineList.ExerciseNumber,
                                   routineList.SetNumber,
                                   routineList.Weight,
                                   routineList.Reps,
                                   routineList.RestTimeAfterSet,
                                   routineList.Notes,
                                   routineList.ProcedureType,
                                   routineList.UserID,
                                   routineList.CoachID,
                                   routineList.Locked,
                                   routineList.RoutineDetail,
                                   routineList.PRData);


            // connect it to our server and database
            var connectionString = "Data Source=workoutdev.database.windows.net;" +
                                    "Initial Catalog=Land;" +
                                    "User id=tshelton;" +
                                    "Password=Ty2358165;";

            using (var conn = new SqlConnection(connectionString))
            {
                // Setup the SQL command
                var cmd = new SqlCommand();
                cmd.CommandText = "Workout.CompleteWorkout";
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Connection = conn;

                //Setup the parameter
                var param = new SqlParameter("Objects", dataSetToPass);
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

        public static DataSet GetUserRoutineList(Models.UserID userID)
        {
            // connect it to our server and database
            var connectionString = "Data Source=workoutdev.database.windows.net;" +
                                    "Initial Catalog=Land;" +
                                    "User id=tshelton;" +
                                    "Password=Ty2358165;";

            // Create the query to run our stored procedure
            var query = "EXEC Workout.GetRoutines @UserID";

            query = query.Replace("@UserID", userID.ID.ToString());

            SqlConnection connection = new SqlConnection(connectionString);

            try
            {
                connection.Open();
                SqlDataAdapter Adapter = new SqlDataAdapter(query, connection);
                DataSet dataSet = new DataSet();
                Adapter.Fill(dataSet, "RoutineList");

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

        public static void DeleteRoutine(Models.UserID routineID)
        {
            // connect it to our server and database
            var connectionString = "Data Source=workoutdev.database.windows.net;" +
                                    "Initial Catalog=Land;" +
                                    "User id=tshelton;" +
                                    "Password=Ty2358165;";

            // Create the query to run our stored procedure
            var query = "EXEC Workout.DeleteRoutine @RoutineID";

            query = query.Replace("@RoutineID", routineID.ID.ToString());

            SqlConnection connection = new SqlConnection(connectionString);
            
            connection.Open();
            SqlDataAdapter Adapter = new SqlDataAdapter(query, connection);
            DataSet dataSet = new DataSet();
            Adapter.Fill(dataSet, "RoutineList");

            connection.Close();
                       
        }

        public static DataSet GetRoutineForCompare(Models.UserID userID)
        {
            // connect it to our server and database
            var connectionString = "Data Source=workoutdev.database.windows.net;" +
                                    "Initial Catalog=Land;" +
                                    "User id=tshelton;" +
                                    "Password=Ty2358165;";

            // Create the query to run our stored procedure
            var query = "EXEC Workout.GetRoutineForCompare @RoutineID";

            query = query.Replace("@RoutineID", userID.ID.ToString());

            SqlConnection connection = new SqlConnection(connectionString);

            try
            {
                connection.Open();
                SqlDataAdapter Adapter = new SqlDataAdapter(query, connection);
                DataSet dataSet = new DataSet();
                Adapter.Fill(dataSet, "RoutineList");

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
