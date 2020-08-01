using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TrackWorkout.Entitys;
using WebService.Processors;
using System.Data;

namespace WebService.Controllers
{
    public class GetUserHistoryDataController : ApiController
    {

        [HttpPost]
        [Route("GetUserHistoryData")]
        // POST api/values
        public DataSet GetUserHistoryData(UserEmail email)
        {

            if (email == null)
            {
                DataSet fakeDataSet = new DataSet();
                //throw
                return fakeDataSet;
            }
            return GetUserDataProcessor.GetUserHistoryData(email);
        }


    }
}