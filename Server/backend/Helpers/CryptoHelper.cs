﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace backend.Helpers
{
    public sealed class CryptoHelper
    {
        private static readonly string PRIVATE_KEY_LOCATION = "./PrivateKey.pem";
        private static CryptoHelper instance = null;
        private static readonly object _lock = new object();
        private RSA rsa = null;
        internal RSA Rsa
        {
            get
            {
                if (rsa == null)
                {
                    if (File.Exists(PRIVATE_KEY_LOCATION))
                    {
                        rsa = RSA.Create();
                        ImportKey();
                    }
                    else
                    {
                        rsa = RSA.Create(2048);
                        ExportKey();
                    }
                }
                return rsa;
            }
        }

        CryptoHelper()
        {
        }

        public static CryptoHelper Instance
        {
            get
            {
                lock (_lock)
                {
                    if (instance == null)
                    {
                        instance = new CryptoHelper();
                    }
                    return instance;
                }
            }
        }

        public String GetPublicKeyPem()
        {
            return "-----BEGIN RSA PUBLIC KEY-----" + Environment.NewLine + Convert.ToBase64String(Rsa.ExportRSAPublicKey()) + Environment.NewLine + "-----END RSA public KEY-----";
        }

        internal byte[] GetPrivateKey()
        {
            return Rsa.ExportRSAPublicKey();
        }

        private void ExportKey()
        {
            var fileStream = new FileStream(PRIVATE_KEY_LOCATION, FileMode.OpenOrCreate);
            using (var writer = new StreamWriter(fileStream))
            {
                writer.Write("-----BEGIN RSA PRIVATE KEY-----\n");
                writer.Flush();
                var key = Convert.ToBase64String(rsa.ExportRSAPrivateKey());
                var count = 0;
                foreach (char c in key)
                {

                    if (++count == 64)
                    {
                        writer.WriteLine(c);
                        count = 0;
                    }
                    else
                    {
                        writer.Write(c);
                    }
                }
                writer.Flush();
                writer.WriteLine("");
                writer.WriteLine("-----END RSA PRIVATE KEY-----");
            }
        }

        private void ImportKey()
        {
            //TODO: Cleanup
            //var data = "";
            //var fileStream = new FileStream("./PrivateKey.pem", FileMode.OpenOrCreate);
            //using (var reader = new StreamReader(fileStream))
            //{
            //    reader.ReadLine();

            //    var line = reader.ReadLine();
            //    do
            //    {
            //        data += line.Trim();
            //        line = reader.ReadLine();
            //    } while (!line.Contains("END RSA PRIVATE KEY"));
            //    Console.Out.Write(data);
            //}
            //rsa.ImportRSAPrivateKey(rsa.ExportRSAPrivateKey);
            using(var filestream = new FileStream(PRIVATE_KEY_LOCATION, FileMode.Open))
            {
                using(var reader = new StreamReader(filestream))
                {
                    var data = reader.ReadToEnd();
                    var span = data.AsSpan();
                    rsa.ImportFromPem(span);
                }
            }




            
        }

    }
}
