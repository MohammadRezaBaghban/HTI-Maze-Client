using System;
using System.Collections.Generic;
using System.Text;

namespace AmazeingCore.Helpers
{
    public class Console_Log
    {
        public static void GenerateConsoleLogging(PossibleActionsAndCurrentScore possibleAction, MazeInfo maze)
        {
            string tileType =
                (possibleAction.CanCollectScoreHere) ? "Collection Spot" :
                (possibleAction.CanExitMazeHere) ? "Exit Spot" :
                "Normal Tile";

            Console.WriteLine($"\nInfo at move:" +
                              $"\nMaze Name: {maze.Name} - Tiles: {maze.TotalTiles}" +
                              $"\nScore In Hand: {possibleAction.CurrentScoreInHand}" +
                              $"\nScore In Bag: {possibleAction.CurrentScoreInBag}/{maze.PotentialReward}" +
                              $"\nTile Type: {tileType}\n");


            var counter = 0;
            Console.WriteLine($"\nSurronding Tiles:");
            foreach (var mva in possibleAction.PossibleMoveActions)
            {
                string type = (mva.AllowsScoreCollection) ? "Collection Spot" : (mva.AllowsExit) ? "Exit Spot" : (mva.IsStart) ? "Start Spot" : "Unkown";
                Console.WriteLine($"{counter++}- {mva.Direction} | RewardAmount:{mva.RewardOnDestination} | Type:{type} | HasVisited: {mva.HasBeenVisited} ");
            }
        }
    }
}
