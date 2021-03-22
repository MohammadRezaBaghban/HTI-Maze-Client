using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AmazeingCore
{
    class Program
    {
        private static AmazeingClient _client;
        private static async Task<PlayerInfo> ClientInfo() => await _client.GetPlayerInfo();

        private static async Task Main()
        {
            await Connection_Initialization();
            await Traverse_Mazes();
            Console.ReadLine();
        }

        public static async Task Traverse_Mazes()
        {
            var mazesList = (await _client.AllMazes()).OrderBy(x => x.TotalTiles).ToList();
            foreach (var maze in mazesList)
            {
                ConsoleLogging.Mazes_Info(mazesList);
                await Traverse.Start(maze);
            }

            Console.WriteLine("You have finished all the Mazes:\n");
            ConsoleLogging.Client_Info(await ClientInfo());
        }

        public static async Task Connection_Initialization()
        {
            var httpClient = new HttpClient();
            var authorization_Key = "HTI Thanks You [e48a]";

            httpClient.DefaultRequestHeaders.Add("Authorization", authorization_Key);
            _client = new AmazeingClient("https://maze.hightechict.nl/", httpClient);
            Traverse.Client = _client;

            await _client.ForgetPlayer();
            await _client.RegisterPlayer(name: "MohammadReza");
            
            Console.WriteLine("About to register client...");
            ConsoleLogging.Client_Info(await ClientInfo());
        }

    }
}
