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

        private static void SaveKey(string inputFilename, byte[] key, byte[] iv)
        {
            var keyFilename = Path.Combine(Path.GetDirectoryName(inputFilename), path2: Path.GetFileNameWithoutExtension(inputFilename) + ".key.txt");
            Console.WriteLine(keyFilename);
            using (var writer = new StreamWriter(keyFilename))
            {
                writer.WriteLine(Convert.ToBase64String(key));
                writer.WriteLine(Convert.ToBase64String(iv));
            }
        }

        private static void LoadKey(string keyFilename, SymmetricAlgorithm algo)
        {
            using (var reader = new StreamReader(keyFilename))
            {
                algo.Key = Convert.FromBase64String(reader.ReadLine());
                algo.IV = Convert.FromBase64String(reader.ReadLine());
            }
        }

        private static void DoCode(string inputFilename, string outputFilename, ICryptoTransform transform)
        {
            using (FileStream input = File.OpenRead(inputFilename), output = File.Open(outputFilename, FileMode.Create))
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
                SaveKey(inputFilename, algo.Key, algo.IV);
            }
        }

        public static void Decrypt(string inputFilename, string outputFilename, Algorithm algorithmType, string keyFilename)
        {
            using (var algo = GetAlgorithm(algorithmType))
            {
                LoadKey(keyFilename, algo);
                var transform = algo.CreateDecryptor();
                DoCode(inputFilename, outputFilename, transform);
            }
        }
    }
}