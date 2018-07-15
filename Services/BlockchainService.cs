using LuisBot.Models;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace LuisBot.Services
{
    [Serializable]
    public class BlockchainService
    {
        private async Task<Contract> GetBlockchainContract()
        {
            var web3 = new Web3("http://127.0.0.1:7545");
            var senderAddress = ConfigurationManager.AppSettings["SenderAddress"];
            var contractAddress = ConfigurationManager.AppSettings["ContractAddress"];
            var password = "password";

            //var unlockAccountResult =
            var unlockAccount = await web3.Personal.UnlockAccount.SendRequestAsync(senderAddress, password, new HexBigInteger(120));

            return web3.Eth.GetContract(Constants.Constants.ABI, contractAddress);
        }
        public async Task<ItemData> GetItemData(string id)
        {
            var result = new ItemData();
            try
            {
                var contract = await GetBlockchainContract();

                var greet = contract.GetFunction("greet")?.CallAsync<string>();
                var getRatings = contract.GetFunction("getTotalRating")?.CallAsync<int>();
                var getRatingCount = contract.GetFunction("getRatingCount")?.CallAsync<int>();

                await Task.WhenAll(greet, getRatings, getRatingCount);

                result.Data = await greet;
                result.TotalRatings = await getRatings;
                result.RatingCount = await getRatingCount;
                return result;
            }
            catch (Exception e)
            {
                var k = e.Message;
            }
            return result;
        }
        public async Task SetRating(int rating)
        {
            try
            {
                var contract = await GetBlockchainContract();
                var setRatings = contract.GetFunction("setTotalRating");
                var gasLimit = new HexBigInteger(0xC3500);
                var gasPrice = new HexBigInteger(0x4A817C800);
                //new HexBigInteger(UnitConversion.Convert.ToWei(1));
                var gas = await setRatings.EstimateGasAsync(rating);
                await setRatings.SendTransactionAsync(ConfigurationManager.AppSettings["SenderAddress"],gas: gas, value: new HexBigInteger(0), functionInput: rating);
            }
            catch (Exception e)
            {
                var t = e.Message;
            }
        }
    }
}