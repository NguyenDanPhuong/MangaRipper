using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octokit;

namespace MangaRipper.Helper
{
    public class UpdateNotification
    {
        public static async Task<string> GetLatestVersion()
        {
            var client = new GitHubClient(new ProductHeaderValue("MyAmazingApp"));
            var release = await client.Repository.Release.GetLatest("NguyenDanPhuong", "MangaRipper");
            return release.TagName;
        }
    }
}
