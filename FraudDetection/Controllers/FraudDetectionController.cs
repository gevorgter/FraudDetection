using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
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
            string response = "OK";
            Transaction tr = new Transaction()
            {
                transactionTime = DateTime.Now
            };
            var en = rq.parameters.EnumerateObject();
            while(en.MoveNext())
            {
                string name = en.Current.Name.ToLower();
                string value = en.Current.Value.ToString();
                if( !TransactionHelper.populateTransaction(name, value, tr))
                    response = $"property {name} is not valid";
            }
            CrankingEngine.CrankingEngineAccountManager.QueueTransaction(rq.midTidId, tr);
            return response;
        }

        [HttpPost("Check")]
        public string Check(ReportRequest rq)
        {
            string response = "OK";
            Transaction tr = new Transaction()
            {
                transactionTime = DateTime.Now
            };
            var en = rq.parameters.EnumerateObject();
            while (en.MoveNext())
            {
                string name = en.Current.Name.ToLower();
                string value = en.Current.Value.ToString();
                if (!TransactionHelper.populateTransaction(name, value, tr))
                    response = $"property {name} is not valid";
            }
            DateTime blockedTill = RuleEngine.RuleEngineAccountManager.IsBlocked(rq.midTidId, tr);
            if( blockedTill != DateTime.MinValue)
                response = $"Blocked till {blockedTill.ToString("hh:mm:ss tt")}";
            return response;
        }

        // This thread procedure performs the task.
        static void ThreadProc(Object stateInfo)
        {
            _queue.Enqueue((ReportRequest)stateInfo);
            Console.WriteLine("Queed");
        }
    }

    public class ReportRequest
    {
        public int midTidId { get; set; }
        public JsonElement parameters { get; set; }
    }
}
