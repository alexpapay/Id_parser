using System;
using System.IO;
using Newtonsoft.Json;
using Vk_Parser_Console.Classes;

namespace Vk_Parser_Console
{

    internal class Program
    {

        // ------------------------------------------------------- //
        //      DATA initialization (change value in const)        //
        // ------------------------------------------------------- //
        // Year range:
        private static int _yearFrom = 1960;
        private static int _yearTo = 2002;
        // Delay range:
        private static int _delayFrom = 550;
        private static int _delayTo = 750;

        // Path to  output file
        private static readonly string OutputFile = string.Format(Environment.CurrentDirectory + @"\Output");
        // Status variables
        private static int _usersFound;
        private static string _parsingStatus;


        // Main function
        private static void Main(string[] args)
        {
            // ------------------------------------------------------- //
            //      Request initialization (Uncomment wanted param)    //
            // ------------------------------------------------------- //
            var initParameters = new RequestParams();
            VkUsers.RequestParamsInit(initParameters);

            initParameters.City = 282;      // 282 - Minsk
                                            //initParameters.Country = 3;   // 3 - BY. All list here: https://vk.com/select_ajax.php?act=a_get_countries
                                            //initParameters.Sex = 1;       // 1 — женщина; 2 — мужчина;
                                            //initParameters.Status = 1;    
                                            /*  1 — не женат (не замужем);
                                                2 — встречается;
                                                3 — помолвлен(-а);
                                                4 — женат (замужем);
                                                5 — всё сложно;
                                                6 — в активном поиске;
                                                7 — влюблен(-а).*/
                                            //initParameters.AgeFrom = 20;
                                            //initParameters.AgeTo = 25;
                                            //initParameters.Online = 1;
                                            //initParameters.Interests = "Drinking";
                                            //initParameters.Position = "CEO";

            
            Console.WriteLine("Please enter year from (ex. 1960): ");
            _yearFrom = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Please enter year to   (ex. 2002): ");
            _yearTo = Convert.ToInt32(Console.ReadLine());
           
            Console.WriteLine("Please enter start delay (>300): ");
            _delayFrom = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Please enter max delay  (unlim): ");
            _delayTo = Convert.ToInt32(Console.ReadLine());   

            Console.WriteLine("Please enter number of city (ex. 282 it`s Minsk: ");
            initParameters.City = Convert.ToInt32(Console.ReadLine());
            
            //_yearFrom = 1970;
            //_yearTo = 2002;
            //_delayFrom = 1000;
            //_delayTo = 1800;
            initParameters.City = 282;

            Console.WriteLine("-----------------------------:-------------------------------------------------");
            Console.WriteLine("Qty names in dictionaries    : {0}", Utils.GetUsersNames().Length);
            Console.WriteLine("Qty of tokens                : {0}\n", File.ReadAllLines(ParseMethods.TokensFile).Length);
            Console.WriteLine("Parsing Name                 : {0}", initParameters.Name);
            Console.WriteLine("Parsing Count                : {0}", initParameters.Count);
            Console.WriteLine("Parsing City                 : {0}", initParameters.City);
            Console.WriteLine("Parsing Country              : {0}", initParameters.Country);
            Console.WriteLine("Parsing Sex                  : {0}", initParameters.Sex);
            Console.WriteLine("Parsing Status               : {0}", initParameters.Status);
            Console.WriteLine("Parsing Age from             : {0}", initParameters.AgeFrom);
            Console.WriteLine("Parsing Age to               : {0}", initParameters.AgeTo);
            Console.WriteLine("Parsing Online status        : {0}", initParameters.Status);
            Console.WriteLine("Parsing Interests            : {0}", initParameters.Interests);
            Console.WriteLine("Parsing Position             : {0}\n", initParameters.Position);
            Console.WriteLine("Parsing Year from            : {0}", _yearFrom);
            Console.WriteLine("Parsing Year to              : {0}", _yearTo);
            Console.WriteLine("Parsing Delay from           : {0}", _delayFrom);
            Console.WriteLine("Parsing Delay to             : {0}", _delayTo);
            Console.WriteLine("-----------------------------:-------------------------------------------------\n");

            Console.WriteLine("Please press Enter to start parsing");
            Console.ReadKey();

            if (File.Exists(OutputFile + "_" + initParameters.City + ".txt") == false)
                    File.WriteAllText(OutputFile + "_" + initParameters.City + ".txt", string.Format("Parsing started at: {0}\n", DateTime.Now));

            // ------------------------------------------------------- //
            //     Methods initialization (Uncomment wanted method)    //
            // ------------------------------------------------------- //
            //ParseMethods.ParsingUsersByBdayDirect(out _usersFound, out _parsingStatus, initParameters, YearFrom, YearTo, DelayFrom, DelayTo, false, true, OutputFile);
            //ParseMethods.ParsingUsersByBdayRandom(out _usersFound, out _parsingStatus, initParameters, YearFrom, YearTo, DelayFrom, DelayTo, false, true, OutputFile);
            ParseMethods.ParsingUsersByBdayFullRandom(out _usersFound, out _parsingStatus, initParameters, _yearFrom, _yearTo, _delayFrom, _delayTo, false, OutputFile);
            //ParseMethods.ParsingUsersByDictionaryName(out _usersFound, out _parsingStatus, initParameters, 20, 25, DelayFrom, DelayTo, false, OutputFile);
            //ParseMethods.ParsingUsersByExecuteRandom(out _usersFound, out _parsingStatus, initParameters, _yearFrom, _yearTo, _delayFrom, _delayTo, OutputFile);

            Console.WriteLine("Qty of founded users: {0}", _usersFound);
            Console.WriteLine("-------------------------------------\n");
            Console.WriteLine(_parsingStatus);
            Console.WriteLine("Press any key to exit...");

            if (File.Exists(string.Format(OutputFile + "_" + initParameters.City + ".txt")))
                {
                    File.Copy(string.Format(OutputFile + "_" + initParameters.City + ".txt"),
                        string.Format(OutputFile + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour +
                                      DateTime.Now.Minute + "_" + initParameters.City + "_finished.txt"));
                    File.Delete(string.Format(OutputFile + "_" + initParameters.City + ".txt"));
                }

            Console.ReadKey();
        }
    }
}
