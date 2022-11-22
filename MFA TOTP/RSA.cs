using System;
using System.Security.Cryptography;
using System.Text;


namespace MFA_TOTP
{
    public class RSA
    {
        public const bool OAEP_PADDING = true;
        /// <summary>
        /// PKCS1 padding is required for most encryption using JavaScript packages
        /// </summary>
        public const bool PKCS1_PADDING = false;

        public static string Encrypt(
            RSACryptoServiceProvider csp,
            string plaintext
        )
        {
            return Convert.ToBase64String(
                csp.Encrypt(
                    Encoding.UTF8.GetBytes(plaintext),
                    PKCS1_PADDING
                )
            );
        }

        public static string Decrypt(
            RSACryptoServiceProvider csp,
            string encrypted
        )
        {
            return Encoding.UTF8.GetString(
                csp.Decrypt(
                    Convert.FromBase64String(encrypted),
                    PKCS1_PADDING
                )
            );
        }

        public static string Sign(
            RSACryptoServiceProvider csp,
            string plaintext
        )
        {
            // compute sha256 hash of the data
            byte[] hash = new SHA256CryptoServiceProvider()
                .ComputeHash(Encoding.UTF8.GetBytes(plaintext));

            // base64 encode the signature
            return Convert.ToBase64String(
                csp.SignHash(hash, CryptoConfig.MapNameToOID("SHA256"))
            );
        }

        public static bool Verify(
            RSACryptoServiceProvider csp,
            string plaintext,
            string signature
        )
        {
            // compute sha256 hash of the data
            byte[] hash = new SHA256CryptoServiceProvider()
                .ComputeHash(Encoding.UTF8.GetBytes(plaintext));

            return csp.VerifyHash(
                hash,
                CryptoConfig.MapNameToOID("SHA256"),
                Convert.FromBase64String(signature));
        }

        public static void runTests()
        {
            Console.WriteLine("Starting RSA tests");

            string plaintext = "this is my secret message";

            RSACryptoServiceProvider initialProvider = new RSACryptoServiceProvider(2048);
            string encrypted = Encrypt(initialProvider, plaintext);
            Console.WriteLine("plaintext encrypted to: " + encrypted);
            string decrypted = Decrypt(initialProvider, encrypted);
            Console.WriteLine("plaintext decrypted to: " + decrypted);
            string signature = Sign(initialProvider, plaintext);
            Console.WriteLine("signature: " + signature);
            Console.WriteLine("signature verified: " + Verify(initialProvider, plaintext, signature));

            Console.WriteLine("RSA tests completed");
        }
    }
}