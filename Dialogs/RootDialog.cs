using LuisBot.Services;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Sample.LuisBot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace LuisBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            /* Wait until the first message is received from the conversation and call MessageReceviedAsync 
             *  to process that message. */
            context.Wait(this.MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            /* When MessageReceivedAsync is called, it's passed an IAwaitable<IMessageActivity>. To get the message,
             *  await the result. */
            var message = await result;
            if (message.Attachments?.Count > 0)
            {
                await context.Forward(new BarcodeScanning(), this.LuisResumeAfter, message, CancellationToken.None);
            }
            else
                await context.Forward(new BasicLuisDialog(), this.LuisResumeAfter, message, CancellationToken.None);
        }

        private async Task LuisResumeAfter(IDialogContext context, IAwaitable<object> result)
        {
            context.Wait(MessageReceivedAsync);
        }
        //private async Task BarcodeResumeAfter(IDialogContext context, IAwaitable<object> result)
        //{
        //    PromptDialog.Choice(
        //      context: context,
        //      resume: ChoiceReceivedAsync,
        //      options: Constants.Constants.Ratings.Keys,
        //      prompt: "Have you used it before? How is the product?",
        //      retry: "Sorry I didn't get it, please choose again",
        //      promptStyle: PromptStyle.Auto
        //      );
        //}
        //public virtual async Task ChoiceReceivedAsync(IDialogContext context, IAwaitable<string> activity)
        //{
        //    string response = await activity;
        //    var service = new BlockchainService();
        //    await service.SetRating(Constants.Constants.Ratings[response]);
        //    await context.PostAsync("Thank you. Is there anything else I can help you with?");

        //}
    }
}