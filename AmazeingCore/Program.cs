using System;
using System.Collections.Generic;
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
            await ConsoleLogging.Client_Info(client);

            var mazesList = (await client.AllMazes()).OrderBy(x=>x.TotalTiles).ToList();

            mazesList.ForEach(async maze =>
            {
                ConsoleLogging.Mazes_Info(mazesList);
                await Traverse.Start(maze);
            });

            Console.WriteLine("You have finished all the Mazes:\n");
            await ConsoleLogging.Client_Info(client);
            await client.ExitMaze();
            Console.ReadLine();
        }

        public static async Task Connection_Initialization()
        {
            var httpClient = new HttpClient();
            var authorization_Key = "HTI Thanks You [e48a]";

            httpClient.DefaultRequestHeaders.Add("Authorization", authorization_Key);
            client = new AmazeingClient("https://maze.hightechict.nl/", httpClient);
            Traverse.Client = client;

            await client.ForgetPlayer();
            Console.WriteLine("About to register client...");
            await client.RegisterPlayer(name: "MohammadReza");
        }

        
    }
}
