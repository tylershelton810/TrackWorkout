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
    public class HistoryInformation
    {
        public string Type { get; set; }
        public int ID { get; set; }
        public int HistoryID { get; set; }
        public int RoutineID { get; set; }
        public DateTime CompleteDate { get; set; }
        public string Name { get; set; }
        public int TotalWeight { get; set; }
        public string WeightType { get; set; }
        public string Notes { get; set; }
        public string HistoryXML { get; set; }
    }

    public class HistoryRootObject

    {
        public List<HistoryInformation> HistoryInformation { get; set; }
    }
}
