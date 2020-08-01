using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebService.Models;
using WebService.Repositories;
using TrackWorkout.Entitys;

namespace WebService.Processors
{
    public class ClientRequestProcessor
    {
        public static int ProcessClientRequest(ClientRequestInfo clientRequest)
        {
            //Processing. Validating. Formatting.

            return ClientRequestRepository.ClientRequest(clientRequest);
        }
    }
}