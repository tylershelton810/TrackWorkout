using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebService.Models;
using WebService.Processors;
using System.Data;
using TrackWorkout.Entitys;

namespace WebService.Controllers
{
    public class SendNoteController : ApiController
    {        
        [HttpPost]
        [Route("SendNote")]
        // POST api/values
        public DataSet SendNote(NoteInformation note)
        {

            if (note == null)
            {
                DataSet fakeDataSet = new DataSet();
                //throw
                return fakeDataSet;
            }
            return GetNoteDataProcessor.SendNote(note);
        }
    }
}