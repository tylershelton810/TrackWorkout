using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrackWorkout.Entitys
{
    public class AcceptRejectCoach
    {
        public string NoteID { get; set; }
        public string CoachID { get; set; }
        public string UserID { get; set; }
        public string AcceptOrReject { get; set; }
    }
}
