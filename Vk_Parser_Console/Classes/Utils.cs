using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Vk_Parser_Console.Classes
{
    public class Utils
    {
        // Path to dictionary files dir
        private static readonly string DictionaryPath = string.Format(Environment.CurrentDirectory + @"\Dic\");

        // ------------------------------------------------------- //
        //     UTILITY methods (requests, checking and othrz)      //
        // ------------------------------------------------------- //
        //
        // Read all dictionaries from Dic folder
        public static string[] GetUsersNames()
        {
            var dicFiles = new DirectoryInfo(DictionaryPath);
            string[] allNames = {};

            foreach (var dic in dicFiles.GetFiles())
            {
                var newNames = File.ReadAllLines(DictionaryPath + dic.Name, Encoding.Default);
                allNames = allNames.Concat(newNames).ToArray();
            }

            return allNames;
        }

        // Write users id in file (streaming or union)
        public static void WriteInFile(VkUsers users, string outputFile, int city)
        {
            using (var stream = new StreamWriter(outputFile + "_" + city + ".txt", true))
            {
                foreach (var user in users.response)
                {
                    /*
                    var oldUsers = File.ReadAllLines(OutputFile + ".txt");
                    string[] newUsers = { user.uid.ToString() };
                    var unionUsers = oldUsers.Union(newUsers).ToArray();
                    File.WriteAllLines(OutputFile + ".txt", unionUsers);
                    */
                    stream.WriteLine(user.uid.ToString());
                }
            }
        }

        // Write users id in file (streaming or union)
        public static void WriteIdInFile(VkId users, string outputFile, int city)
        {
            using (var stream = new StreamWriter(outputFile + "_" + city + ".txt", true))
            {
                foreach (var user in users.response)
                {
                    stream.WriteLine(user.ToString());
                }
            }
        }

        // Get users from Vk (isWriteInFile == false), Get users and write id into file (isWriteInFile == true)
        public static VkUsers GetWriteUsers(RequestParams parameters, int delayFrom, int delayTo, string outputFile,
            bool isWriteInFile)
        {
            var randomGen = new Random();

            Thread.Sleep(randomGen.Next(delayFrom, delayTo));
            var response = VkRequest.VkSearchResponse(parameters);
            var users = VkUsers.VkDeserealiseResponse(response);

            if (isWriteInFile) WriteInFile(users, outputFile, parameters.City);

            return users;
        }

        // Token check with direct parsing
        public static int TokenCheckDirectVoid(
            int i,
            string[] tokens,
            RequestParams parameters,
            int bdayDay,
            VkUsers users)
        {
            // First tokens selecting
            if (i < tokens.Length)
            {
                parameters.Token = tokens[i];
                Console.WriteLine("\nToken are blocked. Selected next {0} token\n", i);
                Console.WriteLine("Lets try again parsing {0} day\n", bdayDay);

                return 1;
            }
            // If all tokens in list are lock - check token list again
            else
            {
                i = 0;
                Console.WriteLine("\n.Start again from 0 token\n");

                try
                {
                    while (users.response.Count == 0 && i < tokens.Length)
                    {
                        Console.WriteLine("\n.Try token: {0} \n", i);
                        parameters.Token = tokens[i];
                        var response = VkRequest.VkSearchResponse(parameters);
                        users = VkUsers.VkDeserealiseResponse(response);
                        i++;

                        // No more working tokens -> exit
                        if (i == tokens.Length)
                        {
                            Console.WriteLine("\nTokens are ended. Parsing is finished!\n");
                            return 2;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\nWhile cycle exception:\n{0}", ex);
                    return 3;
                }

                Console.WriteLine("\nTokens are ended. Try start from {0} token again\n", i);
                Console.WriteLine("Lets try again parsing {0} day\n", bdayDay);

                return 4;
            }
        }
    }
}