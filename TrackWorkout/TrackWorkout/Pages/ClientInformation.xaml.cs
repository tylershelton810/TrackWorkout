using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrackWorkout.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ClientInformation : ContentPage
    {
        public ClientInformation(string ClientName, double ClientWeight, string WeightType, string status) //
        {
            InitializeComponent();

            Weight.Text = ClientWeight + " " + WeightType;
            Status.Text = status;
            NoteTitle.Text = ClientName;
        }
    }
}