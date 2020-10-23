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
                switch (name)
                {
                    case "amount":
                        tr.amount = Decimal.Parse(value);
                        break;
                    case "sourceid":
                        tr.sourceId = Int32.Parse(value);
                        break;
                    case "declined":
                        tr.declined = Boolean.Parse(value);
                        break;
                    case "ip":
                        tr.ip = value;
                        break;
                    default:
                        response = $"property {name} is not valid";
                        break;
                }
            }
            AccountManager.QueueTransaction(new Tuple<int,Transaction>(rq.midTidId, tr));
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
