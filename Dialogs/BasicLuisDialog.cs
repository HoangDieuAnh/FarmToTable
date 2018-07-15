using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Builder.Location.Bing;
using LuisBot.Services;
using Microsoft.Bot.Connector;
using System.Linq;
using LuisBot.Constants;
using System.Threading;

namespace Microsoft.Bot.Sample.LuisBot
{
    // For more information about this template visit http://aka.ms/azurebots-csharp-luis
    [Serializable]
    public class BasicLuisDialog : LuisDialog<object>
    {
        public BasicLuisDialog() : base(new LuisService(new LuisModelAttribute(
            ConfigurationManager.AppSettings["LuisAppId"], 
            ConfigurationManager.AppSettings["LuisAPIKey"], 
            domain: ConfigurationManager.AppSettings["LuisAPIHostName"])))
        {
        }

        [LuisIntent("None")]
        public async Task NoneIntent(IDialogContext context, LuisResult result)
        {
            var answers = new List<String>() { "Sorry I don't get what you said, please try again",
            "Beg your pardon",
            "I don't understand, you saying...?",
            "Could you please repeat your self?"};

            await ShowLuisResult(context, result, answers);
        }

        // Go to https://luis.ai and create a new intent, then train/publish your luis app.
        // Finally replace "Greeting" with the name of your newly created intent in the following handler
        [LuisIntent("Greeting")]
        public async Task GreetingIntent(IDialogContext context, LuisResult result)
        {
            var answers = new List<String>() { "How are you?",
            "Good evening",
            "Hi, how can I help you?",
            "Hello beautiful"};

            await ShowLuisResult(context, result, answers);
        }

        [LuisIntent("Read QR code")]
        public async Task CancelIntent(IDialogContext context, LuisResult result)
        {
            var answers = new List<String>() { "Please send me your bar code first",
            "Sure, let's start with your bar code first",
            "I can definitely help you with that. First, please send me the item's bar code",
            "Do you have the item's bar code with you? Please send it to me"};

            await ShowLuisResult(context, result, answers);
        }

        [LuisIntent("End conversation")]
        public async Task EndConversation(IDialogContext context, LuisResult result)
        {
            await ShowLuisResult(context, result, new List<string> { "Have a good day"});
        }

        [LuisIntent("AboutBot")]
        public async Task AboutBot(IDialogContext context, LuisResult result)
        {
            var answers = new List<string> { "I am a chatbot that was created on 14/07/2018, using LUIS, BingMapAPI and Microsoft Bot Framework (cool ha!). My one and only purpose is to show you the information of products."};
            await ShowLuisResult(context, result, answers);
        }
        [LuisIntent("Get Location")]
        public async Task GetLocation(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Here you go, I'll send you the map");

            await GetMapLocation(context, "21 Lantana Ave, Richland, Washington 99352, United States");

            await Task.Delay(6000);

            PromptDialog.Choice(
             context: context,
             resume: ChoiceReceivedAsync,
             options: Constants.Ratings.Keys,
             prompt: "Have you used it before? How is the product?",
             retry: "Sorry I didn't get it, please choose again",
             promptStyle: PromptStyle.Auto
             );
        }
        public virtual async Task ChoiceReceivedAsync(IDialogContext context, IAwaitable<string> activity)
        {
            string response = await activity;
            var service = new BlockchainService();
            await service.SetRating(Constants.Ratings[response]);
            await context.PostAsync("Thank you. Is there anything else I can help you with?");
            context.Done(true);
        }

        #region Helpers
        private async Task ShowLuisResult(IDialogContext context, LuisResult result, List<string> answers)
        {
            await context.PostAsync(Constants.GenerateRandom(answers));
            context.Done(true);
        }
        /// <summary>
        /// Creates locations hero cards.
        /// </summary>
        /// <param name="locations">List of the locations.</param>
        /// <param name="alwaysShowNumericPrefix">Indicates whether a list containing exactly one location should have a '1.' prefix in its label.</param>
        /// <param name="locationNames">List of strings that can be used as names or labels for the locations.</param>
        /// <returns>The locations card as a list.</returns>
        private IEnumerable<HeroCard> CreateHeroCards(IList<Location> locations, bool alwaysShowNumericPrefix = false, IList<string> locationNames = null)
        {
            var cards = new List<HeroCard>();
            var geoService = new BingGeoSpatialService(ConfigurationManager.AppSettings["BingAPIKey"]);
            int i = 1;

            foreach (var location in locations)
            {
                string nameString = locationNames == null ? string.Empty : $"{locationNames[i - 1]}: ";
                string locationString = $"{nameString}{location.Address.FormattedAddress}";
                string address = locations.Count > 1 ? $"{i}. {locationString}" : locationString;

                var heroCard = new HeroCard
                {
                    Subtitle = address
                };
                
                var image = new CardImage(url: geoService.GetLocationMapImageUrl(location, i));

                heroCard.Images = new[] { image };

                cards.Add(heroCard);

                i++;
            }

            return cards;
        }

        private async Task GetMapLocation(IDialogContext context, string location)
        {
            var geoService = new BingGeoSpatialService(ConfigurationManager.AppSettings["BingAPIKey"]);
            var locationSet = await geoService.GetLocationsByQueryAsync(location);
            var foundLocations = locationSet?.Locations;

            var locationsCardReply = context.MakeMessage();
            locationsCardReply.Attachments = CreateHeroCards(foundLocations).Select(C => C.ToAttachment()).ToList();
            locationsCardReply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            await context.PostAsync(locationsCardReply);

        }
        #endregion
    }
}