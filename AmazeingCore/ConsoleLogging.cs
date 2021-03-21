using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AmazeingCore
{
    public static class ConsoleLogging
    {
        public static async Task Client_Info(AmazeingClient client)
        {
            var clientInfo = await client.GetPlayerInfo();
            Console.WriteLine($"\n==== " + $"Client Info ====\n\n" +
                              $"\tPlayer Id: {clientInfo.PlayerId}\n" +
                              $"\tPlayer Name: {clientInfo.Name}\n" +
                              $"\tCurrent Score: {clientInfo.PlayerScore}\n" +
                              $"\tStatus in Current {clientInfo.Maze ?? "---"} Maze:\n" +
                              $"\t\tScore in Hand{clientInfo.MazeScoreInHand}\n" +
                              $"\t\tScore in Bag{clientInfo.MazeScoreInBag}\n\n"
            );
        }

        public static void Mazes_Info(List<MazeInfo> mazes)
        {
            var row_Separator = $"{new string('-', 82)}";
            Console.WriteLine($"\n==== " + $"Mazes Info ====\n" +
                              $"{row_Separator}\n" + 
                              $"| {"Number of Tiles",-20}| {"Potential Reward",-20}| {"Maze Name",-35}|\n" +
                              row_Separator);

            mazes.ForEach(Maze_Info);
            Console.WriteLine($"{row_Separator}\n");

        }

        public static void Maze_Info(MazeInfo maze) =>
            Console.WriteLine($"| {maze.TotalTiles,-20}| {maze.PotentialReward,-20}| {maze.Name,-35}|");

        public static void CurrentTile_Info(PossibleActionsAndCurrentScore currentTile, MazeInfo maze)
        {
            string tileType =
                (currentTile.CanCollectScoreHere) ? "Collection Spot" :
                (currentTile.CanExitMazeHere) ? "Exit Spot" :
                "Normal Tile";

            var scoreInHand = currentTile.CurrentScoreInHand;
            var scoreInBag = currentTile.CurrentScoreInBag;
            var mazeScore = maze.PotentialReward;

            Console.WriteLine($"\nInfo at move in Maze {maze.Name} with total {maze.TotalTiles} tiles:" +
                              $"\nScore In (Hand | Bag | Total): " + $"({scoreInHand} | {scoreInBag} | {mazeScore})" +
                              $"\nScore Remain to find: {mazeScore - (scoreInHand + scoreInBag)}" +
                              $"\nTile Type: {tileType}\n");

            PossibleMove_Info(currentTile);
        }

        public static void PossibleMove_Info(PossibleActionsAndCurrentScore currentTile )
        {
            var counter = 0;
            Console.WriteLine($"\nSurrounding Tiles:");
            foreach (var mva in currentTile.PossibleMoveActions)
            {
                string type = (mva.AllowsScoreCollection) ? "Collection Spot" : (mva.AllowsExit) ? "Exit Spot" : (mva.IsStart) ? "Start Spot" : "Unkown";
                Console.WriteLine($"\t{counter++}- {mva.Direction} | Reward:{mva.RewardOnDestination} | Type:{type} | HasVisited: {mva.HasBeenVisited} ");
            }
        }
    }
}
