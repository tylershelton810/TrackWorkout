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
    public class PRInformation
    {
        public string Exercise { get; set; }
        public string Weight { get; set; }
        public string WeightType { get; set; }
        public DateTime RecordDate { get; set; }
        public int ExerciseID { get; set; }
    }

    public class PRRootObject
    {
        public List<PRInformation> PRInformation { get; set; }
    }
}
