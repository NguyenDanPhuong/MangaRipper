using MangaRipper.ChromeDriver;
using MangaRipper.Helpers;
using Xunit;

namespace MangaRipper.Test
{
    public class ChromeDriverUpdateTest
    {
#if DEBUG
        [Fact]
#endif
        public async void UpdateAsync()
        {
            var driverUpdate = new ChromeDriverUpdater(".\\");
            await driverUpdate.ExecuteAsync();
        }
    }
}
