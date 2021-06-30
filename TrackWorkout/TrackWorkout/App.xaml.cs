using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Threading.Tasks;
using TrackWorkout.Entitys;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Plugin.SimpleAudioPlayer;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace TrackWorkout
{
    public partial class App : Application
    {

        public static MasterDetailPage MasterDetail { get; set; }

        public static string myConnectionString;
        public static string LoadingText;

        //List to hold the users data
        public static List<ExerciseList> exerciseListApp;
        public static List<HistoryInformation> historyInformationApp;
        public static List<HistoryInformation> clientHistoryInformationApp;
        public static List<NoteInformation> NoteInformationApp;
        public static List<ClientInformation> ClientInformationApp;
        public static List<CoachList> CoachInformationApp;
        public static List<RoutineList> UserRoutineListApp;
        public static List<RoutineList> CoachAssignedRoutineListApp;
        public static List<RoutineList> ClientsRoutineListApp;
        public static List<UserInformation> userInformationApp;

        //External Login token 
        public static IdentityModel.OidcClient.LoginResult loginToken;

        //Sounds
        public static ISimpleAudioPlayer ButtonPop = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;

        //Colors
        public static Color ThemeColor3; // Gold
        public static Color ThemeColor4; // Dark Gray
        public static Color PrimaryThemeColor;
        public static Color SecondaryThemeColor;
        public static List<ColorWheel> MyPallet;

        //Fonts (iOS - These are set in the info.plist and saved in the Resources folder)
        //      (Android - )
        public static string CustomBold;
        public static string CustomRegular;
        public static string CustomLight;

        public async static Task NavigateMasterDetail(Page page)
        {
            App.MasterDetail.IsPresented = false;

            await App.MasterDetail.Detail.Navigation.PushAsync(page);
        }

        
        public App(IdentityModel.OidcClient.LoginResult loginResult = null)
        {

            InitializeComponent();
            //MainPage = new NavigationPage(new Pages.MainPage());

            //Fonts 
            switch (Device.RuntimePlatform)
            {
                //Set the fonts per platform
                case Device.iOS:
                    CustomBold = "Montserrat-Bold";
                    CustomRegular = "Montserrat-Regular";
                    CustomLight = "Montserrat-Light";
                    break;
                case Device.Android:
                    CustomBold = "Montserrat-Bold.ttf#Montserrat-Bold";
                    CustomRegular = "Montserrat-Regular.ttf#Montserrat-Regular";
                    CustomLight = "Montserrat-Light.ttf#Montserrat-Light";
                    break;
            }

            //Initialize the Authorization Client
            AuthorizationManager.Current.Init();

            //Create the Custom Color Pallet that the user can choose from.
            //The codes are stored in the database so should not be changed.
            MyPallet = new List<ColorWheel>
            {
                new ColorWheel
                {
                    Color = "Blue",
                    ColorCode = 0,
                    Hex = Color.FromHex("#5bc2dc"),
                    PrimaryOpacity = .4,
                    SecondaryOpacity = .4
                },
                new ColorWheel
                {
                    Color = "Green",
                    ColorCode = 1,
                    Hex = Color.FromHex("#5bdcb6"),
                    PrimaryOpacity = .4,
                    SecondaryOpacity = .4
                },
                new ColorWheel
                {
                    Color = "Red",
                    ColorCode = 2,
                    Hex = Color.FromHex("#dc755b"),
                    PrimaryOpacity = .4,
                    SecondaryOpacity = .4
                },
                new ColorWheel
                {
                    Color = "Purple",
                    ColorCode = 3,
                    Hex = Color.FromHex("#755bdc"),
                    PrimaryOpacity = .4,
                    SecondaryOpacity = .4
                },
                new ColorWheel
                {
                    Color = "Pink",
                    ColorCode = 4,
                    Hex = Color.FromHex("#dc5bc2"),
                    PrimaryOpacity = .4,
                    SecondaryOpacity = .4
                },
                new ColorWheel
                {
                    Color = "Lime",
                    ColorCode = 5,
                    Hex = Color.FromHex("#5bdc75"),
                    PrimaryOpacity = .4,
                    SecondaryOpacity = .4
                }
            };

            ThemeColor3 = Color.FromHex("#ffc107");
            ThemeColor4 = Color.FromHex("#393939");

            //Sounds
            try
            {
                ButtonPop.Load("PopButton.wav");
            }
            catch
            {
                //do nothing
            }

            //Navigate to the MainPage
            MainPage = new NavigationPage(new Pages.MainPage());
        }

        public static string GrabExerciseList(List<ExerciseList> exerciseList)
        {
            try
            {
                try
                {
                    exerciseListApp = exerciseList;
                }
                catch (Exception ex)
                {
                    string error = "ERROR | " + ex;
                }

                return "success";
            }
            catch (Exception ex)
            {
                return "Error | GrabExerciseList: " + ex.Message.ToString();
            }
        }

        public static string StoreUserInfo(List<UserInformation> userInformation)
        {
            try
            {
                XDocument ProfilePic = new XDocument();
                try
                {
                    ProfilePic = XDocument.Parse(userInformation[0].ProfilePicture);
                }
                catch
                {
                    // Do nothing
                }
                try
                {
                    //Take the profile picture sent in as a string and convert the image now to be used later.
                    if (ProfilePic.Element("Profile").Element("CurrentPic").Value.StartsWith("https://lh3.google"))
                    {
                        userInformation[0].ProfileImage = new UriImageSource { Uri = new Uri(ProfilePic.Element("Profile").Element("CurrentPic").Value) };
                    }
                    else if (ProfilePic.Element("Profile").Element("CurrentPic").Value == null)
                    {
                        userInformation[0].ProfileImage = null;
                    }
                }
                catch
                {
                    userInformation[0].ProfilePicture = null;
                    userInformation[0].ProfileImage = null;
                }

                switch (userInformation[0].ThemeColor)
                {
                    case 0:
                        PrimaryThemeColor = App.MyPallet[0].Hex;
                        App.MyPallet[0].PrimaryOpacity = 1;
                        break;
                    case 1:
                        PrimaryThemeColor = App.MyPallet[1].Hex;
                        App.MyPallet[1].PrimaryOpacity = 1;
                        break;
                    case 2:
                        PrimaryThemeColor = App.MyPallet[2].Hex;
                        App.MyPallet[2].PrimaryOpacity = 1;
                        break;
                    case 3:
                        PrimaryThemeColor = App.MyPallet[3].Hex;
                        App.MyPallet[3].PrimaryOpacity = 1;
                        break;
                    case 4:
                        PrimaryThemeColor = App.MyPallet[4].Hex;
                        App.MyPallet[4].PrimaryOpacity = 1;
                        break;
                    case 5:
                        PrimaryThemeColor = App.MyPallet[5].Hex;
                        App.MyPallet[5].PrimaryOpacity = 1;
                        break;
                }

                switch (userInformation[0].ThemeColor2)
                {
                    case 0:
                        SecondaryThemeColor = App.MyPallet[0].Hex;
                        App.MyPallet[0].SecondaryOpacity = 1;
                        break;
                    case 1:
                        SecondaryThemeColor = App.MyPallet[1].Hex;
                        App.MyPallet[1].SecondaryOpacity = 1;
                        break;
                    case 2:
                        SecondaryThemeColor = App.MyPallet[2].Hex;
                        App.MyPallet[2].SecondaryOpacity = 1;
                        break;
                    case 3:
                        SecondaryThemeColor = App.MyPallet[3].Hex;
                        App.MyPallet[3].SecondaryOpacity = 1;
                        break;
                    case 4:
                        SecondaryThemeColor = App.MyPallet[4].Hex;
                        App.MyPallet[4].SecondaryOpacity = 1;
                        break;
                    case 5:
                        SecondaryThemeColor = App.MyPallet[5].Hex;
                        App.MyPallet[5].SecondaryOpacity = 1;
                        break;
                }

                userInformationApp = userInformation;

                return "success";
            }
            catch (Exception ex)
            {
                return "Error | StoreUserInfo: " + ex.Message.ToString();
            }
        }

        public static string StoreHistoryInfo(List<HistoryInformation> historyInformation)
        {
            try
            {
                // Create user list and client lise
                var orderedList = historyInformation.Where(y => y.Type == "User").OrderByDescending(x => x.HistoryID).ToList();
                List<HistoryInformation> obj = new List<HistoryInformation>();
                obj = orderedList;
                historyInformationApp = obj;

                return "success";
            }
            catch (Exception ex)
            {
                return "Error | StoreHistoryInfo: " + ex.Message.ToString();
            }
        }

        public static void StoreLoginToken(IdentityModel.OidcClient.LoginResult resultFromPlatform)
        {
            // Create user list and client lise
            loginToken = resultFromPlatform;
        }

        public static string StoreClientHistoryInfo(List<HistoryInformation> historyInformation)
        {
            try
            {
                // Create user list and client lise
                var orderedList = historyInformation.Where(y => y.Type == "Client").OrderByDescending(x => x.HistoryID).ToList();
                List<HistoryInformation> obj = new List<HistoryInformation>();
                obj = orderedList;
                clientHistoryInformationApp = obj;

                return "success";
            }
            catch (Exception ex)
            {
                return "Error | StoreClientHistoryInfo: " + ex.Message.ToString();
            }
        }

        public static string StoreNoteInfo(List<NoteInformation> NoteInformation)
        {
            try
            {
                NoteInformationApp = NoteInformation;
                return "success";
            }
            catch (Exception ex)
            {
                return "Error | StoreNoteInfo: " + ex.Message.ToString();
            }
        }

        public static string StoreClientInfo(List<ClientInformation> ClientInformation)
        {
            try
            {
                for (int i = 0; i < ClientInformation.Count; i++) // Loop through List with for
                {
                    //Take the profile picture sent in as a string and convert the image now to be used later.
                    if (ClientInformation[i].ProfilePicture.StartsWith("https://lh3.googleusercontent.com"))
                    {
                        ClientInformation[i].ProfileImage = new UriImageSource { Uri = new Uri(ClientInformation[i].ProfilePicture) };
                    }
                    else
                    {
                        ClientInformation[i].ProfileImage = null;
                    }
                }

                ClientInformationApp = ClientInformation;

                return "success";
            }
            catch (Exception ex)
            {
                return "Error | StoreNoteInfo: " + ex.Message.ToString();
            }
        }

        public static string StoreCoachInfo(List<CoachList> CoachInformation)
        {
            try
            {
                for (int i = 0; i < CoachInformation.Count; i++) // Loop through List with for
                {
                    //Take the profile picture sent in as a string and convert the image now to be used later.
                    if (CoachInformation[i].ProfilePicture.StartsWith("https://lh3.googleusercontent.com"))
                    {
                        CoachInformation[i].ProfileImage = new UriImageSource { Uri = new Uri(CoachInformation[i].ProfilePicture) };
                    }
                    else
                    {
                        CoachInformation[i].ProfileImage = null;
                    }
                }

                CoachInformationApp = CoachInformation;

                return "success";
            }
            catch (Exception ex)
            {
                return "Error | StoreCoachInfo Method: " + ex.Message.ToString();
            }
        }

        public static string StoreRoutineInfoCoach(List<RoutineList> RoutineInformation)
        {
            try
            {
                CoachAssignedRoutineListApp = RoutineInformation.Where(value => value.CoachID != 0).Where(x => x.UserID == userInformationApp[0].UserId.ToString()).ToList();

                return "success";
            }
            catch (Exception ex)
            {
                return "Error | StoreRoutineInfoCoach Method: " + ex.Message.ToString();
            }
        }

        public static string StoreRoutineInfoUser(List<RoutineList> RoutineInformation)
        {
            try
            {
                UserRoutineListApp = RoutineInformation.Where(value => value.CoachID == 0).Where(x => x.UserID == userInformationApp[0].UserId.ToString()).ToList();

                return "success";
            }
            catch (Exception ex)
            {
                return "Error | StoreRoutineInfoUser Method: " + ex.Message.ToString();
            }
        }

        public static string StoreRoutineInfoClient(List<RoutineList> RoutineInformation)
        {
            try
            { 
                ClientsRoutineListApp = RoutineInformation.Where(value => value.CoachID == userInformationApp[0].UserId).ToList();

                return "success";
            }
            catch (Exception ex)
            {
                return "Error | StoreRoutineInfoClient Method: " + ex.Message.ToString();
            }
        }

        public static string HandleMasterLogin(UserMasterInfoRootObject UserMasterInfo)
        {
            // Create User List
            List<UserInformation> UserInfoToPass = new List<UserInformation>();
            foreach (var row in UserMasterInfo.UserMasterInfo.Where(x => x.TypeID == 1))
            {
                UserInformation Obj = new UserInformation
                {
                    UserId = int.Parse(row.UserId.ToString()),
                    CreateDate = DateTime.Parse(row.CreateDate.ToString()),
                    Name = row.Name,
                    Email = row.Email,
                    Birthday = DateTime.Parse(row.Birthday.ToString()),
                    Lbs = double.Parse(row.Lbs.ToString()),
                    Kg = double.Parse(row.Kg.ToString()),
                    AccountType = int.Parse(row.AccountType.ToString()),
                    PublicCode = row.PublicCode,
                    WeightType = row.WeightType,
                    PRData = row.PRData,
                    ProfilePicture = row.ProfilePicture,
                    ThemeColor = int.Parse(row.ThemeColor.ToString()),
                    ThemeColor2 = int.Parse(row.ThemeColor2.ToString()),
                    Weight = row.Weight,
                    Measurements = row.Measurements
                };

                UserInfoToPass.Add(Obj);
            }

            string Response = App.StoreUserInfo(UserInfoToPass);

            if (Response == "success")
            {
                // Create Exercise List
                List<ExerciseList> ExerciseLstToPass = new List<ExerciseList>();
                foreach (var row in UserMasterInfo.UserMasterInfo.Where(x => x.TypeID == 2))
                {
                    ExerciseList Obj = new ExerciseList
                    {
                        ExerciseID = int.Parse(row.ExerciseID.ToString()),
                        FirstLetter = char.Parse(row.FirstLetter.ToString()),
                        ExerciseDescription = row.ExerciseDescription,
                        FirstMuscleCode = int.Parse(row.FirstMuscleCode.ToString()),
                        FirstMuscleCodeDescription = row.FirstMuscleCodeDescription,
                        SecondMuscleCode = row.SecondMuscleCode,
                        SecondMuscleCodeDescription = row.SecondMuscleCodeDescription,
                        ThirdMuscleCode = row.ThirdMuscleCode,
                        ThirdMuscleCodeDescription = row.ThirdMuscleCodeDescription
                    };

                    ExerciseLstToPass.Add(Obj);
                }

                Response = App.GrabExerciseList(ExerciseLstToPass);

                if (Response == "success")
                {
                    // Create User History
                    List<HistoryInformation> UserHistoryToPass = new List<HistoryInformation>();
                    foreach (var row in UserMasterInfo.UserMasterInfo.Where(x => x.TypeID == 3))
                    {
                        HistoryInformation Obj = new HistoryInformation
                        {
                            Type = row.Type,
                            ID = int.Parse(row.ID.ToString()),
                            HistoryID = int.Parse(row.HistoryID.ToString()),
                            RoutineID = int.Parse(row.HistoryRoutineID.ToString()),
                            CompleteDate = DateTime.Parse(row.CompleteDate.ToString()),
                            Name = row.HistoryName,
                            WeightType = row.HistoryWeightType,
                            Notes = row.Notes,
                            HistoryXML = row.HistoryXML
                        };

                        UserHistoryToPass.Add(Obj);
                    }

                    Response = App.StoreHistoryInfo(UserHistoryToPass);

                    if (Response == "success")
                    {
                        Response = App.StoreClientHistoryInfo(UserHistoryToPass);

                        if (Response == "success")
                        {
                            // Create User RoutineList
                            List<RoutineList> UserRoutineListToPass = new List<RoutineList>();
                            foreach (var row in UserMasterInfo.UserMasterInfo.Where(x => x.TypeID == 4))
                            {
                                RoutineList Obj = new RoutineList
                                {
                                    RoutineID = int.Parse(row.RoutineID.ToString()),
                                    UserID = row.RoutineUserID,
                                    RoutineName = row.RoutineName,
                                    CoachID = int.Parse(row.CoachID.ToString()),
                                    CoachName = row.CoachName,
                                    Locked = bool.Parse(row.Locked.ToString()),
                                    RoutineDetail = row.RoutineDetail
                                };

                                UserRoutineListToPass.Add(Obj);
                            }

                            Response = App.StoreRoutineInfoCoach(UserRoutineListToPass);

                            if (Response == "success")
                            {
                                Response = App.StoreRoutineInfoUser(UserRoutineListToPass);

                                if (Response == "success")
                                {
                                    Response = App.StoreRoutineInfoClient(UserRoutineListToPass);

                                    if (Response == "success")
                                    {
                                        // Create Note Data
                                        List<NoteInformation> NoteDataToPass = new List<NoteInformation>();
                                        foreach (var row in UserMasterInfo.UserMasterInfo.Where(x => x.TypeID == 5))
                                        {
                                            NoteInformation Obj = new NoteInformation
                                            {
                                                FromUser = row.FromUser,
                                                FromEmail = row.FromEmail,
                                                ToUser = row.ToUser,
                                                ToEmail = row.ToEmail,
                                                NoteID = int.Parse(row.NoteID.ToString()),
                                                Note = row.Note,
                                                NoteSentDate = DateTime.Parse(row.NoteSentDate.ToString()),
                                                NoteType = row.NoteType,
                                                FromID = row.FromID,
                                                ToID = row.ToID,
                                                Subject = row.Subject
                                            };

                                            NoteDataToPass.Add(Obj);
                                        }

                                        Response = App.StoreNoteInfo(NoteDataToPass);

                                        if (Response == "success")
                                        {
                                            // Client Data
                                            List<ClientInformation> ClientDataToPass = new List<ClientInformation>();
                                            foreach (var row in UserMasterInfo.UserMasterInfo.Where(x => x.TypeID == 6))
                                            {
                                                ClientInformation Obj = new ClientInformation
                                                {
                                                    ClientName = row.ClientName,
                                                    ClientEmail = row.ClientEmail,
                                                    ClientWeight = double.Parse(row.ClientWeight.ToString()),
                                                    WeightType = row.ClientWeightType,
                                                    Status = row.Status,
                                                    ClientBirthday = DateTime.Parse(row.ClientBirthday.ToString()),
                                                    BecameClientDate = DateTime.Parse(row.BecameClientDate.ToString()),
                                                    ClientID = row.ClientID,
                                                    CoachID = int.Parse(row.ClientCoachID.ToString()),
                                                    ProfilePicture = row.ClientProfilePicture,
                                                    ClientWeightHistory = row.ClientWeightHistory,
                                                    ClientMeasurementHistory = row.ClientMeasurementHistory,
                                                    ClientPRData = row.ClientPRData,
                                                    OrderID = int.Parse(row.ClientOrderID.ToString())
                                                };

                                                ClientDataToPass.Add(Obj);
                                            }

                                            Response = App.StoreClientInfo(ClientDataToPass);

                                            if (Response == "success")
                                            {
                                                // Coach List Data
                                                List<CoachList> CoachDataToPass = new List<CoachList>();
                                                foreach (var row in UserMasterInfo.UserMasterInfo.Where(x => x.TypeID == 7))
                                                {
                                                    CoachList Obj = new CoachList
                                                    {
                                                        Name = row.CoachListName,
                                                        CoachID = int.Parse(row.CoachListCoachID.ToString()),
                                                        ClientID = int.Parse(row.CoachListClientID.ToString()),
                                                        Email = row.CoachListEmail,
                                                        RelationshipCreateDate = DateTime.Parse(row.RelationshipCreateDate.ToString()),
                                                        Status = row.CoachListStatus,
                                                        StatusType = int.Parse(row.StatusType.ToString()),
                                                        ProfilePicture = row.CoachListProfilePicture,
                                                        OrderID = int.Parse(row.CoachOrderID.ToString())
                                                    };

                                                    CoachDataToPass.Add(Obj);
                                                }

                                                return Response = App.StoreCoachInfo(CoachDataToPass);
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

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
