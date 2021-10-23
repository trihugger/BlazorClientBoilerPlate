using System.Security.Cryptography;
using System.Text;

namespace BlazorClientBoilerPlate.Client.API.Security
{
    public class Encryption
    {
        public string Encrypt(string text, string key)
        {
            byte[] cypher = Encoding.UTF8.GetBytes(key);
            byte[] payload = Encoding.UTF8.GetBytes(text);

            MD5 md5 = MD5.Create();
            cypher = md5.ComputeHash(cypher);
            md5.Clear();

            TripleDES tdes = TripleDES.Create();
            tdes.Key = cypher;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cypherTransformer = tdes.CreateEncryptor();
            payload = cypherTransformer.TransformFinalBlock(payload, 0, payload.Length);
            tdes.Clear();

            return Convert.ToBase64String(payload, 0, payload.Length);
        }

        public string Decrypt(string text, string key)
        {
            byte[] cypher = Encoding.UTF8.GetBytes(key);
            byte[] payload = Encoding.UTF8.GetBytes(text);

            MD5 md5 = MD5.Create();
            cypher = md5.ComputeHash(cypher);
            md5.Clear();

            TripleDES tdes = TripleDES.Create();
            tdes.Key = cypher;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cypherTransformer = tdes.CreateDecryptor();
            payload = cypherTransformer.TransformFinalBlock(payload, 0, payload.Length);
            tdes.Clear();

            return Encoding.UTF8.GetString(payload);
        }
    }
}
