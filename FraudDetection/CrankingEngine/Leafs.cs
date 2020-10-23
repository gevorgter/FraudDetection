using System;
using System.Collections.Generic;

namespace FraudDetection.CrankingEngine
{
    public enum LEAFID { STORE = 1, MID = 2, AMOUNT = 3, SOURCEID = 4, TRANSACTIONTYPE = 5, CREDITCARD = 6, AVS = 7, SUCCESSFUL = 8 }

    public enum REVIEWFILTERTYPE { DECLINEFILTER = 1}
    //filter that will review account
    public abstract class ReviewFilter
    {
        public virtual int order { get; set; } = 0;
        //return false if no need to continue through other filters
        public abstract void Review(Account account, List<Transaction> transactions);

        public virtual void Init(Account account) { }
    }

    //filter that will answer ignore or add transaction to the stack
    public abstract class TransactionFilter
    {
        public virtual int order { get; set; } = 0;
        //return false if no need to continue through other filters
        public abstract bool Review(Account account, Transaction tr);
        public virtual void Init(Account account) { }
    }

    //filter that will answer ignore or add transaction to the stack. Regardless of account
    public abstract class GlobalTransactionFilter
    {
        public virtual int order { get; set; } = 0;
        //return false if no need to continue through other filters
        public abstract bool Review(Transaction tr);
    }

    
}
