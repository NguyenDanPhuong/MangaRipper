using MangaRipper.Core.Renaming;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MangaRipper.Test
{
    public class RenameByCounterTests
    {
        [Fact]
        public void Test()
        {
            var mock = new Mock<IFileManipulation>();
            mock.Setup(f => f.GetFiles("C:\\TestPath")).Returns(() =>
            {
                return new string[] {
                    "C:\\TestPath\\abc1.jpg",
                    "C:\\TestPath\\abc2.jpg",
                    "C:\\TestPath\\abc3.jpg",
                    "C:\\TestPath\\abc4.jpg",
                    "C:\\TestPath\\abc5.jpg",
                };
            });
            var counterRename = new RenameByCounter(mock.Object);
            counterRename.Run("C:\\TestPath");
            mock.Verify(f => f.Rename(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(5));
            mock.Verify(f => f.Rename("C:\\TestPath\\abc1.jpg", "C:\\TestPath\\1.jpg"), Times.Once());
            mock.Verify(f => f.Rename("C:\\TestPath\\abc2.jpg", "C:\\TestPath\\2.jpg"), Times.Once());
            mock.Verify(f => f.Rename("C:\\TestPath\\abc3.jpg", "C:\\TestPath\\3.jpg"), Times.Once());
            mock.Verify(f => f.Rename("C:\\TestPath\\abc4.jpg", "C:\\TestPath\\4.jpg"), Times.Once());
            mock.Verify(f => f.Rename("C:\\TestPath\\abc5.jpg", "C:\\TestPath\\5.jpg"), Times.Once());
        }
    }
}
