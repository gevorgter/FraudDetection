using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace FraudDetection
{
    public enum LEAFID { STORE = 1, MID = 2, AMOUNT = 3, SOURCEID = 4, TRANSACTIONTYPE = 5, CREDITCARD = 6, AVS = 7, SUCCESSFUL=8 }

    //filter that will review account
    public abstract class ReviewFilter
    {
        public virtual int order { get; set; } = 0;
        //return false if no need to continue through other filters
        public abstract bool Review(IAccount account);
    }

    //filter that will answer ignore or add transaction to the stack
    public abstract class TransactionFilter
    {
        public virtual int order { get; set; } = 0;
        //return false if no need to continue through other filters
        public abstract bool Review(IAccount account, Transaction tr);
    }

    public class Transaction
    {
        public DateTime transactionTime { get; set; }
        public decimal amount { get; set; }
        public int sourceId { get; set; }
        public bool declined { get; set; }
    }

    public interface IAccount
    {
        void AddTransaction(Transaction t);
        void AddReviewFilter(ReviewFilter filter);
        void AddTransactionFilter(TransactionFilter filter);
        void Maintain(DateTime tickNow);
    }

    public class Account: IAccount
    {
        public int midTidId { get; set; }
        public TimeSpan retentionTime { get; set; } = TimeSpan.FromMinutes(20); //Time tranasactions are retained in memory

        object lockObject = new object();
        List<Transaction> transactions = new List<Transaction>(); //transactions for this account in order they come in.
        SortedList<int, ReviewFilter> reviewFilters;
        SortedList<int, TransactionFilter> transactionFilters;

        void IAccount.AddTransaction(Transaction t)
        {
            lock (lockObject)
            {
                bool triggerReview = true;
                foreach (var filter in transactionFilters)
                {
                    triggerReview &= filter.Value.Review(this, t);
                    if (!triggerReview)
                        break;
                }
                if (!triggerReview)
                {
                    transactions.Add(t);
                    TriggerReview();
                }
            }
        }
        void IAccount.AddReviewFilter(ReviewFilter filter)
        {
            lock (lockObject)
            {
                if (reviewFilters == null)
                    reviewFilters = new SortedList<int, ReviewFilter>();

                reviewFilters.Add(filter.order, filter);
            }
        }

        void IAccount.AddTransactionFilter(TransactionFilter filter)
        {
            lock (lockObject)
            {
                if (transactionFilters == null)
                    transactionFilters = new SortedList<int, TransactionFilter>();

                transactionFilters.Add(filter.order, filter);
            }
        }

        /// <summary>
        /// Removes transactions that are older than retentionTime
        /// </summary>
        /// <param name="tickNow"></param>
        void IAccount.Maintain(DateTime tickNow)
        {
            lock (lockObject)
            {
                int indexToRemoveUpTo = 0;
                foreach (var t in transactions)
                {
                    if (tickNow.Subtract(t.transactionTime) < retentionTime)
                        break;//we found up to what point we need to remove transactions
                    indexToRemoveUpTo++;
                }
                if (indexToRemoveUpTo > 0)
                    transactions.RemoveRange(0, indexToRemoveUpTo);
            }
        }

        /// <summary>
        /// Reviews account
        /// </summary>
        void TriggerReview()
        {
            foreach (var filter in reviewFilters)
                filter.Value.Review(this);
        }

        public IAccount AccessAccount()
        {
            if (Monitor.TryEnter(lockObject))
                return (IAccount)this;
            return null;
        }

    }
}
