using System;
using System.Collections.Generic;

namespace FraudDetection.CrankingEngine
{
    public abstract class Filter
    {
        public virtual int priority { get; set; } = 0;
        public abstract string filterName { get; }
    }


    //filter that will review account
    public abstract class ReviewFilter: Filter
    {
        public abstract void Review(CrankingEngineAccount account, Transaction lastTransaction);
        public virtual void Init(CrankingEngineAccount account) { }
    }

    //filter that will answer ignore or add transaction to the stack
    public abstract class TransactionFilter: Filter
    {
        public abstract bool Review(CrankingEngineAccount account, Transaction tr);
        public virtual void Init(CrankingEngineAccount account) { }
    }

    //filter that will answer ignore or add transaction to the stack. Regardless of account
    public abstract class GlobalTransactionFilter : Filter
    {
        //return false if no need to continue through other filters
        public abstract bool Review(Transaction tr);
    }

    
}
