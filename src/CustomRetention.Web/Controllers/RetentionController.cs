using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using CustomRetention.Web.Models;
using NuGet;
using HttpClient = System.Net.Http.HttpClient;

namespace CustomRetention.Web.Controllers
{
    public class RetentionController : ApiController
    {
        // POST /api/retention
        public async Task<HttpResponseMessage> Post([FromBody]WebHookEvent payload)
        {
            // The logic in this method will do the following:
            // 1) Find all packages with the same identifier as the package that was added to the originating feed
            // 2) Enforce the following policy: only the 5 latest (stable) packages matching the same minor version may remain on the feed. Others should be removed.
            string feedUrl = payload.Payload.FeedUrl;

            // Note: the following modifies NuGet's client so that we authenticate every request using the API key.
            // If credentials (e.g. username/password) are preferred, set the NuGet.HttpClient.DefaultCredentialProvider instead.
            PackageRepositoryFactory.Default.HttpClientFactory = uri =>
            {
                var client = new NuGet.HttpClient(uri);
                client.SendingRequest += (sender, args) =>
                {
                    args.Request.Headers.Add("X-NuGet-ApiKey", ConfigurationManager.AppSettings["Retention:NuGetFeedApiKey"]);
                };
                return client;
            };

            // Prepare HttpClient (non-NuGet)
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("X-NuGet-ApiKey", ConfigurationManager.AppSettings["Retention:NuGetFeedApiKey"]);

            // Fetch packages and group them (note:  only doing this for stable packages, ignoring prerelease)
            var packageRepository = PackageRepositoryFactory.Default.CreateRepository(feedUrl);
            var packages = packageRepository.GetPackages().Where(p => p.Id == "GoogleAnalyticsTracker.Core").ToList();
            foreach (var packageGroup in packages.Where(p => p.IsReleaseVersion())
                .GroupBy(p => p.Version.Version.Major + "." + p.Version.Version.Minor))
            {
                foreach (var package in packageGroup.OrderByDescending(p => p.Version).Skip(5))
                {
                    await httpClient.DeleteAsync(string.Format("{0}api/v2/package/{1}/{2}?hardDelete=true", feedUrl, package.Id, package.Version));
                }
            }

            return new HttpResponseMessage(HttpStatusCode.OK) { ReasonPhrase = "Custom retention policy applied." };
        }
    }
}
