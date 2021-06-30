using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Json.Net;
using Newtonsoft.Json;
using TrackWorkout.Entitys;

namespace TrackWorkout.Pages.SharedClasses
{
    public class WebserviceCalls
    {
        private static HttpClient client = new HttpClient();

        public static async Task<string> RefreshCoachList(PassID ID)
        {
            var jsonContent = JsonNet.Serialize(ID);

            try
            {                
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                // API call to webservice to save user in the database
                var response = await client.PostAsync("https://webserviceswo.azurewebsites.net/GetCoachList", httpContent);
                var responseString = await response.Content.ReadAsStringAsync();
                var coachInformation = JsonConvert.DeserializeObject<CoachListRootObject>(responseString);

                if (coachInformation.CoachList.Count() > 0)
                {
                    App.StoreCoachInfo(coachInformation.CoachList);
                    return "success";
                }
                else
                {                    
                    return "There were no clients to return";
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = "Error | RefreshCoachList Webservice call: " + ex.Message.ToString();

                await LogError(ErrorMessage, "CoachDetail", jsonContent, "GetCoachList");

                return ErrorMessage;
            }
        }

        public static async Task<string> RefreshClientList(PassID ID)
        {
            var jsonContent = JsonNet.Serialize(ID);

            try
            {                
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                // API call to webservice to save user in the database
                var response = await client.PostAsync("https://webserviceswo.azurewebsites.net/GetClientData", httpContent);
                var responseString = await response.Content.ReadAsStringAsync();
                var clientInformation = JsonConvert.DeserializeObject<ClientInformationRootObject>(responseString);

                if (clientInformation.ClientInformation.Count() > 0)
                {
                    App.StoreClientInfo(clientInformation.ClientInformation);
                    return "success";
                }
                else
                {
                    return "There were no clients to return";
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = "Error | RefreshClientList Webservice call: " + ex.Message.ToString();

                await LogError(ErrorMessage, "ClientDetail", jsonContent, "GetClientData");

                return ErrorMessage;
            }
        }

        public static async Task<string> HandleClient(AcceptRejectCoach IDs)
        {
            var jsonContent = JsonNet.Serialize(IDs);

            try
            {                
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                // API call to webservice to save user in the database
                var response = await client.PostAsync("https://webserviceswo.azurewebsites.net/HandleClientRequest", httpContent);
                var responseString = await response.Content.ReadAsStringAsync();
                var clientInformation = JsonConvert.DeserializeObject<ClientInformationRootObject>(responseString);

                App.StoreClientInfo(clientInformation.ClientInformation);

                return "success";
            }
            catch (Exception ex)
            {
                string ErrorMessage = "Error | HandleClient Webservice call: " + ex.Message.ToString();

                await LogError(ErrorMessage, "ClientDetail", jsonContent, "HandleClientRequest");

                return ErrorMessage;
            }
        }

        public static async Task<string> HandleCoach(AcceptRejectCoach IDs)
        {
            var jsonContent = JsonNet.Serialize(IDs);

            try
            {                
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                // API call to webservice to save user in the database
                var response = await client.PostAsync("https://webserviceswo.azurewebsites.net/HandleCoachRequest", httpContent);
                var responseString = await response.Content.ReadAsStringAsync();
                var coachInformation = JsonConvert.DeserializeObject<CoachListRootObject>(responseString);

                string Response = App.StoreCoachInfo(coachInformation.CoachList);

                return Response;
            }
            catch (Exception ex)
            {
                string ErrorMessage = "Error | HandleCoach Webservice call: " + ex.Message.ToString();

                await LogError(ErrorMessage, "CoachDetail", jsonContent, "HandleCoachRequest");

                return ErrorMessage;
            }
        }

        public static async Task<Tuple<string, string>> AddClient(ClientRequestInfo Information)
        {
            var jsonContent = JsonNet.Serialize(Information);

            try
            {                
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                // API call to webservice to save user in the database
                var response = await client.PostAsync("https://webserviceswo.azurewebsites.net/ClientRequest", httpContent);
                var responseString = await response.Content.ReadAsStringAsync();
                var clientInformation = JsonConvert.DeserializeObject<ClientInformationRootObject>(responseString);

                if (clientInformation.ClientInformation[0].addClientSQLReturn != "Success")
                {
                    return Tuple.Create("Error", clientInformation.ClientInformation[0].addClientSQLReturn);
                }
                else
                {
                    App.StoreClientInfo(clientInformation.ClientInformation);

                    // Display Success Message 
                    return Tuple.Create("success", "Your request has been sent!");
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = "Error | AddClient Webservice call: " + ex.Message.ToString();

                await LogError(ErrorMessage, "AddClient", jsonContent, "ClientRequest");

                return Tuple.Create("Error", ErrorMessage);
            }
        }

        public static async Task<Tuple<string, string>> AddCoach(ClientRequestInfo Information)
        {
            var jsonContent = JsonNet.Serialize(Information);

            try
            {                
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                // API call to webservice to save user in the database
                var response = await client.PostAsync("https://webserviceswo.azurewebsites.net/CoachRequest", httpContent);
                var responseString = await response.Content.ReadAsStringAsync();
                var coachInformation = JsonConvert.DeserializeObject<CoachListRootObject>(responseString);

                if (coachInformation.CoachList[0].addCoachSQLReturn != "Success")
                {
                    return Tuple.Create("Error", coachInformation.CoachList[0].addCoachSQLReturn);
                }
                else
                {
                    App.StoreCoachInfo(coachInformation.CoachList);

                    return Tuple.Create("success", "Your request has been sent!");
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = "Error | AddCoach Webservice call: " + ex.Message.ToString();

                await LogError(ErrorMessage, "AddCoach", jsonContent, "CoachRequest");

                return Tuple.Create("Error", ErrorMessage);
            }
        }

        public static async Task<string> DeleteRoutine(PassID ID)
        {
            var jsonContent = JsonNet.Serialize(ID);

            try
            {                
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                // API call to webservice to save user in the database
                var response = await client.PostAsync("https://webserviceswo.azurewebsites.net/DeleteRoutine", httpContent);

                return "success";
            }
            catch (Exception ex)
            {
                string ErrorMessage = "Error | DeleteRoutine Webservice call: " + ex.Message.ToString();

                await LogError(ErrorMessage, "RoutineDetail or ClientRoutine", jsonContent, "DeleteRoutine");

                return ErrorMessage;
            }
        }

        public static async Task<string> UpdateSettings(UserInformation User)
        {
            var jsonContent = JsonNet.Serialize(User);

            try
            {
                // Use the Email object to get the rest of the data set                
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                // API call to webservice to save user in the database
                var response = await client.PostAsync("https://webserviceswo.azurewebsites.net/UpdateSettings", httpContent);
                var responseString = await response.Content.ReadAsStringAsync();
                var userInformation = JsonConvert.DeserializeObject<UserInformationRootObject>(responseString);

                //Get user info
                string Response = App.StoreUserInfo(userInformation.UserInformation);

                if (Response == "success")
                {
                    return "success";
                }
                else
                {
                    return Response;
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = "Error | UpdateSettings Webservice call: " + ex.Message.ToString();

                await LogError(ErrorMessage, "Settings", jsonContent, "UpdateSettings");

                return ErrorMessage;
            }
        }

        public static async Task<string> GetUserHistoryData(UserEmail Email)
        {
            var jsonContent = JsonNet.Serialize(Email);

            try
            {                
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                // API call to webservice to save user in the database
                var response = await client.PostAsync("https://webserviceswo.azurewebsites.net/GetUserHistoryData", httpContent);
                var responseString = await response.Content.ReadAsStringAsync();
                var historyInformation = JsonConvert.DeserializeObject<HistoryRootObject>(responseString);

                string Response = App.StoreHistoryInfo(historyInformation.HistoryInformation);

                if (Response == "success")
                {
                    return "success";
                }
                else
                {
                    return Response;
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = "Error | DeleteRoutine Webservice call: " + ex.Message.ToString();

                await LogError(ErrorMessage, "RoutineSummary", jsonContent, "GetUserHistoryData");

                return ErrorMessage;
            }
        }

        public static async Task<string> GetMasterData(UserEmail Email)
        {
            var jsonContent = JsonNet.Serialize(Email);

            try
            {                
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                // API call to webservice to save user in the database
                var WebCallResponse = await client.PostAsync("https://webserviceswo.azurewebsites.net/GetUserMasterInfo", httpContent);
                var responseString = await WebCallResponse.Content.ReadAsStringAsync();
                var userMasterInfo = JsonConvert.DeserializeObject<UserMasterInfoRootObject>(responseString);

                if (userMasterInfo.UserMasterInfo[0].UserId == 0)
                {
                    return "new user";
                }
                else
                {
                    string Response = App.HandleMasterLogin(userMasterInfo);

                    if (Response == "success")
                    {
                        return "success";
                    }
                    else
                    {
                        return Response;
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = "Error | GetMasterData Webservice call: " + ex.Message.ToString();

                await LogError(ErrorMessage, "CreateUser or MainPage", jsonContent, "GetUserMasterInfo");

                return ErrorMessage;
            }
        }

        public static async Task<Tuple<string, string>> SaveNewUser(User CreatedUser)
        {
            var jsonContent = JsonNet.Serialize(CreatedUser);

            try
            {                
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                // API call to webservice to save user in the database
                var response = await client.PostAsync("https://webserviceswo.azurewebsites.net/SaveNewUser", httpContent);
                var responseString = await response.Content.ReadAsStringAsync();

                if (responseString == "0")
                {
                    // Display Failure Message and stay on page
                    return Tuple.Create("Invalid Entry", "This email has already been used. Please use another.");
                }
                else if (responseString == "1")
                {
                    return Tuple.Create("Account Created", $"Thank you for signing up!");
                }
                else
                {
                    // Catch other errors
                    return Tuple.Create("Please contact support", $"email: tylershelton810@gmail.com and send the following message: The response string on CreateUser page is as follows: {responseString}");
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = "Error | DeleteRoutine Webservice call: " + ex.Message.ToString();

                await LogError(ErrorMessage, "CreateUser", jsonContent, "SaveNewUser");

                return Tuple.Create("Error", ErrorMessage);
            }
        }

        public static async Task<string> SaveWeighIn()
        {
            var jsonContent = JsonNet.Serialize(App.userInformationApp[0]);

            try
            {                
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                // API call to webservice to save user in the database
                var WebCallResponse = await client.PostAsync("https://webserviceswo.azurewebsites.net/SaveWeighIn", httpContent);
                var responseString = WebCallResponse.Content.ReadAsStringAsync();

                return "success";
            }
            catch (Exception ex)
            {
                string ErrorMessage = "Error | SaveWeighIn Webservice call: " + ex.Message.ToString();

                await LogError(ErrorMessage, "HomeScreenDetail", jsonContent, "SaveWeighIn");

                return ErrorMessage;
            }
        }

        public static async Task<string> SaveProfilePicture()
        {
            var jsonContent = JsonNet.Serialize(App.userInformationApp[0]);

            try
            {                
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                // API call to webservice to save user in the database
                var WebCallResponse = await client.PostAsync("https://webserviceswo.azurewebsites.net/SaveProfilePicture", httpContent);
                var responseString = WebCallResponse.Content.ReadAsStringAsync();

                return "success";
            }
            catch (Exception ex)
            {
                string ErrorMessage = "Error | SaveProfilePicture Webservice call: " + ex.Message.ToString();

                await LogError(ErrorMessage, "HomeScreenDetail", jsonContent, "SaveProfilePicture");

                return ErrorMessage;
            }
        }

        public static async Task<string> AcceptOrReject(AcceptRejectCoach information)
        {
            var jsonContent = JsonNet.Serialize(information);

            try
            {                
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                // API call to webservice to save user in the database
                var response = await client.PostAsync("https://webserviceswo.azurewebsites.net/AcceptOrRejectCoach", httpContent);
                var responseString = await response.Content.ReadAsStringAsync();
                var noteInformation = JsonConvert.DeserializeObject<NoteInformationRootObject>(responseString);

                string Response = App.StoreNoteInfo(noteInformation.NoteInformation);

                if (Response == "success")
                {
                    return "success";
                }
                else
                {
                    return Response;
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = "Error | AcceptOrReject Webservice call: " + ex.Message.ToString();

                await LogError(ErrorMessage, "NoteInformation", jsonContent, "AcceptOrRejectCoach");

                return ErrorMessage;
            }
        }

        public static async Task<string> SendNote(Entitys.NoteInformation note)
        {
            var jsonContent = JsonNet.Serialize(note);

            try
            {                
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                // API call to webservice to save user in the database
                var WebCallResponse = await client.PostAsync("https://webserviceswo.azurewebsites.net/SendNote", httpContent);
                var responseString = await WebCallResponse.Content.ReadAsStringAsync();
                var newNoteInformation = JsonConvert.DeserializeObject<NoteInformationRootObject>(responseString);

                string Response = App.StoreNoteInfo(newNoteInformation.NoteInformation);
                if (Response == "success")
                {
                    return "success";
                }
                else
                {
                    return Response;
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = "Error | SendNote Webservice call: " + ex.Message.ToString();

                await LogError(ErrorMessage, "NoteInformation", jsonContent, "SendNote");

                return ErrorMessage;
            }
        }

        public static async Task<Tuple<string, Entitys.NoteInformation>> GetNotes(PassID ID, int noteID)
        {
            var jsonContent = JsonNet.Serialize(ID);

            try
            {                
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                // API call to webservice to save user in the database
                var response = await client.PostAsync("https://webserviceswo.azurewebsites.net/GetNoteData", httpContent);
                var responseString = await response.Content.ReadAsStringAsync();
                var noteInformation = JsonConvert.DeserializeObject<NoteInformationRootObject>(responseString);

                if (noteInformation.NoteInformation.Count() > 0)
                {
                    App.StoreNoteInfo(noteInformation.NoteInformation);
                }

                Entitys.NoteInformation NoteObject = noteInformation.NoteInformation.Where(x => x.NoteID == noteID).First();

                return Tuple.Create("success", NoteObject);                
            }
            catch (Exception ex)
            {
                Entitys.NoteInformation NoteObject = new Entitys.NoteInformation();

                string ErrorMessage = "Error | SendNote Webservice call: " + ex.Message.ToString();

                await LogError(ErrorMessage, "NoteInformation", jsonContent, "GetNoteData");

                return Tuple.Create(ErrorMessage, NoteObject);
            }
        }

        public static async Task<string> CompleteWorkout(RoutineList OldRoutine)
        {
            var jsonContent = JsonNet.Serialize(OldRoutine);

            try
            {                
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                // API call to webservice to save user in the database
                var response = await client.PostAsync("https://webserviceswo.azurewebsites.net/CompleteWorkout", httpContent);
                var responseString = await response.Content.ReadAsStringAsync();
                var userInformation = JsonConvert.DeserializeObject<UserInformationRootObject>(responseString);

                //Get user info
                string Response = App.StoreUserInfo(userInformation.UserInformation);

                if (Response == "success")
                {
                    return "success";
                }
                else
                {
                    string ErrorMessage = Response;

                    await LogError(Response, "RoutineSummary", jsonContent, "CompleteWorkout");

                    return Response;
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = "Error | DeleteRoutine Webservice call: " + ex.Message.ToString();

                await LogError(ErrorMessage, "RoutineSummary", jsonContent, "CompleteWorkout");

                return ErrorMessage;
            }
        }

        public static async Task<string> LogError(string error, string page, string passedData, string webServiceCal)
        {            
            Entitys.Error logError = new Entitys.Error { Email = App.userInformationApp[0].Email, ErrorMessage = error, Page = page, PassedData = passedData, WebServiceCall = webServiceCal };

            string eee = @"Exec Report.LogError '@Email','@ErrorMessage','@Page'";
            eee = eee.Replace("@Email", logError.Email).Replace("@ErrorMessage", logError.ErrorMessage).Replace("@Page", logError.Page);

            try
            {
                var jsonContent = JsonNet.Serialize(logError);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                // API call to webservice to save user in the database
                var response = await client.PostAsync("https://webserviceswo.azurewebsites.net/LogError", httpContent);
                var responseString = await response.Content.ReadAsStringAsync();

                return "success";                
            }
            catch (Exception ex)
            {
                string ErrorMessage = "Error | LogError Webservice call: " + ex.Message.ToString();

                return ErrorMessage;
            }
        }

        public static async Task<string> GetUserRoutineList(PassID ID)
        {
            var jsonContent = JsonNet.Serialize(ID);

            try
            {                
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                // API call to webservice to save user in the database
                var response = await client.PostAsync("https://webserviceswo.azurewebsites.net/GetUserRoutineList", httpContent);
                var responseString = await response.Content.ReadAsStringAsync();
                var routineList = JsonConvert.DeserializeObject<RoutineListRootObject>(responseString);

                string Response = App.StoreRoutineInfoCoach(routineList.RoutineList);

                if (Response == "success")
                {
                    Response = App.StoreRoutineInfoUser(routineList.RoutineList);

                    if (Response == "success")
                    {
                        Response = App.StoreRoutineInfoClient(routineList.RoutineList);

                        return "success";
                    }
                    else
                    {
                        return Response;
                    }
                }
                else
                {
                    return Response;
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = "Error | GetUserRoutineList Webservice call: " + ex.Message.ToString();

                await LogError(ErrorMessage, "RoutineSummary, ClientRoutine, RoutineInProgress, or RoutineDetail", jsonContent, "GetUserRoutineList");

                return ErrorMessage;
            }
        }

        public static async Task<string> AddRoutine(List<RoutineList> BuildingRoutine)
        {
            var jsonContent = JsonNet.Serialize(BuildingRoutine);

            try
            {                
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                // API call to webservice to save user in the database
                var response = await client.PostAsync("https://webserviceswo.azurewebsites.net/AddNewRoutine", httpContent);
                var responseString = await response.Content.ReadAsStringAsync();
                var routineList = JsonConvert.DeserializeObject<RoutineListRootObject>(responseString);

                string Response = App.StoreRoutineInfoCoach(routineList.RoutineList);

                if (Response == "success")
                {
                    Response = App.StoreRoutineInfoUser(routineList.RoutineList);

                    if (Response == "success")
                    {
                        Response = App.StoreRoutineInfoClient(routineList.RoutineList);

                        if (Response == "success")
                        {
                            return "success";
                        }
                        else
                        {
                            return Response;
                        }
                    }
                    else
                    {
                        return Response;
                    }
                }
                else
                {
                    return Response;
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = "Error | DeleteRoutine Webservice call: " + ex.Message.ToString();

                await LogError(ErrorMessage, "RoutineInformationEdit", jsonContent, "AddNewRoutine");

                return ErrorMessage;
            }
        }
    }
}
