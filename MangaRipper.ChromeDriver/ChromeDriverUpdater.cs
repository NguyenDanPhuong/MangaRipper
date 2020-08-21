using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;

namespace MangaRipper.ChromeDriver
{
    public class ChromeDriverUpdater
    {
        private readonly string driverFolder;

        public ChromeDriverUpdater(string driverFolder)
        {
            this.driverFolder = driverFolder;
        }

        public async Task ExecuteAsync()
        {
            string chromeVersion = GetChromeVersion();
            var requiredDriverVersion = await GetCompatibleDriverVersionAsync(chromeVersion).ConfigureAwait(false);
            var currentDriverVersion = GetCurrentDriverVersion();
            if (requiredDriverVersion != currentDriverVersion)
            {
                await UpdateDriverAsync(requiredDriverVersion).ConfigureAwait(false);
            }
        }

        private async Task UpdateDriverAsync(string requiredDriverVersion)
        {
            var tempFolder = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            if (!Directory.Exists(tempFolder))
            {
                Directory.CreateDirectory(tempFolder);
            }

            var fileNameWithPath = Path.Combine(tempFolder, "chromedriver_win32.zip");

            var httpClient = new HttpClient();

            using (var response = await httpClient.GetAsync("https://chromedriver.storage.googleapis.com/" + requiredDriverVersion + "/chromedriver_win32.zip"))
            using (var streamReader = new FileStream(fileNameWithPath, FileMode.Create, FileAccess.Write))
            {
                await response.Content.CopyToAsync(streamReader).ConfigureAwait(false);
            }

            var driverExe = Path.Combine(driverFolder, "chromedriver.exe");
            if (File.Exists(driverExe))
            {
                File.Delete(driverExe);
            }

            ZipFile.ExtractToDirectory(fileNameWithPath, driverFolder);
        }

        private string GetCurrentDriverVersion()
        {
            var driverExe = Path.Combine(driverFolder, "chromedriver.exe");
            if (!File.Exists(driverExe))
            {
                return "";
            }

            Process process = new Process();
            process.StartInfo.FileName = driverExe;
            process.StartInfo.Arguments = "--version";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return output.Split(new char[] { ' ' })[1];
        }

        private async Task<string> GetCompatibleDriverVersionAsync(string chromeVersion)
        {
            var url = "https://chromedriver.storage.googleapis.com/LATEST_RELEASE_" + chromeVersion.Remove(chromeVersion.LastIndexOf('.'));
            HttpClient httpClient = new HttpClient();
            return await httpClient.GetStringAsync(url).ConfigureAwait(false);
        }

        private string GetChromeVersion()
        {
            var chromeLocation = @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe";
            return FileVersionInfo.GetVersionInfo(chromeLocation).FileVersion;
        }
    }
}
