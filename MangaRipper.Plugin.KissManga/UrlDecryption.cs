using MangaRipper.Core.CustomException;
using NLog;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MangaRipper.Plugin.KissManga
{
    static internal class UrlDecryption
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Decrypt the Text
        /// </summary>
        /// <param name="cipherText"></param>
        /// <param name="iv"></param>
        /// <param name="keybytes"></param>
        /// <returns></returns>
        static public string DecryptStringAES(string cipherText, string iv, byte[] keybytes)
        {
            var ivbytes = Enumerable.Range(0, iv.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(iv.Substring(x, 2), 16))
                     .ToArray();

            var encrypted = Convert.FromBase64String(cipherText);
            var decriptedFromJavascript = DecryptStringFromBytes(encrypted, keybytes, ivbytes);
            return string.Format(decriptedFromJavascript);
        }


        static private string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
        {
            // Check arguments.  
            if (cipherText == null || cipherText.Length <= 0)
            {
                _logger.Error("Text for decryption was not provided");
                throw new ArgumentNullException("cipherText");
            }
            if (key == null || key.Length <= 0)
            {
                _logger.Error("Key for decryption was not provided");
                throw new ArgumentNullException("key");
            }
            if (iv == null || iv.Length <= 0)
            {
                _logger.Error("IV for decryption was not provided");
                throw new ArgumentNullException("key");
            }

            // Declare the string used to hold  
            // the decrypted text.  
            string plaintext = null;

            // Create an RijndaelManaged object  
            // with the specified key and IV.  
            using (var rijAlg = new RijndaelManaged())
            {
                //Settings
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.FeedbackSize = 128;

                rijAlg.Key = key;
                rijAlg.IV = iv;

                // Create a decrytor to perform the stream transform.  
                var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                try
                {
                    // Create the streams used for decryption.  
                    using (var msDecrypt = new MemoryStream(cipherText))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {

                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                // Read the decrypted bytes from the decrypting stream  
                                // and place them in a string.  
                                plaintext = srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }
                catch
                {
                    _logger.Error("Decryption Failed");
                    throw new MangaRipperException("Cannot decrypt links in KissManga");
                }
            }

            return plaintext;
        }

        static public byte[] ReturnShaKeyBytes(string cipherText)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(cipherText);
            SHA256Managed hashstring = new SHA256Managed();
            return hashstring.ComputeHash(bytes);
        }

        /// <summary>
        /// Change keys from Hex to String
        /// </summary>
        /// <param name="hex">Text as "\\x2E\\x0E"</param>
        /// <returns></returns>
        static public string FromHexToString(string hex)
        {
            // because first element will be empty
            var hexEncode = hex.Replace(@"\", string.Empty).Substring(1).Split('x');

            var sb = new StringBuilder();

            foreach (string liter in hexEncode)
            {
                int value = Convert.ToInt32(liter, 16);
                sb.Append(char.ConvertFromUtf32(value));
            }

            return sb.ToString();
        }
    }
}
