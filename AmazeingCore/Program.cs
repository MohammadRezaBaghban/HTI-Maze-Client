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
            await ConsoleLogging.Print_Client_Info(client);

            var mazesList = (await client.AllMazes()).OrderBy(x=>x.TotalTiles).ToList();

            mazesList.ForEach(async maze =>
            {
                ConsoleLogging.Print_Mazes_Info(mazesList);
                await Traverse_Maze(maze);
            });

            Console.WriteLine("You have finished all the Mazes:\n");
            await ConsoleLogging.Print_Client_Info(client);
            await client.ExitMaze();
            Console.ReadLine();
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

        private static async Task Traverse_Maze(MazeInfo maze)
        {
            Console.WriteLine($"Enter to maze: {maze.Name} with {maze.TotalTiles} tiles and {maze.PotentialReward} potential rewards.");
            var passedDirection = new Stack<Direction>();
            var direction_To_CollectionPoint = new Stack<Direction>();
            var direction_To_Exit = new Stack<Direction>();

            var possibleActions = await client.EnterMaze(maze.Name);

            do
            {
                await Try_Collect_Score(possibleActions);
                await Try_Exit_Maze(possibleActions, maze);


            } while (true);
        }

        private static async Task<PossibleActionsAndCurrentScore> Try_Collect_Score(PossibleActionsAndCurrentScore currentTile)
        {
            if (!currentTile.CanCollectScoreHere || currentTile.CurrentScoreInHand <= 0) return currentTile;
            Console.WriteLine($"Score Collection: {currentTile.CurrentScoreInHand} has been moved to your bag");
            currentTile = await client.CollectScore();
            return currentTile;
        }

        private static async Task Try_Exit_Maze(PossibleActionsAndCurrentScore currentTile, MazeInfo maze)
        {
            if (currentTile.CurrentScoreInBag == maze.PotentialReward && currentTile.CanExitMazeHere)
            {
                Console.WriteLine($"Maze Exit: {maze.Name} with {currentTile.CurrentScoreInBag} score in bag");
                await client.EnterMaze(maze.Name);
            }
        }
    }
}
