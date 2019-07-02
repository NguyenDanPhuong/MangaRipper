using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MangaRipper.Test
{
    public class FanfoxTests : IDisposable
    {
        readonly ChromeDriver ChromeDriver;
        private readonly WebDriverWait Wait;

        public FanfoxTests()
        {
            var options = new ChromeOptions();
            ChromeDriver = new ChromeDriver(options);
            Wait = new WebDriverWait(ChromeDriver, TimeSpan.FromSeconds(10));
        }

        public void Dispose()
        {
            ChromeDriver.Close();
            ChromeDriver.Dispose();
        }

        [Fact]
        public void Test()
        {
            ChromeDriver.Url = "https://fanfox.net/manga/tian_jiang_xian_shu_nan/c001/1.html";
            var img = ChromeDriver.FindElementByXPath("//img[@class='reader-main-img']");
            var imgList = new List<string>();
            var src = img.GetAttribute("src");
            imgList.Add(src);

            var nextButton = ChromeDriver.FindElementByXPath("//a[@data-page][text()='>']");
            do
            {
                var currentDatapage = nextButton.GetAttribute("data-page");
                nextButton.Click();
                var src2 = img.GetAttribute("src");
                imgList.Add(src2);

                Wait.Until(driver =>
                {
                    try
                    {
                        var currentNext = ChromeDriver.FindElementByXPath("//a[@data-page][text()='>']");
                        if (currentNext.GetAttribute("data-page") != currentDatapage)
                        {
                            nextButton = currentNext;
                            return true;
                        }
                        return false;
                    }
                    catch (NoSuchElementException ex)
                    {
                        nextButton = null;
                        return true;
                    }
                });
               
            }
            while (nextButton != null && nextButton.Displayed);

        }
    }
}
