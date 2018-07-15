using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuisBot.Dialogs
{
    using System;
    using System.Configuration;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using LuisBot.Models;
    using LuisBot.Services;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using Nethereum.Hex.HexTypes;
    using Nethereum.RPC.Eth.DTOs;
    using Nethereum.Web3;

    [Serializable]
    public class BarcodeScanning : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);
        }
        private async Task<ItemData> GetItemData(string id)
        {
            var service = new BlockchainService();
            return await service.GetItemData(id);
        }
        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            try
            {
                var message = await argument;
                if (message.Attachments != null && message.Attachments.Any())
                {
                    var attachment = message.Attachments.First();
                    using (HttpClient httpClient = new HttpClient())
                    {
                        // Skype & MS Teams attachment URLs are secured by a JwtToken, so we need to pass the token from our bot.
                        if ((message.ChannelId.Equals("skype", StringComparison.InvariantCultureIgnoreCase) || message.ChannelId.Equals("msteams", StringComparison.InvariantCultureIgnoreCase))
                            && new Uri(attachment.ContentUrl).Host.EndsWith("skype.com"))
                        {
                            var token = await new MicrosoftAppCredentials().GetTokenAsync();
                            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                        }

                        var responseMessage = await httpClient.GetAsync(attachment.ContentUrl);

                        using (Stream streamToReadFrom = await responseMessage.Content.ReadAsStreamAsync())
                        {

                            var bitMap = new Bitmap(streamToReadFrom);

                            string[] datas = Spire.Barcode.BarcodeScanner.Scan(bitMap);

                            if(datas == null || datas.Length == 0)
                                await context.PostAsync("Hmmmm, it's weird, I can't scan you barcode. Please make sure that it's the correct format and try again");

                            var result = await GetItemData(datas[0]);

                            await context.PostAsync($"Here is some information I found: {result?.Data}");

                            await context.PostAsync($"This item received {result?.RatingCount} ratings with an average of {result?.Average}");
                        }

                        
                    }
                }
                else
                {
                    await context.PostAsync("Something is wrong, I can't read your attachment? could you please send it again?");
                }
            }
            catch(Exception e)
            {

            }
            //context.Wait(MessageReceivedAsync);
            context.Done(true);
        }
    }
}