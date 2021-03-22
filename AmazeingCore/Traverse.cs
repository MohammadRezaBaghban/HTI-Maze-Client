using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AmazeingCore
{
    public static class Traverse
    {
        public static AmazeingClient Client;
        public static Direction Direction = Direction.Up;
        private static Stack<Direction> _passedDirection;
        private static Stack<Direction> _directionToExit;
        private static Stack<Direction> _directionToCollectionPoint;

        public static async Task Start(MazeInfo maze)
        {
            Console.WriteLine($"Enter to maze: {maze.Name} with {maze.TotalTiles} tiles and {maze.PotentialReward} potential rewards.");
            _directionToCollectionPoint = new Stack<Direction>();
            _directionToExit = new Stack<Direction>();
            _passedDirection = new Stack<Direction>();
            Direction = Direction.Up;

            var currentTile = await Client.EnterMaze(maze.Name);

            do
            {
                var scoreInBag = currentTile.CurrentScoreInBag;
                var scoreInHand = currentTile.CurrentScoreInHand;
                var allPointsPicked = maze.PotentialReward == scoreInBag + scoreInHand;
                bool collectionBackTrack = false, passedBackTrack = false, exitBackTrack = false;


                currentTile = await Try_Collect_Score(currentTile);
                if (await Try_Exit_Maze(currentTile, maze))
                {
                    return;
                }

                Scan_For_Collection_And_Exit_Spots(currentTile);
                ConsoleLogging.CurrentTile_Info(currentTile, maze);

                if (!allPointsPicked)
                {
                    var possibleReward = currentTile.PossibleMoveActions
                        .Where(di => di.RewardOnDestination != 0 || !di.HasBeenVisited)
                        .OrderBy(di => di.RewardOnDestination != 0)
                        .Select(di => di.Direction)
                        .ToList();

                    if (possibleReward.Count != 0)
                    {
                        Direction = possibleReward[0];
                        currentTile = await Client.Move(Direction);
                    }
                    else
                    {
                        //3rd Priority would be back tracking the passed tiles
                        currentTile = await BackTrack(_passedDirection);
                        passedBackTrack = true;
                    }
                }
                else
                {
                    if (scoreInHand != 0)
                    {
                        // Go Collection Points: Scores needs to be transferred to Bag
                        if (_directionToCollectionPoint != null && _directionToCollectionPoint.Count != 0)
                        {
                            collectionBackTrack = true;
                            currentTile = await BackTrack(_directionToCollectionPoint);
                        }
                    }
                    else
                    {
                        // Go Exit: All Scored already moved to Bag 
                        if (_directionToExit != null && _directionToExit.Count != 0)
                        {
                            exitBackTrack = true;
                            currentTile = await BackTrack(_directionToExit);
                        }
                        else if (_passedDirection != null && _passedDirection.Count != 0)
                        {
                            passedBackTrack = true;
                            currentTile = await BackTrack(_passedDirection);
                        }
                    }
                }

                if (collectionBackTrack == false) _directionToCollectionPoint?.Push(ReverseDirection(Direction));
                if (exitBackTrack == false) _directionToExit?.Push(ReverseDirection(Direction));
                if (passedBackTrack == false) _passedDirection.Push(ReverseDirection(Direction));

                if (currentTile.CanExitMazeHere) _directionToExit.Clear();
                if (currentTile.CanCollectScoreHere) _directionToCollectionPoint.Clear();

            } while (true);
        }

        private static async Task<PossibleActionsAndCurrentScore> Try_Collect_Score(PossibleActionsAndCurrentScore currentTile)
        {
            if (!currentTile.CanCollectScoreHere || currentTile.CurrentScoreInHand <= 0) return currentTile;
            Console.WriteLine($"Score Collection: {currentTile.CurrentScoreInHand} has been moved to your bag");
            currentTile = await Client.CollectScore();
            return currentTile;
        }

        private static async Task<bool> Try_Exit_Maze(PossibleActionsAndCurrentScore currentTile, MazeInfo maze)
        {
            if (currentTile.CurrentScoreInBag != maze.PotentialReward || !currentTile.CanExitMazeHere) return false;
            Console.WriteLine($"Maze Exit: {maze.Name} with {currentTile.CurrentScoreInBag} score in bag");
            await Client.ExitMaze();
            return true;
        }

        private static async Task<PossibleActionsAndCurrentScore> BackTrack(Stack<Direction> stack)
        {
            Direction = stack.Pop();
            return await Client.Move(Direction);
        }

        public static Direction ReverseDirection(Direction dr) =>
            dr switch
            {
                Direction.Up => Direction.Down,
                Direction.Down => Direction.Up,
                Direction.Right => Direction.Left,
                Direction.Left => Direction.Right,
                _ => dr
            };

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

            if (possibleCollect.Count == 0) return;
            _directionToCollectionPoint = new Stack<Direction>();
            _directionToCollectionPoint.Push(possibleCollect[0]);
        }
    }
}
