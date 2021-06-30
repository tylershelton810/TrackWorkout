using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Xml.Linq;
using Rg.Plugins.Popup.Extensions;

namespace TrackWorkout.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NoteInformation : ContentPage
    {
        string coachNameIDStore;
        string noteIDStore;
        string coachIDStore;
        Entitys.NoteInformation noteinfo;
        string PassedBy;
        Entitys.ClientInformation Client;
        Entitys.CoachList Coach;

        public NoteInformation(Entitys.NoteInformation noteInformation, string PassBy)
        {
            InitializeComponent();

            FillerBox.BackgroundColor = App.PrimaryThemeColor;
            FillerBox2.BackgroundColor = App.PrimaryThemeColor;
            ButtonGridBottom.BackgroundColor = App.PrimaryThemeColor;
            MainFrame.BackgroundColor = App.PrimaryThemeColor;
            NoteTitle.FontFamily = App.CustomBold;
            Notes.FontFamily = App.CustomRegular;
            RefreshingLayout.RefreshColor = App.ThemeColor3;

            if (PassBy == "Client")
            {
                if (App.userInformationApp[0].Email == noteInformation.ToEmail)
                {
                    Client = App.ClientInformationApp.Where(X => X.ClientEmail == noteInformation.FromEmail).FirstOrDefault();
                }
                else
                {
                    Client = App.ClientInformationApp.Where(X => X.ClientEmail == noteInformation.ToEmail).FirstOrDefault();
                }
                Coach = App.CoachInformationApp.Where(x => x.Email == App.userInformationApp[0].Email).FirstOrDefault();
            }
            else
            {
                if (App.userInformationApp[0].Email == noteInformation.ToEmail)
                {
                    Coach = App.CoachInformationApp.Where(x => x.Email == noteInformation.FromEmail).FirstOrDefault();
                }
                else
                {
                    Coach = App.CoachInformationApp.Where(x => x.Email == noteInformation.ToEmail).FirstOrDefault();
                }
                Client = App.ClientInformationApp.Where(X => X.ClientEmail == App.userInformationApp[0].Email).FirstOrDefault();
            }
            NavigationPage.SetHasNavigationBar(this, false);

            //Create UI elements
            Label HeaderLabel = new Label
            {
                Text = PassBy,
                FontFamily = App.CustomBold,
                FontSize = 15,
                FontAttributes = FontAttributes.Bold,
                HeightRequest = 20,
                Margin = new Thickness(25, 0, 0, 15),
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.End,
                TextColor = Color.White
            };

            // Create Frame
            Frame HeaderFrame = new Frame
            {
                HasShadow = true,
                Padding = 0,
            };


            BackButton.Clicked += (o, e) =>
            {
                Navigation.PopAsync();
            };

            PassedBy = PassBy;

            ImageButton AcceptButton = new ImageButton
            {
                BackgroundColor = Color.Transparent,
                Source = "Check.png"
            };

            ImageButton TrashButton = new ImageButton
            {
                BackgroundColor = Color.Transparent,
                Source = "Trash.png"
            };

            ImageButton SendButton = new ImageButton
            {
                BackgroundColor = Color.Transparent,
                Source = "SendNote.png"
            };

            switch (noteInformation.NoteType)
            {
                case "Workout Completed":
                    ProfilePicture.Source = "Routine.png";
                    NoteFrame.IsVisible = false;
                    Notes.IsEnabled = false;
                    MainFrame.Content = TrashButton;
                    TrashButton.BackgroundColor = App.SecondaryThemeColor;
                    TrashButton.CornerRadius = 27;
                    break;
                case "Conversation":
                    ProfilePicture.Source = "Note.png";
                    MainFrame.Content = SendButton;
                    SendButton.BackgroundColor = App.SecondaryThemeColor;
                    SendButton.CornerRadius = 27;
                    break;
                case "Coach Request":
                    ProfilePicture.Source = "Coach.png";
                    NoteFrame.IsVisible = false;
                    Notes.IsEnabled = false;
                    MainFrame.Content = AcceptButton;
                    AcceptButton.BackgroundColor = App.SecondaryThemeColor;
                    AcceptButton.CornerRadius = 27;
                    TrashButton.BackgroundColor = Color.Transparent;
                    TrashButton.VerticalOptions = LayoutOptions.End;
                    TrashButton.HorizontalOptions = LayoutOptions.Center;
                    ButtonRow.Children.Add(TrashButton, 4, 1);
                    break;
            }
            //This is a reply to a note
            if (noteInformation.NoteType == "Conversation" && noteInformation.NoteID != -1)
            {
                TrashButton.BackgroundColor = Color.Transparent;
                TrashButton.VerticalOptions = LayoutOptions.End;
                TrashButton.HorizontalOptions = LayoutOptions.Center;
                //Add button to grid column 4 row 1
                ButtonRow.Children.Add(TrashButton, 4, 1);
            }

            noteinfo = noteInformation;

            if (noteInformation.NoteType == "Conversation")
            {
                NoteTitle.Text = noteInformation.Subject;                                               
            }
            else if (noteInformation.NoteType == "Workout Completed" || noteInformation.NoteType == "Coach Request")
            {
                NoteTitle.Text = noteInformation.NoteType;
            }


            coachNameIDStore = noteInformation.FromUser;
            XDocument Message = new XDocument();

            try
            {
                Message = XDocument.Parse(noteInformation.Note);
            }
            catch(Exception ex)
            {
                string error = ex.ToString();
                // Do Nothing. This happens when a new note is being created.
            }
            noteIDStore = noteInformation.NoteID.ToString();
            coachIDStore = noteInformation.FromID;

            //Chain together all notes that have been replied to. 
            if (noteInformation.NoteType == "Conversation" && noteInformation.NoteID != -1)
            {
                int count = 0;
                foreach (XElement Node in Message.Element("Note").Elements("Message"))
                {
                    Color BackColorToUse = App.SecondaryThemeColor;
                    Thickness marginToUse = new Thickness(10, 5, 60, 5);
                    //int gridColToUse = 0;
                    if (Node.Element("From").Value.Trim(' ') == noteInformation.ToUser.Trim(' '))
                    {
                        BackColorToUse = App.ThemeColor3;
                        marginToUse = new Thickness(60, 5, 10, 5);
                        //gridColToUse = 1;

                    }
                    Frame LabelFrame = new Frame
                    {
                        VerticalOptions = LayoutOptions.Start,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        CornerRadius = 10,
                        Padding = 10,
                        HasShadow = false,
                        BackgroundColor = BackColorToUse,
                        Margin = marginToUse
                    };
                    Label MessageLabel = new Label
                    {
                        VerticalOptions = LayoutOptions.Start,
                        FontFamily = App.CustomRegular,
                        HorizontalOptions = LayoutOptions.Start,
                        TextColor = App.ThemeColor4,
                        FontSize = 12,
                        BackgroundColor = BackColorToUse,
                        Text = Node.Element("Text").Value
                    };
                    LabelFrame.Content = MessageLabel;

                    gridStack.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto});                   

                    gridStack.Children.Add(LabelFrame, 0, count);
                    Grid.SetColumnSpan(LabelFrame, 2);


                    count++;
                }
            }
            else if (noteInformation.NoteType == "Conversation" && noteInformation.NoteID == -1)
            {
                // Do not add anything. this is a new note.
            }
            else
            {
                Label MessageLabel = new Label
                {
                    VerticalOptions = LayoutOptions.Start,
                    HorizontalOptions = LayoutOptions.Start,
                    FontFamily = App.CustomRegular,
                    TextColor = App.ThemeColor4,
                    Margin = new Thickness(5, 15, 5, 5),
                    FontSize = 12,
                    Text = Message.Element("Note").Element("Message").Element("Text").Value
                };
                gridStack.RowDefinitions.Add(new RowDefinition());

                gridStack.Children.Add(MessageLabel,0,0);
                Grid.SetColumnSpan(MessageLabel, 2);
            }

            AcceptButton.Clicked += (o, e) =>
            {
                //Entitys.AcceptRejectCoach executeProc = new Entitys.AcceptRejectCoach { NoteID = noteIDStore, CoachID = coachIDStore, UserID = App.userInformationApp[0].UserId.ToString(), AcceptOrReject = "2" };
                //ExecuteQuery(executeProc);
            };

            TrashButton.Clicked += (o, e) =>
            {
                DeclineClick(o, e);
            };

            SendButton.Clicked += async (o, e) =>
            {
                if (Notes.Text != "" && Notes.Text != null)
                {
                    if (Message.ToString() == "")
                    {
                        //This is a new note.
                        Message = new XDocument(
                                       new XElement("Note",
                                            new XElement("Message",
                                                new XElement("Text", Notes.Text),
                                                new XElement("ID", "1"),
                                                new XElement("From", App.userInformationApp[0].Name)
                                                        ) //End message
                                                    ) //End Note
                                                ); //End Document
                    }
                    else
                    {
                        int MaxID = 0;

                        //Loop through the XML to find the highest id
                        foreach (XElement ID in Message.Element("Note").Descendants("ID"))
                        {
                            int CheckID = int.Parse(ID.Value);
                            if (MaxID < CheckID)
                            {
                                MaxID = CheckID;
                            }
                        }

                        //Add one to increment to the next id
                        MaxID++;

                        //Add the new Message Node to the XML
                        Message.Element("Note").Add(new XElement("Message",
                                                        new XElement("Text", Notes.Text),
                                                        new XElement("ID", MaxID.ToString()),
                                                        new XElement("From", App.userInformationApp[0].Name)
                                                                ) //End Message
                                                    ); //End Add
                    }
                    noteInformation.Note = Message.ToString();

                    noteInformation.FromID = App.userInformationApp[0].UserId.ToString();
                    //This will be used to return the correct data
                    if (PassBy == "Client")
                    {
                        noteInformation.ToID = Client.ClientID.ToString();                        
                    }
                    else if (PassBy == "Coach")
                    {
                        noteInformation.ToID = Coach.CoachID.ToString();                        
                    }
                    
                    SendCall(noteInformation);
                }
                else
                {
                    var Pop = new Pages.PopUps.ErrorMessage("No Response", "There was not a note to send.", "OK");
                    await App.Current.MainPage.Navigation.PushPopupAsync(Pop, true);
                }
            };

            //yScroll();
        }

        //private async void yScroll()
        //{
        //    double yPositionScroll = 45 * gridStack.Children.Count;
        //    await NoteContent.ScrollToAsync(0, 1500, false);
        //}

        private void WhileRefreshing(object sender, EventArgs e)
        {
            RefreshClientData();
        }

        private async void RefreshClientData()
        {
            Entitys.PassID passNoteRequest = new Entitys.PassID { ID = App.userInformationApp[0].UserId.ToString() };

            (string Response, Entitys.NoteInformation NoteObject) = await SharedClasses.WebserviceCalls.GetNotes(passNoteRequest, noteinfo.NoteID);

            if (Response == "success")
            {
                RefreshingLayout.IsRefreshing = false;
                Navigation.InsertPageBefore(new NoteInformation(NoteObject, PassedBy), this); await Navigation.PopAsync(); // added await?
            }
            else
            {
                var Pop = new Pages.PopUps.ErrorMessage("Error", Response, "OK");
                await App.Current.MainPage.Navigation.PushPopupAsync(Pop, true);
            }            
        }

        private async void DeclineClick(object sender, EventArgs e)
        {
            if (noteinfo.NoteType == "Coach Request")
            {
                var QuestionPop = new Pages.PopUps.UserChoiceMessage("Confirm", "Are you sure you deny this coach request?", "Yes", "No");
                var answer = await QuestionPop.Show();
                if (answer) // Yes
                {
                    Entitys.AcceptRejectCoach executeProc = new Entitys.AcceptRejectCoach { NoteID = noteIDStore, CoachID = coachIDStore, UserID = App.userInformationApp[0].UserId.ToString(), AcceptOrReject = "3" };
                    ExecuteQuery(executeProc);
                }
            }

            if (noteinfo.NoteType == "Workout Completed")
            {
                var QuestionPop = new Pages.PopUps.UserChoiceMessage("Confirm", "Are you sure you want to delete this note?", "Yes", "No");
                var answer = await QuestionPop.Show();
                if (answer) // Yes
                {
                    Entitys.AcceptRejectCoach executeProc = new Entitys.AcceptRejectCoach { NoteID = noteIDStore, CoachID = coachIDStore, UserID = App.userInformationApp[0].UserId.ToString(), AcceptOrReject = "5" };
                    ExecuteQuery(executeProc);
                }                
            }

            if (noteinfo.NoteType == "Conversation")
            {
                var QuestionPop = new Pages.PopUps.UserChoiceMessage("Confirm", "Are you sure you want to delete this conversation?", "Yes", "No");
                var answer = await QuestionPop.Show();
                if (answer) // Yes
                {
                    Entitys.AcceptRejectCoach executeProc = new Entitys.AcceptRejectCoach { NoteID = noteIDStore, CoachID = coachIDStore, UserID = App.userInformationApp[0].UserId.ToString(), AcceptOrReject = "6" };
                    ExecuteQuery(executeProc);
                }
            }
        }

        private async void ExecuteQuery(Entitys.AcceptRejectCoach executeProc)
        {
            var LoadingPop = new Pages.PopUps.Loading("", false);
            await App.Current.MainPage.Navigation.PushPopupAsync(LoadingPop, true);

            string Response = await SharedClasses.WebserviceCalls.AcceptOrReject(executeProc);

            await App.Current.MainPage.Navigation.PopPopupAsync();

            if (Response == "succcess")
            {
                if (executeProc.AcceptOrReject == "2")
                {
                    var Pop = new Pages.PopUps.InformationMessage("You have a new coach!", $"You have accepted {coachNameIDStore} as your coach!", "OK");
                    await App.Current.MainPage.Navigation.PushPopupAsync(Pop, true);
                    foreach (var Coach in App.CoachInformationApp)
                    {
                        if (Coach.CoachID.ToString() == executeProc.CoachID)
                        {
                            Coach.Status = "Active";
                            Coach.StatusType = 2;
                        }
                    }
                }

                if (executeProc.AcceptOrReject == "3")
                {
                    var Pop = new Pages.PopUps.InformationMessage("You have declined.", $"You have declined {coachNameIDStore}. Bummer for them.", "OK");
                    await App.Current.MainPage.Navigation.PushPopupAsync(Pop, true);
                }

                if (executeProc.AcceptOrReject == "5")
                {
                    var Pop = new Pages.PopUps.InformationMessage("Note Deleted", $"You have deleted {noteinfo.FromUser}'s Note.", "OK");
                    await App.Current.MainPage.Navigation.PushPopupAsync(Pop, true);
                }

                if (executeProc.AcceptOrReject == "6")
                {
                    var Pop = new Pages.PopUps.InformationMessage("Note Deleted", $"You have deleted the conversation.", "OK");
                    await App.Current.MainPage.Navigation.PushPopupAsync(Pop, true);
                }

                if (PassedBy == "Client")
                {
                    await Navigation.PushAsync(new Pages.Client.ClientNote(Client));
                }
                else if (PassedBy == "Coach")
                {
                    await Navigation.PushAsync(new Pages.Coach.CoachNote(Coach));
                }
            }
            else
            {
                var Pop = new Pages.PopUps.ErrorMessage("Error", Response, "OK");
                await App.Current.MainPage.Navigation.PushPopupAsync(Pop, true);
            }
        }

        private async void SendCall(Entitys.NoteInformation note)
        {
            var LoadingPop = new Pages.PopUps.Loading("Sending Note");
            await App.Current.MainPage.Navigation.PushPopupAsync(LoadingPop, true);

            string Response = await SharedClasses.WebserviceCalls.SendNote(note);

            await App.Current.MainPage.Navigation.PopPopupAsync();

            if (Response == "success")
            {
                DoneAnimationFrame.IsVisible = true;
                DoneAnimation.IsVisible = true;
                DoneAnimation.Play();
            }
            else
            {
                var Pop = new Pages.PopUps.ErrorMessage("Error", Response, "OK");
                await App.Current.MainPage.Navigation.PushPopupAsync(Pop, true);
            }
        }

        override protected bool OnBackButtonPressed()
        {
            return false;
        }

        private async void AnimationComplete(object sender, EventArgs e)
        {
            await EndRoutineAsync();
        }

        private async Task EndRoutineAsync()
        {
            if (PassedBy == "Client")
            {
                await Navigation.PushAsync(new Pages.Client.ClientNote(Client));
            }
            else if (PassedBy == "Coach")
            {
                await Navigation.PushAsync(new Pages.Coach.CoachNote(Coach));
            }
        }
    }
}