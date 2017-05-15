using System;

namespace Likhachev.Nsudotnet.Enigma
{
    internal class Program
    {
        private const int EncryptArgumentsCount = 4;
        private const int DecryptArgumentsCount = 5;
        private const int ModePosition = 0;
        private const int InputFilenamePosition = 1;
        private const int AlgorithmPosition = 2;
        private const int EncryptOutputFilenamePosition = 3;
        private const int KeyFilenamePosition = 3;
        private const int DecryptOutputFilenamePosition = 4;

        private static void Main(string[] args)
        {
            try
            {
                switch (args[ModePosition].ToLower())
                {
                    case "encrypt":
                        if (args.Length != EncryptArgumentsCount)
                        {
                            Console.WriteLine("Usage: {0} encrypt <input file> <aes|des|rc2|rijndael> <output file>",
                                System.AppDomain.CurrentDomain.FriendlyName);
                        }
                        else
                        {
                            var parsed = Enum.TryParse(args[AlgorithmPosition], true, out Algorithm algorithm);
                            if (!parsed)
                            {
                                Console.WriteLine("Unknown encryption algorithm: {0}", args[AlgorithmPosition]);
                            }
                            Enigma.Encrypt(args[InputFilenamePosition], args[EncryptOutputFilenamePosition], algorithm);
                        }
                        break;
                    case "decrypt":
                        if (args.Length != DecryptArgumentsCount)
                        {
                            Console.WriteLine(
                                "Usage: {0} decrypt <input file> <aes|des|rc2|rijndael> <key file> <output file>",
                                System.AppDomain.CurrentDomain.FriendlyName);
                        }
                        else
                        {
                            var parsed = Enum.TryParse(args[AlgorithmPosition], true, out Algorithm algorithm);
                            if (!parsed)
                            {
                                Console.WriteLine("Unknown encryption algorithm: {0}", args[AlgorithmPosition]);
                            }
                            Enigma.Decrypt(args[InputFilenamePosition], args[DecryptOutputFilenamePosition], algorithm,
                                args[KeyFilenamePosition]);
                        }
                        break;
                    default:
                        Console.WriteLine("Usage: {0} <encrypt|decrypt>...");
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
