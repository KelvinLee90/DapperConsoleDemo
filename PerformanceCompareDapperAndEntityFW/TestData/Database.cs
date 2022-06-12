using Microsoft.EntityFrameworkCore;
using PerformanceCompareDapperAndEntityFW.DTOs;
using PerformanceCompareDapperAndEntityFW.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceCompareDapperAndEntityFW.TestData
{
    public static class Database
    {
        public static void ResetDb()
        {
            using (SportDbEFCoreContext context = new SportDbEFCoreContext(GetOptions()))
            {
                context.Database.ExecuteSqlRaw("Delete from Players");
                context.Database.ExecuteSqlRaw("DBCC CHECKIDENT('Players', RESEED, 0)");
                context.Database.ExecuteSqlRaw("Delete from Teams");
                context.Database.ExecuteSqlRaw("DBCC CHECKIDENT('Teams', RESEED, 0)");
                context.Database.ExecuteSqlRaw("Delete from Sports");
                context.Database.ExecuteSqlRaw("DBCC CHECKIDENT('Sports', RESEED, 0)");
            }
        }

        public static void LoadNewDb(int numSports, int numTeams, int numPlayers)
        {
            // Add sports data
            List<SportDTO> sports = InitialDB.GenerateSports(numSports);
            var listSports = AddSportsData(sports);

          
            List<TeamDTO> teams = new List<TeamDTO>();
            List<PlayerDTO> players = new List<PlayerDTO>();

            //Add teams data
            foreach (var sport in listSports)
            {
                var newTeams = InitialDB.GenerateTeams(sport.Id, numTeams);
                teams.AddRange(newTeams);
            }

            var listTeams = AddTeamsData(teams);

            //Add  players data
            foreach (var team in listTeams)
            {
                var newPlayers = InitialDB.GeneratePlayers(team.Id, numPlayers);
                players.AddRange(newPlayers);
            }
            AddPlayersData(players);
        }

        private static void AddPlayersData(List<PlayerDTO> players)
        {
            using (SportDbEFCoreContext context = new SportDbEFCoreContext(GetOptions()))
            {
                foreach (var player in players)
                {
                    context.Players.Add(new Player()
                    {
                        //Id = player.Id,
                        FirstName = player.FirstName,
                        LastName = player.LastName,
                        DateOfBirth = player.DateOfBirth,
                        TeamId = player.TeamId
                    });
                }
                context.SaveChanges();
            }
        }

        private static List<Team> AddTeamsData(List<TeamDTO> teams)
        {
            using (SportDbEFCoreContext context = new SportDbEFCoreContext(GetOptions()))
            {
                foreach (var team in teams)
                {
                    context.Teams.Add(new Team()
                    {
                        //Id = team.Id,
                        Name = team.Name,
                        SportId = team.SportId,
                        FoundingDate = team.FoundingDate
                    });
                }
                context.SaveChanges();
                return context.Teams.ToList();
            }
        }

        private static List<Sport> AddSportsData(List<SportDTO> sports)
        {
            using (SportDbEFCoreContext context = new SportDbEFCoreContext(GetOptions()))
            {
                foreach (var sport in sports)
                {
                    context.Sports.Add(new Sport()
                    {
                        //Id = sport.Id,
                        Name = sport.Name
                    });
                }
                context.SaveChanges();
                return context.Sports.ToList();
            }
        }

        public static DbContextOptions GetOptions()
        {
            DbContextOptionsBuilder builder = new DbContextOptionsBuilder();
            builder.UseSqlServer(ConnectionString.DemoDbConnectionString);
            return builder.Options;
        }
    }
}
