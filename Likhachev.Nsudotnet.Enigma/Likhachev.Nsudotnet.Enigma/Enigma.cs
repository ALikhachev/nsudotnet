using System;
using System.IO;
using System.Security.Cryptography;

namespace Likhachev.Nsudotnet.Enigma
{
    public enum Algorithm
    {
        AES,
        DES,
        RC2,
        Rijndael
    }

    public class Enigma
    {
        private static SymmetricAlgorithm GetAlgorithm(Algorithm algorithmType)
        {
            switch (algorithmType)
            {
                case Algorithm.AES:
                    return Aes.Create();
                case Algorithm.DES:
                    return DES.Create();
                case Algorithm.RC2:
                    return RC2.Create();
                case Algorithm.Rijndael:
                    return Rijndael.Create();
                default:
                    throw new ArgumentOutOfRangeException(nameof(algorithmType), algorithmType, null);
            }
        }

        private static void SaveKey(string keyFilename, byte[] key, byte[] iv)
        {
            using (var writer = new StreamWriter(keyFilename))
            {
                writer.WriteLine(Convert.ToBase64String(key));
                writer.WriteLine(Convert.ToBase64String(iv));
            }
        }

        private static void LoadKey(string keyFilename, SymmetricAlgorithm algo)
        {
            try
            {
                using (var reader = new StreamReader(keyFilename))
                {
                    algo.Key = Convert.FromBase64String(reader.ReadLine());
                    algo.IV = Convert.FromBase64String(reader.ReadLine());
                }
            }
            catch (Exception)
            {
                throw new ArgumentException(keyFilename + " is not a key file!");
            }
        }

        private static void DoCode(string inputFilename, string outputFilename, ICryptoTransform transform, Action<int> updateProgress)
        {
            using (FileStream input = File.OpenRead(inputFilename), output = File.Open(outputFilename, FileMode.Create))
            {
                using (var cs = new CryptoStream(output, transform, CryptoStreamMode.Write))
                {
                    var buf = new byte[10 * 1024];
                    var totalRead = 0L;
                    var read = 0;
                    while ((read = input.Read(buf, 0, buf.Length)) != 0)
                    {
                        totalRead += read;
                        cs.Write(buf, 0, read);
                        updateProgress((int) ((10000 * totalRead) / input.Length));
                    }
                }
            }
        }

        public static void Encrypt(string inputFilename, string outputFilename, Algorithm algorithmType, string keyFilename, Action<int> updateProgress)
        {
            using (var algo = GetAlgorithm(algorithmType))
            {
                var transform = algo.CreateEncryptor();
                DoCode(inputFilename, outputFilename, transform, updateProgress);
                if (string.IsNullOrEmpty(keyFilename))
                {
                    keyFilename = GetDefaultKeyFilename(inputFilename);
                }
                SaveKey(keyFilename, algo.Key, algo.IV);
            }
        }

        public static string GetDefaultKeyFilename(string inputFilename)
        {
            if (string.IsNullOrEmpty(inputFilename))
            {
                return null;
            }
            return Path.Combine(Path.GetDirectoryName(inputFilename), path2: Path.GetFileNameWithoutExtension(inputFilename) + ".key");
        }

        public static string GetDefaultCryptedFilename(string inputFilename)
        {
            if (string.IsNullOrEmpty(inputFilename))
            {
                return null;
            }
            return Path.Combine(Path.GetDirectoryName(inputFilename), path2: Path.GetFileNameWithoutExtension(inputFilename) + ".bin");
        }

        public static void Decrypt(string inputFilename, string outputFilename, Algorithm algorithmType, string keyFilename, Action<int> updateProgress)
        {
            using (var algo = GetAlgorithm(algorithmType))
            {
                LoadKey(keyFilename, algo);
                var transform = algo.CreateDecryptor();
                DoCode(inputFilename, outputFilename, transform, updateProgress);
            }
        }
    }
}