using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AmazeingCore
{
    public static class ConsoleLogging
    {
        public static async Task Print_Client_Info(AmazeingClient client)
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

        public static void Print_Mazes_Info(List<MazeInfo> mazes)
        {
            var row_Separator = $"{new string('-', 82)}";
            Console.WriteLine($"\n==== " + $"Mazes Info ====\n" +
                              $"{row_Separator}\n" + 
                              $"| {"Number of Tiles",-20}| {"Potential Reward",-20}| {"Maze Name",-35}|\n" +
                              row_Separator);

            mazes.ForEach(Print_MazeInfo);
            Console.WriteLine($"{row_Separator}\n");

        }

        public static void Print_MazeInfo(MazeInfo maze) =>
            Console.WriteLine($"| {maze.TotalTiles,-20}| {maze.PotentialReward,-20}| {maze.Name,-35}|");
    }
}
