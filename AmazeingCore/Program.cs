using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AmazeingCore.Helpers;

namespace AmazeingCore
{
    class Program
    {
        private static AmazeingClient client;
        private static Random rand = new Random();

        static async Task Main(string[] args)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", "HTI Thanks You [e48a]");
            client = new AmazeingClient("https://maze.hightechict.nl/", httpClient);

            await client.ForgetPlayer();
            Console.WriteLine("About to register...");
            await client.RegisterPlayer(name: "MohammadReza");

            var mazeInfo = await client.AllMazes();
            var mazeList = mazeInfo.ToList();

            MazeInfo chosenMaze = null;
        }
    }
}
