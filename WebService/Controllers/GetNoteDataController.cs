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
    public class GetNoteDataController : ApiController
    {

        [HttpPost]
        [Route("GetNoteData")]
        // POST api/values
        public DataSet GetNoteData(UserID userID)
        {

            if (userID == null)
            {
                DataSet fakeDataSet = new DataSet();
                //throw
                return fakeDataSet;
            }
            return GetNoteDataProcessor.ProcessGetNoteData(userID);
        }       
    }
}