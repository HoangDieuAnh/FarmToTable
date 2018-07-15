using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuisBot.Services
{
    using System;
    using System.Configuration;
    using System.Globalization;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Internals.Fibers;
    using Microsoft.Bot.Builder.Location.Bing;
    using Newtonsoft.Json;

    [Serializable]
    internal sealed class BingGeoSpatialService
    {
        private static readonly string FormCode = "BTCTRL";
        private readonly static string FindByQueryApiUrl = $"https://dev.virtualearth.net/REST/v1/Locations?form={FormCode}&q={{0}}&key={{1}}";
        private readonly static string FindByPointUrl = $"https://dev.virtualearth.net/REST/v1/Locations/{{0}},{{1}}?form={FormCode}&key={{2}}";
        private readonly static string ImageUrlByPoint = $"https://dev.virtualearth.net/REST/V1/Imagery/Map/Road/{{0}},{{1}}/15?form={FormCode}&mapSize=500,280&pp={{0}},{{1}};1;{{2}}&dpi=1&logo=always&key={{3}}";
        private readonly static string ImageUrlByBBox = $"https://dev.virtualearth.net/REST/V1/Imagery/Map/Road?form={FormCode}&mapArea={{0}},{{1}},{{2}},{{3}}&mapSize=500,280&pp={{4}},{{5}};1;{{6}}&dpi=1&logo=always&key={{7}}";

        private readonly string apiKey = ConfigurationManager.AppSettings["BingAPIKey"];

        internal BingGeoSpatialService(string apiKey)
        {
            SetField.NotNull(out this.apiKey, nameof(apiKey), apiKey);
        }

        public async Task<LocationSet> GetLocationsByQueryAsync(string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                throw new ArgumentNullException(nameof(address));
            }

            return await this.GetLocationsAsync(string.Format(FindByQueryApiUrl, Uri.EscapeDataString(address), this.apiKey));
        }

        public async Task<LocationSet> GetLocationsByPointAsync(double latitude, double longitude)
        {
            return await this.GetLocationsAsync(
                string.Format(CultureInfo.InvariantCulture, FindByPointUrl, latitude, longitude, this.apiKey));
        }

        public string GetLocationMapImageUrl(Location location, int? index = null)
        {
            if (location == null)
            {
                throw new ArgumentNullException(nameof(location));
            }

            var point = location.Point;
            if (point == null)
            {
                throw new ArgumentNullException(nameof(point));
            }

            if (location.BoundaryBox != null && location.BoundaryBox.Count >= 4)
            {
                return string.Format(
                    CultureInfo.InvariantCulture,
                    ImageUrlByBBox,
                    location.BoundaryBox[0],
                    location.BoundaryBox[1],
                    location.BoundaryBox[2],
                    location.BoundaryBox[3],
                    point.Coordinates[0],
                    point.Coordinates[1],
                    index,
                    apiKey);
            }
            else
            {
                return string.Format(
                    CultureInfo.InvariantCulture,
                    ImageUrlByPoint,
                    point.Coordinates[0],
                    point.Coordinates[1],
                    index,
                    apiKey);
            }
        }

        private async Task<LocationSet> GetLocationsAsync(string url)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetStringAsync(url);
                var apiResponse = JsonConvert.DeserializeObject<LocationApiResponse>(response);

                // TODO: what is the right logic for picking a location set?
                return apiResponse.LocationSets?.FirstOrDefault();
            }
        }
    }


    [Serializable]
    internal class LocationApiResponse
    {
        [JsonProperty(PropertyName = "authenticationResultCode")]
        public string AuthenticationResultCode { get; set; }

        [JsonProperty(PropertyName = "brandLogoUri")]
        public string BrandLogoUri { get; set; }

        [JsonProperty(PropertyName = "copyright")]
        public string Copyright { get; set; }

        [JsonProperty(PropertyName = "resourceSets")]
        public List<LocationSet> LocationSets { get; set; }

        [JsonProperty(PropertyName = "statusCode")]
        public int SatusCode { get; set; }

        [JsonProperty(PropertyName = "statusDescription")]
        public string StatusDescription { get; set; }

        [JsonProperty(PropertyName = "traceId")]
        public string TraceId { get; set; }
    }


    [Serializable]
    public class LocationSet
    {
        /// <summary>
        /// The total estimated results.
        /// </summary>
        [JsonProperty(PropertyName = "estimatedTotal")]
        public int EstimatedTotal { get; set; }

        /// <summary>
        /// The location list.
        /// </summary>
        [JsonProperty(PropertyName = "resources")]
        public List<Location> Locations { get; set; }
    }
}
