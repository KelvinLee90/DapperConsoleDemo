using Microsoft.EntityFrameworkCore;
using PerformanceCompareDapperAndEntityFW.Models;
using PerformanceCompareDapperAndEntityFW.TestData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceCompareDapperAndEntityFW.DataAccessFW
{
    public class EntityFrameworkCore : IBasicTestFunctions
    {
        public long GetPlayerById(int id)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            using (SportDbEFCoreContext context = new SportDbEFCoreContext(Database.GetOptions()))
            {
                var player = context.Players.First(x => x.Id == id);
            }
            watch.Stop();
            return watch.ElapsedMilliseconds;
        }

        public long GetRosterByTeamId(int teamId)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            using (SportDbEFCoreContext context = new SportDbEFCoreContext(Database.GetOptions()))
            {
                var players = context.Teams.Include(x => x.Players).AsNoTracking().Single(x => x.Id == teamId);
            }
            watch.Stop();
            return watch.ElapsedMilliseconds;
        }

        public long GetTeamRostersForSport(int sportId)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            using (SportDbEFCoreContext context = new SportDbEFCoreContext(Database.GetOptions()))
            {
                var players = context.Teams.Include(x => x.Players).Where(x => x.SportId == sportId).AsNoTracking().ToList();
            }
            watch.Stop();
            return watch.ElapsedMilliseconds;
        }
    }
}
