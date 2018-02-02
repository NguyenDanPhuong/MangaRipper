using MangaRipper.Core.FilenameDetectors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaRipper.Test
{
    [TestClass]
    public class FilenameDetectorTest
    {

        [TestInitialize]
        public void Initialize()
        {

        }

        [TestCleanup]
        public void Cleanup()
        {

        }

        [TestMethod]
        public void GoogleProxyUrlReaderTest_CorrectUrl()
        {
            var reader = new GoogleProxyFilenameDetector();
            string input = "https://images1-focus-opensocial.googleusercontent.com/gadgets/proxy?container=focus&gadget=a&no_expand=1&resize_h=0&rewriteMime=image%2F*&url=http://2.bp.blogspot.com/-daAIY2sJQcE/V8rt280634I/AAAAAAAA404/Ld1A6XZGrvcKioYmulO4MG8RcbPJf8zagCHM/s0/0001-001.png";
            var filename = reader.ParseFilename(input);
            Assert.AreEqual("0001-001.png", filename);
        }

        [TestMethod]
        public void GoogleProxyUrlReaderTest_WrongUrl()
        {
            var reader = new GoogleProxyFilenameDetector();
            string input = "https://images2-focus-opensocial.googleusercontent.com/gadgets/proxy?container=focus&gadget=a&no_expand=1&resize_h=0&rewriteMime=image%2F*&url=http://2.bp.blogspot.com/-daAIY2sJQcE/V8rt280634I/AAAAAAAA404/Ld1A6XZGrvcKioYmulO4MG8RcbPJf8zagCHM/s0/0001-001.png";
            var filename = reader.ParseFilename(input);
            Assert.AreEqual(null, filename);
        }

        [TestMethod]
        public void GoogleProxyUrlReaderTest_WrongParam()
        {
            var reader = new GoogleProxyFilenameDetector();
            string input = "https://images-focus-opensocial.googleusercontent.com/gadgets/proxy?container=focus&gadget=a&no_expand=1&resize_h=0&rewriteMime=image%2F*&url2=http://2.bp.blogspot.com/-daAIY2sJQcE/V8rt280634I/AAAAAAAA404/Ld1A6XZGrvcKioYmulO4MG8RcbPJf8zagCHM/s0/0001-001.png";
            var filename = reader.ParseFilename(input);
            Assert.AreEqual(null, filename);
        }
    }
}
