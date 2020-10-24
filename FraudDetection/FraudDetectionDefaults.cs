using FraudDetection.CrankingEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FraudDetection
{
    public static class FraudDetectionDefaults
    {
        public static TimeSpan[] banInMinutes = { TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(15) };
        public static int maintenanceTime = 10 * 60 * 1000;
        public static TimeSpan retentionTime = TimeSpan.FromMinutes(10);
    }
}
