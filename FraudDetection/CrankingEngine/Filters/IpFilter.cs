using FraudDetection.RuleEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FraudDetection.CrankingEngine
{
    /*
    public class IpFilter : ReviewFilter
    {
        TimeSpan _reviewtime = TimeSpan.FromMinutes(10);
        string[] _fieldsToMatch = new string[] { "ip" };

        public override string filterName { get => "IpFilter"; }

        public override void Review(CrankingEngineAccount account, Transaction lastTransaction)
        {
            int amountOfDeclines = 0;
            var transactions = account.GetTransactions(_reviewtime, _fieldsToMatch, lastTransaction);
            foreach (var t in transactions)
            {
                if (t.declined)
                    amountOfDeclines++;
            }
            if (amountOfDeclines > _amountOfDeclines)
            {
                Rule rl = new Rule();
                rl.AddParameter("ip", new ParameterValue(lastTransaction.ip));
                RuleEngineAccountManager.AddRule(account._midTidId, rl);
            }
        }
    }
    */
}
