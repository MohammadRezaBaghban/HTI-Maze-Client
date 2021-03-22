using System;
using System.Collections.Generic;

namespace AmazeingCore
{
    public static class ConsoleLogging
    {
        public static void Client_Info(PlayerInfo clientInfo)
        {
            Console.WriteLine("\n==== Client Info ====\n\n" +
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
            Console.WriteLine("\n==== Mazes Info ====\n" +
                              $"{row_Separator}\n" + 
                              $"| {"Number of Tiles",-20}| {"Potential Reward",-20}| {"Maze Name",-35}|\n" +
                              row_Separator);

            mazes.ForEach(Maze_Info);
            Console.WriteLine($"{row_Separator}\n");
        }

        public static void Maze_Info(MazeInfo maze) =>
            Console.WriteLine($"| {maze.TotalTiles,-20}| {maze.PotentialReward,-20}| {maze.Name,-35}|");

        public static void CurrentTile_Info(PossibleActionsAndCurrentScore currentTile, MazeInfo maze, Direction lastMove)
        {
            var mazeScore = maze.PotentialReward;
            var scoreInBag = currentTile.CurrentScoreInBag;
            var scoreInHand = currentTile.CurrentScoreInHand;
            var remaining = mazeScore - (scoreInHand + scoreInBag);

            Console.Write($"\nLast Move: {lastMove} | Score Hand + Bag + Remain = Total: " +
                          $"({scoreInHand} + {scoreInBag} + {remaining} = {mazeScore})" +
                          $"\nMaze \"{maze.Name}\"  | Tile Type: {TileType(currentTile)} | ");

            PossibleMove_Info(currentTile);
        }

        public static void PossibleMove_Info(PossibleActionsAndCurrentScore currentTile )
        {
            var counter = 0;
            Console.WriteLine("Surrounding Tiles:");
            foreach (var mva in currentTile.PossibleMoveActions)
            {
                Console.WriteLine($"\t{counter++}- {mva.Direction} | Reward:{mva.RewardOnDestination} | Type:{TileType(currentTile)} | HasVisited: {mva.HasBeenVisited} ");
            }
        }

        public static string TileType(PossibleActionsAndCurrentScore tile) =>
            (tile.CanCollectScoreHere) ? "Collection Spot" : (tile.CanExitMazeHere) ? "Exit Spot" : "Normal";
    }
}
