using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebService.Models;
using WebService.Repositories;
using System.Data;
using TrackWorkout.Entitys;

namespace WebService.Processors
{
    public class GetNoteDataProcessor
    {
        public static DataSet ProcessGetNoteData(UserID userID)
        {
            //Processing. Validating. Formatting.

            return GetUserDataRepository.GetNoteData(userID);
        }
        public static DataSet SendNote(NoteInformation note)
        {
            //Processing. Validating. Formatting.

            return ClientRequestRepository.SendNote(note);
        }
    }
}