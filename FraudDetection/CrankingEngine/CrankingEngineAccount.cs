using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace FraudDetection.CrankingEngine
{
    public class CrankingEngineAccount
    {
        public int _midTidId { get; set; }
        public TimeSpan _retentionTime { get; set; } = FraudDetectionDefaults.retentionTime;
        public List<Transaction> _transactions { get; set; } = new List<Transaction>(); //transactions for this account in order they come in.

        object lockObject = new object();
        
        List<ReviewFilter> reviewFilters;
        List<TransactionFilter> transactionFilters;

        public CrankingEngineAccount(int midTidId)
        {
            _midTidId = midTidId;
        }
        public void Init()
        {

        }

        public void AddTransaction(Transaction t)
        {
            lock (lockObject)
            {
                bool triggerReview = true;
                if (transactionFilters != null)
                {
                    foreach (var filter in transactionFilters)
                    {
                        triggerReview &= filter.Review(this, t);
                        if (!triggerReview)
                            return;
                    }
                }
                //lets process this transaction
                if (reviewFilters != null)
                {
                    _transactions.Add(t);
                    TriggerReview(t);
                }

            }
        }
        public void AddReviewFilter(ReviewFilter filter)
        {
            lock (lockObject)
            {
                reviewFilters ??= new List<ReviewFilter>();
                reviewFilters.Add(filter);
                filter.Init(this);
            }
        }

        public void AddTransactionFilter(TransactionFilter filter)
        {
            lock (lockObject)
            {
                transactionFilters ??= new List<TransactionFilter>();
                transactionFilters.Add(filter);
                filter.Init(this);
            }
        }

        public void Normalize()
        {
            reviewFilters?.OrderByDescending(x => x.priority);
            transactionFilters?.OrderByDescending(x => x.priority);
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
        void TriggerReview(Transaction tr)
        {
            foreach (var filter in reviewFilters)
                filter.Review(this, tr);
        }

        public IEnumerable<Transaction> GetTransactions(TimeSpan reviewTime, FIELDID[] fieldsToMatch, Transaction trToMatch)
        {
            DateTime dtNow = DateTime.Now;
            foreach (Transaction t in _transactions)
            {
                if (dtNow.Subtract(t.transactionTime) > reviewTime)
                    continue; //do not need this transactions
                if (TransactionHelper.CompareTransactions(fieldsToMatch, t, trToMatch))
                    yield return t;
            }
        }
    }
}
