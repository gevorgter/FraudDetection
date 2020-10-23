using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FraudDetection
{
    public class SourceIdGlobalFilter : GlobalTransactionFilter
    {
        public override bool Review(Transaction tr)
        {
            if (tr.sourceId == 29)
                return false;
            return true;
        }
    }
}
