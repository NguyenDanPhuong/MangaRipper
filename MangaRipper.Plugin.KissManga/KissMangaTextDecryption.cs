using MangaRipper.Core.CustomException;
using NLog;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MangaRipper.Plugin.KissManga
{
    public class KissMangaTextDecryption
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public KissMangaTextDecryption(string iv, string chko)
        {
            IV = Enumerable.Range(0, iv.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(iv.Substring(x, 2), 16))
                     .ToArray(); ;
            Key = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(chko));
        }

        public byte[] Key { get; set; }

        public byte[] IV { get; set; }

        /// <summary>
        /// Decrypt the Text
        /// </summary>
        /// <param name="cipherText"></param>
        /// <returns></returns>
        public string DecryptFromBase64(string cipherText)
        {
            var encrypted = Convert.FromBase64String(cipherText);
            var decriptedFromJavascript = DecryptFromBytes(encrypted);
            return decriptedFromJavascript;
        }

        private string DecryptFromBytes(byte[] cipherText)
        {
            try
            {
                var decryptor = CreateDecryptor();
                using (var msDecrypt = new MemoryStream(cipherText))
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (var srDecrypt = new StreamReader(csDecrypt))
                {
                    return srDecrypt.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Decryption Failed");
                throw new MangaRipperException("Cannot decrypt links in KissManga");
            }
        }

        private ICryptoTransform CreateDecryptor()
        {
            using (var rijAlg = new RijndaelManaged())
            {
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.FeedbackSize = 128;
                rijAlg.Key = Key;
                rijAlg.IV = IV;
                return rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);
            }
        }
    }
}
