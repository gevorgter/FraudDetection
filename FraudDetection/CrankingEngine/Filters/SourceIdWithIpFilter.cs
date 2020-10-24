using FraudDetection.RuleEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FraudDetection.CrankingEngine.Filters
{
    public class SourceIdWithIpFilter : ReviewFilter
    {
        TimeSpan _reviewtime = TimeSpan.FromMinutes(10);
        FIELDID[] _fieldsToMatch = new FIELDID[] { FIELDID.sourceId, FIELDID.IP };

        public override string filterName { get => "SourceIdWithIpFilter"; }

        public override void Review(CrankingEngineAccount account, Transaction lastTransaction)
        {
            int amountOfDeclines = 0;
            var transactions = account.GetTransactions(_reviewtime, _fieldsToMatch, lastTransaction);
            foreach (var t in transactions)
            {
                if (t.declined)
                    amountOfDeclines++;
            }
            if (amountOfDeclines > 5)
            {
                Rule rl = new Rule();
                rl.AddParameter(FIELDID.sourceId, new ParameterValue(lastTransaction.sourceId));
                rl.AddParameter(FIELDID.IP, new ParameterValue(lastTransaction.ip));
                RuleEngineAccountManager.AddRule(account._midTidId, rl);
            }
        }

    }
}
