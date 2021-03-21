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

        static async Task Main(string[] args)
        {
            await Connection_Initialization();
            var mazeInfo = await client.AllMazes();

            var mazeList = mazeInfo.ToList();
            MazeInfo chosenMaze = null;
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
