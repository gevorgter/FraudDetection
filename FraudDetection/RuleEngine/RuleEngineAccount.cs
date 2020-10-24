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
        object _lockObject = new object();
        List<Rule> _rules = new List<Rule>();

        public RuleEngineAccount(int midTidId)
        {
            _midTidId = midTidId;
        }
        public void AddRule(Rule r1)
        {
            lock (_lockObject)
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
        }
        public void Init()
        {
        }

        public DateTime IsBlocked(Transaction tr)
        {
            lock (_lockObject)
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

        /// <summary>
        /// finds expired rules and remove if they are past latest ban time
        /// returns amount of rules it has
        /// </summary>
        /// <param name="dtNow"></param>
        /// <returns></returns>
        public int Maintain(DateTime dtNow)
        {
            lock (_lockObject)
            {
                List<Rule> rules = new List<Rule>();
                foreach (var r in _rules)
                {
                    if (r._expirationTime < dtNow)
                    {
                        if (dtNow.Subtract(r._expirationTime) > FraudDetectionDefaults.banInMinutes[FraudDetectionDefaults.banInMinutes.Length])
                            continue; //do not add this rule to the list
                    }
                    rules.Add(r);
                }
                _rules = rules;
                return _rules.Count;
            }
        }
    }
}
