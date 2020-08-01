using System;
namespace TrackWorkout.Entitys
{
    public class Error
    {
        public string Email { get; set; }
        public string ErrorMessage { get; set; }
        public string Page { get; set; }
        public string PassedData { get; set; }
        public string WebServiceCall { get; set; }
    }
}
