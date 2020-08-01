using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace TrackWorkout.Entitys
{
    public class CoachList
    {
        public string Name { get; set; }
        public int CoachID { get; set; }
        public int ClientID { get; set; }
        public string Email { get; set; }
        public DateTime RelationshipCreateDate { get; set; }
        public string Status { get; set; }
        public int StatusType { get; set; }
        public string ProfilePicture { get; set; }
        public UriImageSource ProfileImage { get; set; }
        public string addCoachSQLReturn { get; set; }
        public int OrderID { get; set; }
    }

    public class CoachListRootObject
    {
        public List<CoachList> CoachList { get; set; }
    }
}
