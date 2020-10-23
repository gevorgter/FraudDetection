using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace FraudDetection.Controllers
{
    [ApiController]
    //[Route("api/[controller]")]
    [Route("V1")]
    public class FraudDetectionController : ControllerBase
    {
        private readonly ILogger<FraudDetectionController> _logger;
        static ConcurrentQueue<ReportRequest> _queue = new System.Collections.Concurrent.ConcurrentQueue<ReportRequest>();

        public FraudDetectionController(ILogger<FraudDetectionController> logger)
        {
            _logger = logger;
        }


        [HttpPost("Report")]
        public string Report(ReportRequest rq)
        {
            ThreadPool.QueueUserWorkItem(ThreadProc, rq);
            return "OK";
        }

        // This thread procedure performs the task.
        static void ThreadProc(Object stateInfo)
        {
            _queue.Enqueue((ReportRequest)stateInfo);
            Console.WriteLine("Queed");
        }
    }

    public class Parameter
    {
        public string name { get; set; }
        public string value { get; set; }
    }
    public class ReportRequest
    {
        public int midTidId { get; set; }
        public List<Parameter> parameters { get; set; }
    }
}
