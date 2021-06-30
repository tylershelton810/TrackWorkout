using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace TrackWorkout.Entitys
{
    public class RoutineList
    {
        public int RoutineID { get; set; }
        public string RoutineName { get; set; }
        public string ExerciseDescription { get; set; }
        public int SupersetID { get; set; }
        public int ExerciseNumber { get; set; }
        public int SetNumber { get; set; }
        public string Weight { get; set; }
        public string Reps { get; set; }
        public int RestTimeAfterSet { get; set; }
        public string Notes { get; set; }
        public int ProcedureType { get; set; }
        public bool KeepRow { get; internal set; }
        public string UserID { get; set; }
        public int CoachID { get; set; }
        public string CoachName { get; set; }
        public bool Locked { get; set; }
        public string RoutineDetail { get; set; }  
        public string PRData { get; set; }
    }

    public class RoutineListRootObject
    {
        public List<RoutineList> RoutineList { get; set; }
    }
}
