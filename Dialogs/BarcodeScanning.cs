using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuisBot.Dialogs
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;

    [Serializable]
    internal class BarcodeScanning : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);
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
                            //streamToReadFrom.CopyTo(ms);
                            //FileStream file = new FileStream(@"C:\Users\nhimb\thisFile.jpg", FileMode.Create, FileAccess.Write);
                            //streamToReadFrom.CopyTo(file);
                            //file.Close();

                            string[] datas = Spire.Barcode.BarcodeScanner.Scan(bitMap);
                            var text = datas[0];



                        }

                        await context.PostAsync($"Attachment of {attachment.ContentType} type and size of bytes received.");
                    }
                }
                else
                {
                    await context.PostAsync("Hi there! I'm a bot created to show you how I can receive message attachments, but no attachment was sent to me. Please, try again sending a new message including an attachment.");
                }
            }
            catch(Exception e)
            {

            }
            context.Wait(this.MessageReceivedAsync);
        }
    }
}