using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrackWorkout.Pages.Client
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ClientNote : ContentPage
    {
        Entitys.ClientInformation PassClientInfo;

        public ClientNote(Entitys.ClientInformation Client) 
        {
            InitializeComponent();

            NoClientLabel.FontFamily = App.CustomBold;
            NoClientLabel.Text = $"Click + button to start a conversation with {Client.ClientName}";
            NoClientLabel.TextColor = App.SecondaryThemeColor;
            HeaderLayout.BackgroundColor = App.PrimaryThemeColor;
            Note.BackgroundColor = App.SecondaryThemeColor;
            ButtonGridBottom.BackgroundColor = App.PrimaryThemeColor;
            NoteButtonFrame.BackgroundColor = App.PrimaryThemeColor;
            NoteButtonFrame2.BackgroundColor = App.SecondaryThemeColor;

            NavigationPage.SetHasNavigationBar(this, false);            

            BackButton.Clicked += (o, e) =>
            {
                OnBackButtonPressed();
            };

            PassClientInfo = Client;

            try
            {
                var ClientsNotes = App.NoteInformationApp.Where(x => (x.ToID == Client.ClientID || x.FromID == Client.ClientID));

                List<string> uniqueByType = ClientsNotes.Select(x => x.NoteType).AsParallel().Distinct().ToList();
                // Loop through new list
                if(uniqueByType.Count() > 0)
                {
                    NoClientLabel.IsVisible = false;
                    NoClientAnimation.IsVisible = false;
                }
                for (int i = 0; i < uniqueByType.Count; i++)
                {
                    CreateNoteGrids(uniqueByType, i, ClientsNotes);
                }
            }
            catch {
                Label NoNotes = new Label
                {
                    Text = "There are no notes for this client.", 
                    FontFamily = App.CustomRegular,
                    FontSize = 15,
                    TextColor = App.PrimaryThemeColor,
                    HorizontalOptions = LayoutOptions.Center
                };

                gridStack.Children.Add(NoNotes);
            }
            
        }

        public void CreateNoteGrids(List<string> uniqueByType, int i, IEnumerable<Entitys.NoteInformation> ClientsNotes)
        {
            var listOfNotesWithSameType = ClientsNotes.Where(value => value.NoteType == uniqueByType[i]).ToList();

            listOfNotesWithSameType.Sort((x, y) => y.NoteSentDate.CompareTo(x.NoteSentDate));

            string NoteTypeLabelText = listOfNotesWithSameType.Min(x => x.NoteType);

            // Create Name Label
            Label NoteTypeLabel = new Label
            {
                Text = NoteTypeLabelText,
                FontSize = 15,
                FontFamily = App.CustomRegular,
                HeightRequest = 20,
                Margin = new Thickness(15, 10, 0, 0),
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.End,
                TextColor = App.PrimaryThemeColor
            };

            Frame NoteFrame = new Frame
            {
                HasShadow = false,
                Padding = 0,
                Margin = new Thickness(0, 0, 0, 10)
            };

            Grid NoteGridByType = new Grid
            {
                RowSpacing = 0,
                ColumnSpacing = 0,
                BackgroundColor = Color.White,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            NoteFrame.Content = NoteGridByType;

            // Add Routine Name to the Screen
            gridStack.Children.Add(NoteTypeLabel);

            for (int y = 0; y < listOfNotesWithSameType.Count; y++)
            {
                Entitys.NoteInformation NoteObject = listOfNotesWithSameType[y];

                string NameToDisplay;

                if (NoteObject.ToID == App.userInformationApp[0].UserId.ToString())
                {
                    NameToDisplay = NoteObject.FromUser;
                }
                else
                {
                    NameToDisplay = NoteObject.ToUser;
                }

                Button NoteButton = new Button
                {
                    BackgroundColor = Color.White,
                    TextColor = App.ThemeColor4,
                    FontFamily = App.CustomLight,
                    Text = listOfNotesWithSameType[y].Subject.ToString()
                };

                NoteGridByType.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40, GridUnitType.Absolute) });
                NoteGridByType.Children.Add(NoteButton, 0, y);                

                NoteButton.Clicked += async (o, e) =>
                {
                    await Navigation.PushAsync(new Pages.NoteInformation(NoteObject, "Client"));
                };
            }

            gridStack.Children.Add(NoteFrame);
        }

        private async void GoHome(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Pages.Client.ClientInformation(PassClientInfo));
        }

        private async void AddNote(object sender, EventArgs e)
        {
            var SubjectPop = new Pages.PopUps.NoteSubject();
            string Subject = await SubjectPop.Show();

            if (Subject != "Cancelled")
            {
                Entitys.NoteInformation noteInformation = new Entitys.NoteInformation
                {
                    FromUser = App.userInformationApp[0].Name,
                    FromEmail = App.userInformationApp[0].Email,
                    ToUser = PassClientInfo.ClientName,
                    Subject = Subject,
                    ToEmail = PassClientInfo.ClientEmail,
                    NoteID = -1,
                    Note = String.Empty,
                    NoteSentDate = DateTime.Today,
                    NoteType = "Conversation",
                    FromID = App.userInformationApp[0].UserId.ToString()
                };

                await Navigation.PushAsync(new Pages.NoteInformation(noteInformation, "Client"));
            }
        }

        private async void GoRoutine(object sender, EventArgs e)
        {            
            await Navigation.PushAsync(new Pages.Client.ClientRoutine(PassClientInfo));
        }

        override protected bool OnBackButtonPressed()
        {
            Navigation.PushAsync(new ClientsDetail());

            return true;
        }

        //void removePagefromStack(string pageToRemove)
        //{
        //    foreach (var item in Navigation.NavigationStack)
        //    {
        //        if (item.GetType().Name == pageToRemove)
        //        {
        //            Navigation.RemovePage(item);
        //            break;
        //        }
        //    }
        //}
    }
}