using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FraudDetection.RuleEngine
{
    public class RuleEngineAccountManager
    {
        static object _lockObject = new object();
        static Timer maintenanceTime = null;
        static Dictionary<int, RuleEngineAccount> _accounts = null;

        public static void Init()
        {
            _accounts = new Dictionary<int, RuleEngineAccount>();
            maintenanceTime = new Timer(Maintain);
            maintenanceTime.Change(FraudDetectionDefaults.maintenanceTime, FraudDetectionDefaults.maintenanceTime); 
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

        /// <summary>
        /// go over all accounts and remove the one that are not needed
        /// </summary>
        public static void Maintain(object _)
        {
            lock (_lockObject)
            {
                DateTime tickNow = DateTime.Now;
                var it = _accounts.GetEnumerator();
                //keep accounts we want to remove since we can not remove them form collection we iterate on
                List<int> accountsToRemove = new List<int>();
                while (it.MoveNext())
                {
                    if (it.Current.Value.Maintain(tickNow) == 0)
                        accountsToRemove.Add(it.Current.Key);
                }
                foreach (var id in accountsToRemove)
                    _accounts.Remove(id);
            }
        }
    }
}
