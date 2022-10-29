using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MessageClient {
    internal class Crypto {
        RSA? rsa;

        public Crypto(byte[] pubKey) {
            rsa = RSACng.Create();
            int bytesRead;
            rsa.ImportRSAPublicKey(pubKey, out bytesRead);
        }

        public byte[] EncryptString(string message) {
            if (rsa == null) throw new Exception("RSA does not exist!");
            return rsa.Encrypt(Encoding.UTF8.GetBytes(message), RSAEncryptionPadding.Pkcs1);
        }
    }
}
