using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceCompareDapperAndEntityFW.TestData
{
    public class TestResult
    {
        public double PlayerByIDMilliseconds { get; set; }
        public double PlayersForTeamMilliseconds { get; set; }
        public double TeamsForSportMilliseconds { get; set; }
        public Framework Framework { get; set; }
        public int Run { get; set; }
    }

    public enum Framework
    {
        EntityFrameworkCore,
        EntityFrameworkCoreWithTracking,
        Dapper
    }
}
