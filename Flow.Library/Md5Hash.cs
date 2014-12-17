using System.Text;

namespace Flow.Library
{
    public class Md5Hash
    {
        /// <summary>
        /// Create a hash
        /// </summary>
        /// <returns>hash</returns>
        public static string CreateMd5Hash(byte[] inputBytes)
        {
            // Use input string to calculate MD5 hash
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            //byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            // Convert the byte array to hexadecimal string
            var sb = new StringBuilder();
            for (var i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
                // To force the hex string to lower-case letters instead of
                // upper-case, use the following line instead:
                // sb.Append(hashBytes[i].ToString("x2")); 
            }
            return sb.ToString();
        }

    }
}
