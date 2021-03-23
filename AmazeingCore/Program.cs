using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AmazeingCore
{
    public class Program
    {
        private static AmazeingClient _client;
        private static async Task<PlayerInfo> ClientInfo() => await _client.GetPlayerInfo();

        private static async Task Main()
        {
            try
            {
                Traverse.Client = await Connection_Initialization("MohammadReza");
                await Traverse_Mazes();
                Console.ReadLine();
            }
            catch (Exception e)
            {
                ConsoleLogging.ExceptionHandler(e, $"Unknown Upper Level Case");
            }
        }

        public static async Task Traverse_Mazes()
        {
            var mazesList = (await _client.AllMazes()).OrderBy(x => x.TotalTiles).ToList();
            foreach (var maze in mazesList)
            {
                try
                {
                    ConsoleLogging.Mazes_Info(mazesList);
                    await Traverse.Start(maze);
                }
                catch (Exception e)
                {
                    ConsoleLogging.ExceptionHandler(e, $"Traversing Maze \"{maze.Name}\"");
                }
            }

            Console.WriteLine("You have finished all the Mazes:\n");
            ConsoleLogging.Client_Info(await ClientInfo());
        }

        public static async Task<AmazeingClient> Connection_Initialization(string playerName, string token = null)
        {
            try
            {
                var httpClient = new HttpClient();
                var authorization_Key = token ?? "HTI Thanks You [e48a]";

                httpClient.DefaultRequestHeaders.Add("Authorization", authorization_Key);
                _client = new AmazeingClient("https://maze.hightechict.nl/", httpClient);

                await _client.ForgetPlayer();
                await _client.RegisterPlayer(name: playerName);

                Console.WriteLine("About to register client...");
                ConsoleLogging.Client_Info(await ClientInfo());
                return _client;
            }
            catch (Exception e)
            {
                ConsoleLogging.ExceptionHandler(e, $"Connection Initialization Phase");
                return null;
            }
        }
    }
}
