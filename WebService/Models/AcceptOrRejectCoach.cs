using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebService.Models
{
    public class AcceptOrRejectCoach
    {
        public string NoteID { get; set; }
        public string CoachID { get; set; }
        public string UserID { get; set; }
        public string AcceptOrReject { get; set; }

    }
}