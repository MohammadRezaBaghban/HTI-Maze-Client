using System.Threading.Tasks;
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
            var _client = await AmazeingCore.Program.Connection_Initialization();
            var clientInfo = _client.GetPlayerInfo();

            ////Assert
            Assert.Equal(playerName, clientInfo.Result.Name);
        }

        
    }
}