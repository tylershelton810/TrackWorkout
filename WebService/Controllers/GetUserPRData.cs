using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebService.Models;
using WebService.Processors;
using System.Data;

namespace WebService.Controllers
{
    public class GetUserPRDataController : ApiController
    {

        [HttpPost]
        [Route("GetUserPRData")]
        // POST api/values
        public DataSet GetUserPRData(UserID userID)
        {

            if (userID == null)
            {
                DataSet fakeDataSet = new DataSet();
                //throw
                return fakeDataSet;
            }
            return GetUserDataProcessor.GetUserPRData(userID);
        }


    }
}