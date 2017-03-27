using System;
using System.Security.Cryptography;
using System.Text;
using MangaRipper.Helpers;
using MangaRipper.Plugin.KissManga;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MangaRipper.Test
{
    [TestClass]
    public class KissMangaDecryptionTest
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
        public void Decryption()
        {
            string input =
                "La95D0tCAA+7+KgBsY2jd/pyPuZWTQzpbvHtVfADHY1XGDx5dwf/MCyZOnscbbmUK3hd5obqZZqoGtk3iOhIpt7dVahYZzfv1P61p4uVHzEcV1XjwUsGoty4VglIfcAwgwwacDZUzSGVy/wsTH5CEsgBQuC5+oZRfYs0l5KugpPG2f//jIJHzrlR6y2wmMGVJSgbhE8QKkMQK0ckbsf4XvZAt/wygkHX6NSAybFIqEjigodKstHqccs5QW3fJLTsnPCAwNOgHnWO3q+L7d7qy9KeYPe0rVzYC8AinxHhsS2h6sYkg6ywwW6nnwgepSO6hnrjCqE2cSLgV3sp5MM4cw==";
            string expected =
                "https://images1-focus-opensocial.googleusercontent.com/gadgets/proxy?container=focus&gadget=a&no_expand=1&resize_h=0&rewriteMime=image%2F*&url=http%3a%2f%2fcdn.eatmanga.com%2fmangas%2fManga-Scan%2fNaruto%2fNaruto-Side-Story-01%2f001.jpg&imgmax=30000";
            string iv = "a5e8e2e9c2721be0a84ad660c472c1f3";
            string chko = "72nnasdasd9asdn123";
            var tool = new KissMangaTextDecryption(iv, chko);
            var result = tool.DecryptFromBase64(input);
            Assert.AreEqual(expected, result);
        }
    }
}
