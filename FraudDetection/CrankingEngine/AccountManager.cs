using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

namespace FraudDetection.CrankingEngine
{
    public static class DefaultAccountSettings
    {
        public static int maintenanceTime = 10 * 60 * 1000;
        public static TimeSpan retentionTime = TimeSpan.FromMinutes(10);
        public static List<REVIEWFILTERTYPE> _commonFilters = new List<REVIEWFILTERTYPE>()
        {
            REVIEWFILTERTYPE.DECLINEFILTER,
        };
    }

    public static class AccountManager
    {
        static object _lockObject = new object();
        static Timer maintenanceTime = null;
        static Dictionary<int, Account> _accounts = null;
        static List<GlobalTransactionFilter> _transactionFilter = null;
        public static void Init()
        {
            _accounts = new Dictionary<int, Account>();
            maintenanceTime = new Timer(Maintain);
            maintenanceTime.Change(DefaultAccountSettings.maintenanceTime, DefaultAccountSettings.maintenanceTime); //perform maintenance every 10 minutes
            _transactionFilter = new List<GlobalTransactionFilter>()
            {
                new SourceIdGlobalFilter(), //we will not even attempt to process transasction with that sourceId
            };
            _transactionFilter.OrderByDescending(x => x.order);
        }

        public static void QueueTransaction(Tuple<int, Transaction> t)
        {
            ThreadPool.QueueUserWorkItem(ProcessQueuedTransaction, t);
        }

        public static void ProcessQueuedTransaction(object state)
        {
            (int midTidId, Transaction t) = (Tuple<int, Transaction>)state;
            AddTransaction(midTidId, t);
        }

        public static void AddTransaction(int midTidId, Transaction t)
        {
            Account acc;
            bool process = true;
            foreach(var f in _transactionFilter)
            {
                process &= f.Review(t);
                if (!process) 
                    return;
            }
            //lets process this transaction
            lock (_lockObject)
            {
                if( !_accounts.TryGetValue(midTidId, out acc))
                    acc = LoadAccount(midTidId);
            }
            acc.AddTransaction(t);
        }

        /// <summary>
        /// Loads account that has not been hit yet
        /// </summary>
        /// <param name="midTidId"></param>
        /// <returns></returns>
        public static Account LoadAccount(int midTidId)
        {
            var acc = new Account();
            acc._retentionTime = DefaultAccountSettings.retentionTime;

            //add common filters
            foreach(REVIEWFILTERTYPE filterType in DefaultAccountSettings._commonFilters)
            {
                var filter = FilterHelper.GetFilter(filterType);
                acc.AddReviewFilter(filter);
                filter.Init(acc);
            }

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
                while(it.MoveNext())
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
