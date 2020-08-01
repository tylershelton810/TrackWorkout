using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebService.Models;
using WebService.Processors;
using TrackWorkout.Entitys;

namespace WebService.Controllers
{
    public class ClientRequestController : ApiController
    {

        [HttpPost]
        [Route("ClientRequest")]
        // POST api/values
        public int ClientRequest(ClientRequestInfo clientRequest)
        {

            if (clientRequest == null)
            {
                return -1;
            }
            return ClientRequestProcessor.ProcessClientRequest(clientRequest);
        }


    }
}