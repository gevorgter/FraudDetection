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
        int _amountOfDeclines = 3;
        FIELDID []_fieldsToMatch = new FIELDID[] { FIELDID.sourceId };

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
            if (amountOfDeclines > _amountOfDeclines)
            {
                Rule rl = new Rule();
                rl.AddParameter(FIELDID.sourceId, new ParameterValue(lastTransaction.sourceId));
                RuleEngineAccountManager.AddRule(account._midTidId, rl);
            }
        }

    }
}
