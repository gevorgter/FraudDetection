using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FraudDetection.RuleEngine
{
    public class RuleEngineAccountManager
    {
        static object _lockObject = new object();
        static Dictionary<int, RuleEngineAccount> _accounts = null;

        public static void Init()
        {
            _accounts = new Dictionary<int, RuleEngineAccount>();
        }

        public static void AddRule(int midTidId, Rule rl)
        {
            RuleEngineAccount acc;
            lock (_lockObject)
            {
                if (!_accounts.TryGetValue(midTidId, out acc))
                {
                    acc = new RuleEngineAccount(midTidId);
                    acc.Init();
                    _accounts[midTidId] = acc;
                }
            }
            rl.Normalize();
            acc.AddRule(rl);
        }

        public static DateTime IsBlocked(int midTidId, Transaction tr)
        {
            if (!_accounts.TryGetValue(midTidId, out var acc))
                return DateTime.MinValue;
            return acc.IsBlocked(tr);
        }
    }
}
