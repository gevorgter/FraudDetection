using FraudDetection.RuleEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FraudDetection.CrankingEngine
{
    public class SourceIdFilter : ReviewFilter
    {
        TimeSpan _reviewtime = TimeSpan.FromMinutes(10);
        string []_fieldsToMatch = new string[] { "sourceId" };

        public override string filterName { get => "SourceIdFilter"; }

        public override void Review(CrankingEngineAccount account, Transaction lastTransaction)
        {
            int amountOfDeclines = 0;
            var transactions  = account.GetTransactions(_reviewtime, _fieldsToMatch, lastTransaction);
            foreach (var t in transactions)
            {
                if (t.declined)
                    amountOfDeclines++;
            }
            if (amountOfDeclines > 5)
            {
                Rule rl = new Rule();
                rl.AddParameter("sourceId", new ParameterValue(lastTransaction.sourceId));
                RuleEngineAccountManager.AddRule(account._midTidId, rl);
            }
        }

    }
}
