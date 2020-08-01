using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TrackWorkout.Entitys;
using WebService.Processors;

namespace WebService.Controllers
{
    public class UpdateSettingsController : ApiController
    {

        [HttpPost]
        [Route("UpdateSettings")]
        // POST api/values
        public DataSet UpdateSettings(UserInformation UserInfo)
        {
            return GetUserDataProcessor.UpdateUserSettings(UserInfo);
        }


    }
}