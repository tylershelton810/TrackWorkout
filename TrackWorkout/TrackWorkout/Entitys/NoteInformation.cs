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
    public class NoteInformation
    {
        public string FromUser { get; set; }
        public string FromEmail { get; set; }
        public string ToUser { get; set; }
        public string ToEmail { get; set; }
        public int NoteID { get; set; }
        public string Note { get; set; }
        public DateTime NoteSentDate { get; set; }
        public string NoteType { get; set; }
        public string FromID { get; set; }
        public string ToID { get; set; }
        public string Subject { get; set; }
    }

    public class NoteInformationRootObject
    {
        public List<NoteInformation> NoteInformation { get; set; }
    }
}
