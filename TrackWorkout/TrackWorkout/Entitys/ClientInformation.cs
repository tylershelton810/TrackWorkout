using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrackWorkout.Entitys
{
    public class ClientInformation
    {
        public string ClientName { get; set; }
        public string ClientEmail { get; set; }
        public double ClientWeight { get; set; }
        public string WeightType { get; set; }
        public string Status { get; set; }
        public DateTime ClientBirthday { get; set; }
        public DateTime BecameClientDate { get; set; }
        public string ClientID { get; set; }
        public int CoachID { get; set; }
        public string ProfilePicture { get; set; }
        public UriImageSource ProfileImage { get; set; }
        public string ClientWeightHistory { get; set; }
        public string ClientMeasurementHistory { get; set; }
        public string ClientPRData { get; set; }
        public string addClientSQLReturn { get; set; }
        public int OrderID { get; set; }
    }

    public class ClientInformationRootObject
    {
        public List<ClientInformation> ClientInformation { get; set; }
    }
}
