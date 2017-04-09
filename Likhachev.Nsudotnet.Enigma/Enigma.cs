using System;
using System.IO;
using System.Security.Cryptography;

namespace Likhachev.Nsudotnet.Enigma
{
    public enum Algorithm
    {
        Aes,
        Des,
        Rc2,
        Rijndael
    }

    public class Enigma
    {
        private static SymmetricAlgorithm GetAlgorithm(Algorithm algorithmType)
        {
            switch (algorithmType)
            {
                case Algorithm.Aes:
                    return Aes.Create();
                case Algorithm.Des:
                    return DES.Create();
                case Algorithm.Rc2:
                    return RC2.Create();
                case Algorithm.Rijndael:
                    return Rijndael.Create();
                default:
                    throw new ArgumentOutOfRangeException(nameof(algorithmType), algorithmType, null);
            }
        }

        private static void DoCode(string inputFilename, string outputFilename, ICryptoTransform transform)
        {
            using (FileStream input = File.OpenRead(inputFilename), output = File.OpenWrite(outputFilename))
            {
                using (var cs = new CryptoStream(output, transform, CryptoStreamMode.Write))
                {
                    input.CopyTo(cs);
                }
            }
        }

        public static void Encrypt(string inputFilename, string outputFilename, Algorithm algorithmType)
        {
            using (var algo = GetAlgorithm(algorithmType))
            {
                var transform = algo.CreateEncryptor();
                DoCode(inputFilename, outputFilename, transform);
                // TODO: save key and initial vector
            }
        }

        public static void Decrypt(string inputFilename, string outputFilename, Algorithm algorithmType, string keyFilename)
        {
            using (var algo = GetAlgorithm(algorithmType))
            {
                // TODO: load key and initial vector
                var transform = algo.CreateDecryptor();
                DoCode(inputFilename, outputFilename, transform);
            }
        }
    }
}