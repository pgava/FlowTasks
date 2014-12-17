using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace Flow.Library.Security
{
    /// <summary>
    /// Encryption
    /// </summary>
    public static class Encryption
    {
        /// <summary>
        /// IV
        /// </summary>
        private static readonly byte[] Iv = { 0x1F, 0xED, 0x8C, 0x3A, 0x30, 0xFD, 0x26, 0xE2 };
#if USE_ENCRYPTION        
        private static readonly string _encryptionKey = "FL0WTA5K";
#endif
        /// <summary>
        /// Encrypt
        /// </summary>
        /// <param name="messageToEncrypt">MessageToEncrypt</param>
        /// <param name="encryptionKey">EncryptionKey</param>
        /// <returns>Emcrypted message</returns>
        private static string Encrypt(string messageToEncrypt, string encryptionKey)
        {
            byte[] key = Encoding.UTF8.GetBytes(encryptionKey);
            byte[] inputByteArray = Encoding.UTF8.GetBytes(messageToEncrypt);
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();

            var memoryStream = new MemoryStream(); // memoryStream dosposed by CryptoStream
            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, des.CreateEncryptor(key, Iv), CryptoStreamMode.Write))
            {
                cryptoStream.Write(inputByteArray, 0, inputByteArray.Length);
                cryptoStream.FlushFinalBlock();

                return Convert.ToBase64String(memoryStream.ToArray());
            }
        
        }

        /// <summary>
        /// Decrypt
        /// </summary>
        /// <param name="messageToDecrypt">MessageToDecrypt</param>
        /// <param name="encryptionKey">EncryptionKey</param>
        /// <returns>Message decrypted</returns>
        private static string Decrypt(string messageToDecrypt, string encryptionKey)
        {                
            byte[] key = Encoding.UTF8.GetBytes(encryptionKey);
            byte[] inputByteArray = Convert.FromBase64String(messageToDecrypt);
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();

            var memoryStream = new MemoryStream(); // memoryStream dosposed by CryptoStream
            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, des.CreateDecryptor(key, Iv), CryptoStreamMode.Write))
            {            
                cryptoStream.Write(inputByteArray, 0, inputByteArray.Length);
                cryptoStream.FlushFinalBlock();

                Encoding encodingObj = Encoding.UTF8;
                return encodingObj.GetString(memoryStream.ToArray());
            }
        }

        /// <summary>
        /// Decrypt
        /// </summary>
        /// <param name="msg">Message</param>
        /// <returns>Decrypted message</returns>
        public static string Decrypt(string msg)
        {
#if USE_ENCRYPTION
            return Decrypt(msg, _encryptionKey);
#else
            return msg;
#endif
        }

        /// <summary>
        /// Encrypt
        /// </summary>
        /// <param name="msg">Message</param>
        /// <returns>Encrypted message</returns>
        public static string Encrypt(string msg)
        {
#if USE_ENCRYPTION
            return Encrypt(msg, _encryptionKey);
#else
            return msg;
#endif
        }

    }
}
