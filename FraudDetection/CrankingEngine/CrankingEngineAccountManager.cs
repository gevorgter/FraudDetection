using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace FraudDetection.CrankingEngine
{


    public static class CrankingEngineAccountManager
    {
        static object _lockObject = new object();
        static Timer maintenanceTime = null;
        static Dictionary<int, CrankingEngineAccount> _accounts = null;
        static List<GlobalTransactionFilter> _transactionFilter = new List<GlobalTransactionFilter>();

        public static void Init()
        {
            _accounts = new Dictionary<int, CrankingEngineAccount>();
            maintenanceTime = new Timer(Maintain);
            maintenanceTime.Change(FraudDetectionDefaults.maintenanceTime, FraudDetectionDefaults.maintenanceTime); 
            
            var en = FilterHelper.GetGlobalTransactionFilters();
            while (en.MoveNext())
                _transactionFilter.Add(en.Current.Value());

            _transactionFilter.OrderByDescending(x => x.priority);
            _transactionFilter.ForEach(x => x.Init());
        }

        public static void QueueTransaction(int midTidId, Transaction t)
        {
            ThreadPool.QueueUserWorkItem(ProcessQueuedTransaction, new Tuple<int, Transaction>(midTidId, t));
        }

        public static void ProcessQueuedTransaction(object state)
        {
            (int midTidId, Transaction t) = (Tuple<int, Transaction>)state;
            AddTransaction(midTidId, t);
        }

        public static void AddTransaction(int midTidId, Transaction t)
        {
            CrankingEngineAccount acc;
            bool process = true;
            foreach (var f in _transactionFilter)
            {
                process &= f.Review(t);
                if (!process)
                    return;
            }
            //lets process this transaction
            lock (_lockObject)
            {
                if (!_accounts.TryGetValue(midTidId, out acc))
                    acc = LoadAccount(midTidId);
            }
            acc.AddTransaction(t);
        }

        /// <summary>
        /// Loads account that has not been hit yet
        /// </summary>
        /// <param name="midTidId"></param>
        /// <returns></returns>
        public static CrankingEngineAccount LoadAccount(int midTidId)
        {
            var acc = new CrankingEngineAccount(midTidId);
            acc._retentionTime = FraudDetectionDefaults.retentionTime;
            acc.Init();

            //add common filters
            var en = FilterHelper.GetReviewFilters();
            while (en.MoveNext())
            {
                var filter = en.Current.Value();
                acc.AddReviewFilter(filter);
                filter.Init(acc);
            }
            acc.Normalize();
            _accounts[midTidId] = acc;
            return acc;
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
