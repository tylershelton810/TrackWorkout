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
    public class GetUserDataController : ApiController
    {

        [HttpPost]
        [Route("GetUserData")]
        // POST api/values
        public DataSet GetUserData(UserEmail email)
        {

            if (email == null)
            {
                DataSet fakeDataSet = new DataSet();
                //throw
                return fakeDataSet;
            }
            return GetUserDataProcessor.ProcessGetUserData(email);
        }


    }
}