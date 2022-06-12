using PerformanceCompareDapperAndEntityFW.DataAccessFW;
using PerformanceCompareDapperAndEntityFW.DTOs;
using PerformanceCompareDapperAndEntityFW.TestData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PerformanceCompareDapperAndEntityFW
{
    class Program
    {
        public static int NumPlayers { get; set; }
        public static int NumTeams { get; set; }
        public static int NumSports { get; set; }
        public static int NumRuns { get; set; }
        static void Main(string[] args)
        {
            char input;
            do
            {
                ShowMenu();
                input = Console.ReadLine().First();
                switch (input)
                {
                    case 'Q':
                        break;
                    case 'T':
                        List<TestResult> testResults = new List<TestResult>();

                        Console.WriteLine("# of Test Runs:");
                        NumRuns = int.Parse(Console.ReadLine());

                        //Gather Details for Test
                        Console.WriteLine("# of Sports per Run: ");
                        NumSports = int.Parse(Console.ReadLine());

                        Console.WriteLine("# of Teams per Sport: ");
                        NumTeams = int.Parse(Console.ReadLine());

                        Console.WriteLine("# of Players per Team: ");
                        NumPlayers = int.Parse(Console.ReadLine());

                        Database.ResetDb();
                        Database.LoadNewDb(NumSports, NumTeams, NumPlayers);

                        for (int i = 0; i < NumRuns + 1; i++)
                        {
                            if (i == 0)
                            {//Discard first runs
                                DataAccessFW.Dapper firstDapperTest = new DataAccessFW.Dapper();
                                RunTests(i, Framework.Dapper, firstDapperTest);

                                EntityFrameworkCore firstEfCoreTest = new EntityFrameworkCore();
                                RunTests(i, Framework.EntityFrameworkCore, firstEfCoreTest);

                                EntityFrameworkCoreWithTracking firstTrackingTest = new EntityFrameworkCoreWithTracking();
                                RunTests(i, Framework.EntityFrameworkCoreWithTracking, firstTrackingTest);
                            }

                            //Run real tests
                            DataAccessFW.Dapper dapperTest = new DataAccessFW.Dapper();
                            testResults.AddRange(RunTests(i, Framework.Dapper, dapperTest));

                            EntityFrameworkCoreWithTracking trackingTest = new EntityFrameworkCoreWithTracking();
                            testResults.AddRange(RunTests(i, Framework.EntityFrameworkCoreWithTracking, trackingTest));

                            EntityFrameworkCore efCoreTest = new EntityFrameworkCore();
                            testResults.AddRange(RunTests(i, Framework.EntityFrameworkCore, efCoreTest));
                        }
                        ProcessResults(testResults);

                        break;
                }
            }
            while (input != 'Q');
        }
        public static List<TestResult> RunTests(int runID, Framework framework, IBasicTestFunctions testFunctions)
        {
            List<TestResult> results = new List<TestResult>();

            TestResult result = new TestResult() { Run = runID, Framework = framework };
            List<long> playerByIDResults = new List<long>();
            for (int i = 1; i <= NumPlayers; i++)
            {
                playerByIDResults.Add(testFunctions.GetPlayerById(i));
            }
            result.PlayerByIDMilliseconds = Math.Round(playerByIDResults.Average(), 3);

            List<long> playersForTeamResults = new List<long>();
            for (int i = 1; i <= NumTeams; i++)
            {
                playersForTeamResults.Add(testFunctions.GetRosterByTeamId(i));
            }
            result.PlayersForTeamMilliseconds = Math.Round(playersForTeamResults.Average(), 3);
            List<long> teamsForSportResults = new List<long>();
            for (int i = 1; i <= NumSports; i++)
            {
                teamsForSportResults.Add(testFunctions.GetTeamRostersForSport(i));
            }
            result.TeamsForSportMilliseconds = Math.Round(teamsForSportResults.Average(), 3);
            results.Add(result);

            return results;
        }

        public static void ProcessResults(List<TestResult> results)
        {
            var groupedResults = results.GroupBy(x => x.Framework);
            foreach (var group in groupedResults)
            {
                Console.WriteLine(group.Key.ToString() + " Results");
                Console.WriteLine("Run #\tPlayer by ID\t\tPlayers per Team\t\tTeams per Sport");
                var orderedResults = group.OrderBy(x => x.Run);
                foreach (var orderResult in orderedResults)
                {
                    Console.WriteLine(orderResult.Run.ToString() + "\t\t" + orderResult.PlayerByIDMilliseconds + "\t\t\t" + orderResult.PlayersForTeamMilliseconds + "\t\t\t" + orderResult.TeamsForSportMilliseconds);
                }
            }
        }

        public static void ShowMenu()
        {
            Console.WriteLine("Please enter one of the following options:");
            Console.WriteLine("Q - Quit");
            Console.WriteLine("T - Run Test");
            Console.WriteLine("Options:");
        }
    }
}
