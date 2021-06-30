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
    public class ExerciseList
    {
        //private static int idCounter = 0;

        //public int uniqueID { get; set; }
        //public int setID { get; set; }
        //public int addedID { get; set; }
        public int ExerciseID { get; set; }
        public int SupersetID { get; set; }
        public char FirstLetter { get; set; }
        public string ExerciseDescription { get; set; }
        public int FirstMuscleCode { get; set; }
        public string FirstMuscleCodeDescription { get; set; }
        public int? SecondMuscleCode { get; set; }
        public string SecondMuscleCodeDescription { get; set; }
        public int? ThirdMuscleCode { get; set; }
        public string ThirdMuscleCodeDescription { get; set; }
        //public string Reps { get; set; }
        //public string Weight { get; set; }
        //public string RoutineName { get; set; }

        //public string UserID { get; set; }
        //public string RestTime { get; set; }

    
    }

    public class ExerciseListRootObject
    {
        public List<ExerciseList> ExerciseList { get; set; }
    }

  
}
