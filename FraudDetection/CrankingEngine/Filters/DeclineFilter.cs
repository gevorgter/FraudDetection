using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FraudDetection.CrankingEngine
{
    public class DeclineFilter : ReviewFilter
    {
        public TimeSpan reviewTime = TimeSpan.FromMinutes(10);

        public override void Review(Account account, List<Transaction> transactions)
        {
            int amountOfDeclines = 0;
            int amountOfSuccess = 0;
            DateTime dtNow = DateTime.Now;

            foreach (Transaction t in transactions)
            {
                if (dtNow.Subtract(t.transactionTime) > reviewTime)
                    continue; //do not need this transactions
                if (t.declined)
                    amountOfDeclines++;
                else
                    amountOfSuccess++;
            }
            if (amountOfDeclines > 5)
            {
                //block this account
                Console.WriteLine($"We are blocking {account._midTidId}");
            }
        }

    }
}
