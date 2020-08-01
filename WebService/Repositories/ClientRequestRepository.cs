using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using WebService.Models;
using TrackWorkout.Entitys;

namespace WebService.Repositories
{
    public class ClientRequestRepository
    {
        public static int ClientRequest(ClientRequestInfo clientRequest)
        {

            // connect it to our server and database
            var connectionString = "Data Source=workoutdev.database.windows.net;" +
                                    "Initial Catalog=Land;" +
                                    "User id=tshelton;" +
                                    "Password=Ty2358165;";

            // Create the query to run our stored procedure
            var query = "Exec Relationship.SendClientRequest '@CoachID', '@PublicCode'";

            // Replace our dummy columns with our real values from our class
            query = query.Replace("@PublicCode", clientRequest.PublicCode).Replace("@CoachID", clientRequest.CoachID.ToString());

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

        public static DataSet AcceptOrRejectCoach(AcceptOrRejectCoach acceptOrRejectCoach)
        {

            // connect it to our server and database
            var connectionString = "Data Source=workoutdev.database.windows.net;" +
                                    "Initial Catalog=Land;" +
                                    "User id=tshelton;" +
                                    "Password=Ty2358165;";

            // Create the query to run our stored procedure
            var query = "Exec Relationship.AcceptOrDeclineCoach '@NoteID','@CoachID','@UserID','@AcceptOrReject'";

            // Replace our dummy columns with our real values from our class
            query = query.Replace("@NoteID", acceptOrRejectCoach.NoteID).Replace("@UserID", acceptOrRejectCoach.UserID).Replace("@AcceptOrReject", acceptOrRejectCoach.AcceptOrReject).Replace("@CoachID", acceptOrRejectCoach.CoachID);

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

        public static DataSet SendNote(NoteInformation note)
        {

            // connect it to our server and database
            var connectionString = "Data Source=workoutdev.database.windows.net;" +
                                    "Initial Catalog=Land;" +
                                    "User id=tshelton;" +
                                    "Password=Ty2358165;";

            // Create the query to run our stored procedure
            var query = "Exec Relationship.SendNote '@UserID','@ClientID','@Note','@NoteType','@NoteID'";

            // Replace our dummy columns with our real values from our class
            query = query.Replace("@UserID", note.FromID).Replace("@ClientID", note.ToID).Replace("@NoteType", note.NoteType).Replace("@NoteID", note.NoteID.ToString()).Replace("@Note", note.Note);

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
    }
}