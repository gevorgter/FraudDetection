using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FraudDetection.CrankingEngine
{
    public class FilterHelper
    {

        public static ReviewFilter GetFilter(REVIEWFILTERTYPE filterType)
        {
            switch (filterType)
            {
                case REVIEWFILTERTYPE.DECLINEFILTER:
                    return new DeclineFilter();
                default:
                    throw new Exception("Uknown Filter Type");
            }
        }
    }
}
