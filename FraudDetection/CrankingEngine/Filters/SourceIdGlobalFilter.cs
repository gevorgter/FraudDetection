﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FraudDetection.CrankingEngine
{
    public class SourceIdGlobalFilter : GlobalTransactionFilter
    {
        public override string filterName { get => "SourceIdGlobalFilter"; }

        public override bool Review(Transaction tr)
        {
            if (tr.sourceId == 29)
                return false;
            return true;
        }
    }
}
