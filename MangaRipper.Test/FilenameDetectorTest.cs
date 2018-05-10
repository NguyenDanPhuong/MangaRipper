using MangaRipper.Core.FilenameDetectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MangaRipper.Test
{
    public class FilenameDetectorTest
    {

        [Fact]
        public void GoogleProxyUrlReaderTest_CorrectUrl()
        {
            var reader = new GoogleProxyFilenameDetector();
            string input = "https://images1-focus-opensocial.googleusercontent.com/gadgets/proxy?container=focus&gadget=a&no_expand=1&resize_h=0&rewriteMime=image%2F*&url=http://2.bp.blogspot.com/-daAIY2sJQcE/V8rt280634I/AAAAAAAA404/Ld1A6XZGrvcKioYmulO4MG8RcbPJf8zagCHM/s0/0001-001.png";
            var filename = reader.ParseFilename(input);
            Assert.Equal("0001-001.png", filename);
        }

        [Fact]
        public void GoogleProxyUrlReaderTest_WrongUrl()
        {
            var reader = new GoogleProxyFilenameDetector();
            string input = "https://images2-focus-opensocial.googleusercontent.com/gadgets/proxy?container=focus&gadget=a&no_expand=1&resize_h=0&rewriteMime=image%2F*&url=http://2.bp.blogspot.com/-daAIY2sJQcE/V8rt280634I/AAAAAAAA404/Ld1A6XZGrvcKioYmulO4MG8RcbPJf8zagCHM/s0/0001-001.png";
            var filename = reader.ParseFilename(input);
            Assert.Equal(null, filename);
        }

        [Fact]
        public void GoogleProxyUrlReaderTest_WrongParam()
        {
            var reader = new GoogleProxyFilenameDetector();
            string input = "https://images-focus-opensocial.googleusercontent.com/gadgets/proxy?container=focus&gadget=a&no_expand=1&resize_h=0&rewriteMime=image%2F*&url2=http://2.bp.blogspot.com/-daAIY2sJQcE/V8rt280634I/AAAAAAAA404/Ld1A6XZGrvcKioYmulO4MG8RcbPJf8zagCHM/s0/0001-001.png";
            var filename = reader.ParseFilename(input);
            Assert.Equal(null, filename);
        }
    }
}
