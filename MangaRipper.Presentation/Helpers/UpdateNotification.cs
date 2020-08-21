using System;
using System.Threading.Tasks;
using Octokit;
using System.Net;

namespace MangaRipper.Helpers
{
    public class UpdateNotification
    {
        public static async Task<string> GetLatestVersion()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var client = new GitHubClient(new ProductHeaderValue("MyAmazingApp"));
            var release = await client.Repository.Release.GetLatest("NguyenDanPhuong", "MangaRipper");
            return release.TagName;
        }

        public static long GetLatestBuildNumber(string version)
        {
            return Convert.ToInt64(version.Remove(0, version.LastIndexOf(".", StringComparison.Ordinal) + 1));
        }
    }
}