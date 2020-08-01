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
    public class UserMasterInfo
    {
        public int TypeID { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreateDate { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime? Birthday { get; set; }
        public double? Lbs { get; set; }
        public double? Kg { get; set; }
        public int? AccountType { get; set; }
        public string PublicCode { get; set; }
        public string WeightType { get; set; }
        public string PRData { get; set; }
        public string ProfilePicture { get; set; }
        public int? ThemeColor { get; set; }
        public int? ThemeColor2 { get; set; }
        public string Weight { get; set; }
        public string Measurements { get; set; }
        //ExerciseList
        public int? ExerciseID { get; set; }
        public char? FirstLetter { get; set; }
        public string ExerciseDescription { get; set; }
        public int? FirstMuscleCode { get; set; }
        public string FirstMuscleCodeDescription { get; set; }
        public int? SecondMuscleCode { get; set; }
        public string SecondMuscleCodeDescription { get; set; }
        public int? ThirdMuscleCode { get; set; }
        public string ThirdMuscleCodeDescription { get; set; }
        //History
        public string Type { get; set; }
        public int? ID { get; set; }
        public int? HistoryID { get; set; }
        public int? HistoryRoutineID { get; set; }
        public DateTime? CompleteDate { get; set; }
        public string HistoryName { get; set; }
        public string HistoryWeightType { get; set; }
        public string Notes { get; set; }
        public string HistoryXML { get; set; }
        //User Routine
        public int? RoutineID { get; set; }
        public string RoutineUserID { get; set; }
        public string RoutineName { get; set; }
        public int? CoachID { get; set; }
        public string CoachName { get; set; }
        public bool? Locked { get; set; }
        public string RoutineDetail { get; set; }
        //Note Data
        public string FromUser { get; set; }
        public string FromEmail { get; set; }
        public string ToUser { get; set; }
        public string ToEmail { get; set; }
        public int? NoteID { get; set; }
        public string Note { get; set; }
        public DateTime? NoteSentDate { get; set; }
        public string NoteType { get; set; }
        public string FromID { get; set; }
        public string ToID { get; set; }
        public string Subject { get; set; }
        //Client Data
        public string ClientName { get; set; }
        public string ClientEmail { get; set; }
        public double? ClientWeight { get; set; }
        public string ClientWeightType { get; set; }
        public string Status { get; set; }
        public DateTime? ClientBirthday { get; set; }
        public DateTime? BecameClientDate { get; set; }
        public string ClientID { get; set; }
        public int? ClientCoachID { get; set; }
        public string ClientProfilePicture { get; set; }
        public string ClientWeightHistory { get; set; }
        public string ClientMeasurementHistory { get; set; }
        public string ClientPRData { get; set; }
        public int? ClientOrderID { get; set; }
        //Coach List
        public string CoachListName { get; set; }
        public int? CoachListCoachID { get; set; }
        public int? CoachListClientID { get; set; }
        public string CoachListEmail { get; set; }
        public DateTime? RelationshipCreateDate { get; set; }
        public string CoachListStatus { get; set; }
        public int? StatusType { get; set; }
        public string CoachListProfilePicture { get; set; }
        public int? CoachOrderID { get; set; }
    }

    public class UserMasterInfoRootObject
    {
        public List<UserMasterInfo> UserMasterInfo { get; set; }
    }
}
