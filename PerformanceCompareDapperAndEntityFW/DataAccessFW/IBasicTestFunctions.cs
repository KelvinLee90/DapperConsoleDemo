using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceCompareDapperAndEntityFW.DataAccessFW
{
    public interface IBasicTestFunctions
    {
        long GetPlayerById(int id);
        long GetRosterByTeamId(int teamId);
        long GetTeamRostersForSport(int sportId);
    }
}
