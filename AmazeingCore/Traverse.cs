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
        public static PossibleActionsAndCurrentScore CurrentTile;
        private static Dictionary<string, Stack<Direction>> _backtrackStacks;

        public static async Task Start(MazeInfo maze)
        {
            Console.WriteLine($"Enter to maze: {maze.Name} with {maze.TotalTiles} tiles and {maze.PotentialReward} potential rewards.");
            _backtrackStacks = new Dictionary<string, Stack<Direction>>
            {
                {"Collection",new Stack<Direction>()},
                {"Exit", new Stack<Direction>() },
                {"Pass",new Stack<Direction>()}
            };

            CurrentTile = await Client.EnterMaze(maze.Name);

            do
            {
                try
                {
                    var scoreInBag = CurrentTile.CurrentScoreInBag;
                    var scoreInHand = CurrentTile.CurrentScoreInHand;
                    var allPointsPicked = maze.PotentialReward == scoreInBag + scoreInHand;
                    bool collectionBackTrack = false, passedBackTrack = false, exitBackTrack = false;

                    CurrentTile = await Scan_For_Collection_And_Exit_Spots(CurrentTile);
                    if (await Try_Exit_Maze(CurrentTile, maze)) return;

                    if (!allPointsPicked)
                    {
                        var possibleReward = CurrentTile.PossibleMoveActions
                            .Where(di => di.RewardOnDestination != 0 || !di.HasBeenVisited)
                            .OrderBy(di => di.RewardOnDestination != 0)
                            .Select(di => di.Direction).ToList();

                        if (possibleReward.Count != 0)
                        {
                            Direction = possibleReward[0];
                            CurrentTile = await Client.Move(Direction);
                        }
                        else
                        {
                            CurrentTile = await BackTrack(_backtrackStacks["Pass"]);
                            passedBackTrack = true;
                        }
                    }
                    else
                    {
                        if (scoreInHand != 0)
                        {
                            // Go Collection Points: Scores needs to be transferred to Bag
                            if (_backtrackStacks["Collection"] != null && _backtrackStacks["Collection"].Count != 0)
                            {
                                collectionBackTrack = true;
                                CurrentTile = await BackTrack(_backtrackStacks["Collection"]);
                            }
                        }
                        else
                        {
                            // Go Exit: All Scored already moved to Bag 
                            if (_backtrackStacks["Exit"] != null && _backtrackStacks["Exit"].Count != 0)
                            {
                                exitBackTrack = true;
                                CurrentTile = await BackTrack(_backtrackStacks["Exit"]);
                            }
                            else if (_backtrackStacks["Pass"] != null && _backtrackStacks["Pass"].Count != 0)
                            {
                                passedBackTrack = true;
                                CurrentTile = await BackTrack(_backtrackStacks["Pass"]);
                            }
                        }
                    }
                    ConsoleLogging.CurrentTile_Info(CurrentTile, maze, Direction);

                    //Updating Stacks with taken movement
                    if (CurrentTile.CanExitMazeHere) _backtrackStacks["Exit"].Clear();
                    if (CurrentTile.CanCollectScoreHere) _backtrackStacks["Collection"].Clear();
                    if (collectionBackTrack == false) _backtrackStacks["Collection"]?.Push(ReverseDirection(Direction));
                    if (exitBackTrack == false) _backtrackStacks["Exit"]?.Push(ReverseDirection(Direction));
                    if (passedBackTrack == false) _backtrackStacks["Pass"].Push(ReverseDirection(Direction));

                }
                catch (Exception e)
                {
                    ConsoleLogging.ExceptionHandler(e, "Trying to Make a move");
                    CurrentTile = await BackTrack(_backtrackStacks["Pass"]);
                }
            } while (true);
        }

        /// <summary>
        /// Check if there is a collection or Exit point in approximation of current tile and collect scores
        /// </summary>
        private static async Task<PossibleActionsAndCurrentScore> Scan_For_Collection_And_Exit_Spots(PossibleActionsAndCurrentScore currentTile)
        {
            currentTile = await Try_Collect_Score();

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
                _backtrackStacks["Exit"] = new Stack<Direction>();
                _backtrackStacks["Exit"].Push(possibleExit[0]);
            }

            if (possibleCollect.Count == 0) return currentTile;
            _backtrackStacks["Collection"] = new Stack<Direction>();
            _backtrackStacks["Collection"].Push(possibleCollect[0]);
            return currentTile;
        }

        private static async Task<PossibleActionsAndCurrentScore> Try_Collect_Score()
        {
            try
            {
                if (!CurrentTile.CanCollectScoreHere || CurrentTile.CurrentScoreInHand <= 0) return CurrentTile;
                Console.WriteLine($"Score Collection: {CurrentTile.CurrentScoreInHand} has been moved to your bag");
                CurrentTile = await Client.CollectScore();
                return CurrentTile;
            }
            catch (Exception e)
            {
                ConsoleLogging.ExceptionHandler(e, "Trying to Collect score");
                return CurrentTile;
            }
        }

        private static async Task<bool> Try_Exit_Maze(PossibleActionsAndCurrentScore tile, MazeInfo maze)
        {
            try
            {
                if (tile.CurrentScoreInBag != maze.PotentialReward || !tile.CanExitMazeHere) return false;
                Console.WriteLine($"Maze Exit: {maze.Name} with {tile.CurrentScoreInBag} score in bag");
                await Client.ExitMaze();
                return true;
            }
            catch (Exception e)
            {
                ConsoleLogging.ExceptionHandler(e, $"Trying to Exit Maze \"{maze.Name}\"");
                return false;
            }
        }

        private static async Task<PossibleActionsAndCurrentScore> BackTrack(Stack<Direction> stack)
        {
            try
            {
                Direction = stack.Pop();
                var possibleMovements = CurrentTile.PossibleMoveActions.Select(x => x.Direction).ToList();
                if (possibleMovements.Contains(Direction))
                    return await Client.Move(Direction);
                return await Client.Move(possibleMovements[0]);
            }
            catch (Exception e)
            {
                ConsoleLogging.ExceptionHandler(e, $"Poping from backtracking {stack.GetType().Name} stacks");
                throw;
            }
        }

        public static Direction ReverseDirection(Direction dr) => dr switch
        {
            Direction.Up => Direction.Down,
            Direction.Down => Direction.Up,
            Direction.Right => Direction.Left,
            Direction.Left => Direction.Right,
            _ => dr
        };
    }
}