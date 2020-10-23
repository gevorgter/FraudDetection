using System;
using System.Collections.Generic;
using System.Threading;

namespace FraudDetection.CrankingEngine
{
    public class Account
    {
        public int _midTidId { get; set; }
        public TimeSpan _retentionTime { get; set; } = DefaultAccountSettings.retentionTime;
        public List<Transaction> _transactions { get; set; } = new List<Transaction>(); //transactions for this account in order they come in.

        object lockObject = new object();
        
        SortedList<int, ReviewFilter> reviewFilters;
        SortedList<int, TransactionFilter> transactionFilters;

        public void AddTransaction(Transaction t)
        {
            lock (lockObject)
            {
                bool triggerReview = true;
                if (transactionFilters != null)
                {
                    foreach (var filter in transactionFilters)
                    {
                        triggerReview &= filter.Value.Review(this, t);
                        if (!triggerReview)
                            return;
                    }
                }
                //lets process this transaction
                if (reviewFilters != null)
                {
                    _transactions.Add(t);
                    TriggerReview();
                }

            }
        }
        public void AddReviewFilter(ReviewFilter filter)
        {
            lock (lockObject)
            {
                if (reviewFilters == null)
                    reviewFilters = new SortedList<int, ReviewFilter>();

                reviewFilters.Add(filter.order, filter);
                filter.Init(this);
            }
        }

        void AddTransactionFilter(TransactionFilter filter)
        {
            lock (lockObject)
            {
                if (transactionFilters == null)
                    transactionFilters = new SortedList<int, TransactionFilter>();

                transactionFilters.Add(filter.order, filter);
                filter.Init(this);
            }
        }

        /// <summary>
        /// Removes transactions that are older than retentionTime
        /// and returns amount of transactions left in a queue
        /// </summary>
        /// <param name="tickNow"></param>
        public int Maintain(DateTime tickNow)
        {
            lock (lockObject)
            {
                int indexToRemoveUpTo = 0;
                foreach (var t in _transactions)
                {
                    if (tickNow.Subtract(t.transactionTime) < _retentionTime)
                        break;//we found up to what point we need to remove transactions
                    indexToRemoveUpTo++;
                }
                if (indexToRemoveUpTo > 0)
                    _transactions.RemoveRange(0, indexToRemoveUpTo);
                return _transactions.Count;
            }
        }

        /// <summary>
        /// Reviews account
        /// </summary>
        void TriggerReview()
        {
            foreach (var filter in reviewFilters)
                filter.Value.Review(this, _transactions);
        }

    }
}
