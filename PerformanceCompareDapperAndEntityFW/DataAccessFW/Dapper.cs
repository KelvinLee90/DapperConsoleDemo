using Dapper;
using PerformanceCompareDapperAndEntityFW.DTOs;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceCompareDapperAndEntityFW.DataAccessFW
{
    public class Dapper :IBasicTestFunctions
    {
        public long GetPlayerById(int id)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            using(SqlConnection conn = new SqlConnection(ConnectionString.DemoDbConnectionString))
            {
                conn.Open();
                var query = $"SELECT Id, FirstName, LastName, DateOfBirth, TeamId FROM Players WHERE Id = @Id";
                var player = conn.QuerySingle<PlayerDTO>(query, new { Id = id });
            }
            watch.Stop();
            return watch.ElapsedMilliseconds;
        }

        public long GetRosterByTeamId(int teamId)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            using (SqlConnection conn = new SqlConnection(ConnectionString.DemoDbConnectionString))
            {
                conn.Open();
                var team = conn.QuerySingle<TeamDTO>("SELECT Id, Name, SportID, FoundingDate FROM Teams WHERE ID = @id", new { id = teamId });

                team.Players = conn.Query<PlayerDTO>("SELECT Id, FirstName, LastName, DateOfBirth, TeamId FROM Players WHERE TeamId = @ID", new { ID = teamId }).ToList();
            }
            watch.Stop();
            return watch.ElapsedMilliseconds;
        }

        public long GetTeamRostersForSport(int sportId)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            using (SqlConnection conn = new SqlConnection(ConnectionString.DemoDbConnectionString))
            {
                conn.Open();
                var teams = conn.Query<TeamDTO>("SELECT ID, Name, SportID, FoundingDate FROM Teams WHERE SportID = @ID", new { ID = sportId });

                var teamIDs = teams.Select(x => x.Id).ToList();

                var players = conn.Query<PlayerDTO>("SELECT ID, FirstName, LastName, DateOfBirth, TeamID FROM Players WHERE TeamID IN @IDs", new { IDs = teamIDs });

                foreach (var team in teams)
                {
                    team.Players = players.Where(x => x.TeamId == team.Id).ToList();
                }
            }
            watch.Stop();
            return watch.ElapsedMilliseconds;
        }
    }
}
