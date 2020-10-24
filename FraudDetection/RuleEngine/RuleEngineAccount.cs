using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace FraudDetection.RuleEngine
{
    public class RuleEngineAccount
    {
        public int _midTidId { get; set; }
        List<Rule> _rules = new List<Rule>();

        public RuleEngineAccount(int midTidId)
        {
            _midTidId = midTidId;
        }
        public void AddRule(Rule r1)
        {
            //check that we already do not have that rule active. If it's not active then activate it.
            DateTime dtNow = DateTime.Now;
            foreach (var r2 in _rules)
            {
                if (Rule.Compare(r2, r1))
                {
                    if (r2._expirationTime < dtNow)
                        r2.ActivateRule(dtNow);//we already had that rule but it was not active. 

                    //we already have that rule, let's exit
                    return;
                }
            }
            //we do not have that rule, activate it and add to the list
            r1.ActivateRule(dtNow);
            _rules.Add(r1);
        }
        public void Init()
        {
        }

        public DateTime IsBlocked(Transaction tr)
        {
            DateTime dtNow = DateTime.Now;
            foreach (var rl in _rules)
            {
                if (!rl.IsExpired(dtNow) && rl.IsMatching(tr))
                    return rl._expirationTime;
            }
            return DateTime.MinValue;
        }
    }
}
