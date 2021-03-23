using System.Linq;
using System.Threading.Tasks;
using AmazeingCore;
using Xunit;

namespace UnitTests
{
    public class UnitTests
    {


        [Theory]
        [InlineData("MohammadReza")]
        public async Task Program_ConnectionInitialization_CorrectPlayerNameFromAPI(string playerName)
        {
            //Arrange & Act 
            var _client = await AmazeingCore.Program.Connection_Initialization(playerName);
            var clientInfo = _client.GetPlayerInfo();

            ////Assert
            Assert.Equal(playerName, clientInfo.Result.Name);
        }

        [Theory]
        [InlineData("Exit")]
        [InlineData("Reverse")]
        [InlineData("PacMan")]
        public async Task Traverse_OnSpecificMaze_AllPointCollected(string mazeName)
        {
            //Arrange
            Traverse.Client = await AmazeingCore.Program.Connection_Initialization("Someone");
            var mazeList = await Traverse.Client.AllMazes();
            var maze = mazeList.Single(m => m .Name == mazeName);
            
            //Act
            await Traverse.Start(maze);
            var playerScore = (await Traverse.Client.GetPlayerInfo()).PlayerScore;

            //Assert
            Assert.Equal(maze.PotentialReward, playerScore);
        }


    }
}