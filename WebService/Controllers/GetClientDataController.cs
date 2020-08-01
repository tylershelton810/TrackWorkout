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
    public class GetClientDataController : ApiController
    {

        [HttpPost]
        [Route("GetClientData")]
        // POST api/values
        public DataSet GetClientData(UserID userID)
        {

            if (userID == null)
            {
                DataSet fakeDataSet = new DataSet();
                //throw
                return fakeDataSet;
            }
            return GetClientDataProcessor.ProcessGetClientData(userID);
        }


    }
}