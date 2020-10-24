using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace FraudDetection.CrankingEngine
{

    public static class FilterHelper
    {
        static Dictionary<string, Func<GlobalTransactionFilter>> _mapGlobalTransactioFilters = new Dictionary<string, Func<GlobalTransactionFilter>>();
        static Dictionary<string, Func<ReviewFilter>> _mapReviewFilters = new Dictionary<string, Func<ReviewFilter>>();
        static Dictionary<string, Func<TransactionFilter>> _mapTransactionFilters = new Dictionary<string, Func<TransactionFilter>>();

        public static Dictionary<string, Func<ReviewFilter>>.Enumerator GetReviewFilters()
        {
            return _mapReviewFilters.GetEnumerator();
        }

        public static Dictionary<string, Func<GlobalTransactionFilter>>.Enumerator GetGlobalTransactionFilters()
        {
            return _mapGlobalTransactioFilters.GetEnumerator();
        }

        public static void ScanAssembly(Assembly assembly = null)
        {
            if (assembly == null)
                assembly = typeof(FilterHelper).Assembly;
            var tGlobalTransactionFilter = typeof(GlobalTransactionFilter);
            var tReviewFilter = typeof(ReviewFilter);
            var tTransactionFilter = typeof(TransactionFilter);

            foreach (Type t in assembly.GetTypes())
            {
                if (t.IsAbstract)
                    continue;

                if (tGlobalTransactionFilter.IsAssignableFrom(t))
                {
                    var f = GetInstantiator<GlobalTransactionFilter>(t);
                    var o = f();
                    _mapGlobalTransactioFilters.Add(o.filterName, f);
                    continue;
                }
                if (tReviewFilter.IsAssignableFrom(t))
                {
                    var f = GetInstantiator<ReviewFilter>(t);
                    var o = f();
                    _mapReviewFilters.Add(o.filterName, f);
                    continue;
                }
                if (tTransactionFilter.IsAssignableFrom(t))
                {
                    var f = GetInstantiator<TransactionFilter>(t);
                    var o = f();
                    _mapTransactionFilters.Add(o.filterName, f);
                    continue;
                }
            }

        }

        public static Func<T> GetInstantiator<T>(Type t)
        {
            return Expression.Lambda<Func<T>>(Expression.New(t)).Compile();
        }
    }
}
