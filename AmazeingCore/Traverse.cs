using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AmazeingCore
{
    public static class Traverse
    {
        public static AmazeingClient Client;
        private static Stack<Direction> _passedDirection;
        private static Stack<Direction> _directionToExit;
        private static Stack<Direction> _directionToCollectionPoint;

        public static async Task Start(MazeInfo maze)
        {
            Console.WriteLine($"Enter to maze: {maze.Name} with {maze.TotalTiles} tiles and {maze.PotentialReward} potential rewards.");
            _directionToCollectionPoint = new Stack<Direction>();
            _directionToExit = new Stack<Direction>();
            _passedDirection = new Stack<Direction>();

            var currentTile = await Client.EnterMaze(maze.Name);

            do
            {
                var scoreInBag = currentTile.CurrentScoreInBag;
                var scoreInHand = currentTile.CurrentScoreInHand;
                var allPointsPicked = maze.PotentialReward == scoreInBag + scoreInHand;
                bool CollectionBackTrack = false, PassedBackTrack = false, ExitBackTrack = false;


                currentTile = await Try_Collect_Score(currentTile);
                await Try_Exit_Maze(currentTile, maze);

                Scan_For_Collection_And_Exit_Spots(currentTile);
                ConsoleLogging.CurrentTile_Info(currentTile, maze);

            } while (true);
        }

        private static async Task<PossibleActionsAndCurrentScore> Try_Collect_Score(PossibleActionsAndCurrentScore currentTile)
        {
            if (!currentTile.CanCollectScoreHere || currentTile.CurrentScoreInHand <= 0) return currentTile;
            Console.WriteLine($"Score Collection: {currentTile.CurrentScoreInHand} has been moved to your bag");
            currentTile = await Client.CollectScore();
            return currentTile;
        }

        private static async Task Try_Exit_Maze(PossibleActionsAndCurrentScore currentTile, MazeInfo maze)
        {
            if (currentTile.CurrentScoreInBag == maze.PotentialReward && currentTile.CanExitMazeHere)
            {
                Console.WriteLine($"Maze Exit: {maze.Name} with {currentTile.CurrentScoreInBag} score in bag");
                await Client.EnterMaze(maze.Name);
            }
        }

        

        /// <summary>
        /// Check if there is a collection or Exit point in approximation of current tile and store pass to them
        /// </summary>
        private static void Scan_For_Collection_And_Exit_Spots(PossibleActionsAndCurrentScore currentTile)
        {
            var possibleExit = currentTile.PossibleMoveActions
                .Where(di => di.AllowsExit)
                .Select(di => di.Direction)
                .ToList();

            var possibleCollect = currentTile.PossibleMoveActions
                .Where(di => di.AllowsScoreCollection)
                .Select(di => di.Direction)
                .ToList();

            if (possibleExit.Count != 0)
            {
                _directionToExit = new Stack<Direction>();
                _directionToExit.Push(possibleExit[0]);
            }

            if (possibleCollect.Count != 0)
            {
                _directionToCollectionPoint = new Stack<Direction>();
                _directionToCollectionPoint.Push(possibleCollect[0]);
            }
        }
    }
}
