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
    public class UserInformation
    {
        public int UserId { get; set; }
        public DateTime CreateDate { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime Birthday { get; set; }
        public double Lbs { get; set; }
        public double Kg { get; set; }
        public int AccountType { get; set; }
        public string PublicCode { get; set; }
        public string WeightType { get; set; }        
        public string PRData { get; set; }
        public string ProfilePicture { get; set; }
        public UriImageSource ProfileImage { get; set; }
        public int ThemeColor { get; set; }
        public int ThemeColor2 { get; set; }
        public string Weight { get; set; }
        public string Measurements { get; set; }
    }

    public class UserInformationRootObject
    {
        public List<UserInformation> UserInformation { get; set; }
    }    
}
