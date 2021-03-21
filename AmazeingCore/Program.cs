using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AmazeingCore
{
    class Program
    {
        private static AmazeingClient client;
        private static Random rand = new Random();
        private static MazeInfo chosenMaze = null;

        static async Task Main(string[] args)
        {
            await Connection_Initialization();
            await ConsoleLogging.Print_Client_Info(client);

            var mazesList = (await client.AllMazes()).OrderBy(x=>x.TotalTiles).ToList();
            ConsoleLogging.Print_Mazes_Info(mazesList);

        }

        public static async Task Connection_Initialization()
        {
            var httpClient = new HttpClient();
            var authorization_Key = "HTI Thanks You [e48a]";

            httpClient.DefaultRequestHeaders.Add("Authorization", authorization_Key);
            client = new AmazeingClient("https://maze.hightechict.nl/", httpClient);

            await client.ForgetPlayer();
            Console.WriteLine("About to register client...");
            await client.RegisterPlayer(name: "MohammadReza");
        }

    }
}
