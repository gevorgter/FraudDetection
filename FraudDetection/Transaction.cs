using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace FraudDetection
{
    public enum FIELDID { IP = 1, sourceId = 2, amount = 3, declined = 4 }

    public class Transaction
    {
        public DateTime transactionTime { get; set; }
        public decimal amount { get; set; }
        public int sourceId { get; set; }
        public bool declined { get; set; }
        public string ip { get; set; }
    }
    
}
