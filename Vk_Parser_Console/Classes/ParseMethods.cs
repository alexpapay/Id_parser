using System;
using System.IO;
using System.Threading;
using Newtonsoft.Json;

namespace Vk_Parser_Console.Classes
{
    public class ParseMethods
    {
        // Path to file with tokens list
        public static readonly string TokensFile = string.Format(Environment.CurrentDirectory + @"\Tokens\Tokens.txt");

        // ------------------------------------------------------- //
        //     PARSING methods (requests, checking and othrz)      //
        // ------------------------------------------------------- //
        //
        // Parsing functions :: Only by Birthday :: Random token & date from list at each parsing
        public static void ParsingUsersByExecuteRandom(
            out int usersFound,
            out string parsingStatus,
            RequestParams parameters,
            int birthYearFrom,
            int birthYearTo,
            int delayFrom,
            int delayTo,
            string outputFile)
        {
            usersFound = 0;
            var randomGen = new Random();
            var tokens = File.ReadAllLines(TokensFile);
            var qtyTokens = File.ReadAllLines(TokensFile).Length;
            var allDates = Month.SetAllDatesFromTo(birthYearFrom, birthYearTo);
            var countOfDates = Month.SetAllDatesFromTo(birthYearFrom, birthYearTo).Length;
            var countOfRequests = 1;
            var requestData = "";
            var currentNumOfDate = Randomizer.SetRandomDateNums(birthYearFrom, birthYearTo);
            const char delimeter = '.';

            // Generate random array numbers of tokens for Token Checking
            var rndTokenArray = new int[qtyTokens];
            var rndTokenArrayNums = new Randomizer(0, qtyTokens);
            for (var rnd = 0; rnd < rndTokenArrayNums.Count; rnd++)
                rndTokenArray[rnd] = rndTokenArrayNums.Next();

            #region Parsing cycle with random date & token
            for (var date = 0; date < countOfDates; date++)
            {
                #region Checking all tokens
                if (rndTokenArray.Length == 0)
                {
                    Console.WriteLine("\nTokens are ended. Parsing is finished!\n");
                    Console.WriteLine("You want to continue parsing? Y/N?");

                    RndFull: var a = Console.ReadLine();
                    if (a == "Y" || a == "y")
                    {
                        // Re-init array of random tokens
                        qtyTokens = File.ReadAllLines(TokensFile).Length;
                        rndTokenArray = new int[qtyTokens];
                        rndTokenArrayNums = new Randomizer(0, qtyTokens);
                        for (var rnd = 0; rnd < rndTokenArrayNums.Count; rnd++)
                            rndTokenArray[rnd] = rndTokenArrayNums.Next();
                        date = date + 24;
                    }
                    else if (a == "N" || a == "n")
                    {
                        parsingStatus = "Parsing not full completed!";
                        return;
                    }
                    else
                    {
                        Console.WriteLine("Please enter correct answer (Y or N)");
                        goto RndFull;
                    }
                }
                #endregion
                
                var splitDate = allDates[currentNumOfDate[date]].Split(delimeter);

                parameters.BirthDay = Convert.ToInt32(splitDate[0]);
                parameters.BirthMonth = Convert.ToInt32(splitDate[1]);
                parameters.BirthYear = Convert.ToInt32(splitDate[2]);

                requestData += string.Format("var%20result{0}%3DAPI.users.search%28%7B%22count%22%3A1000%2C%22city%22%3A{1}%2C%22country%22%3A{2}%2C%22sex%22%3A{3}%2C%22status%22%3A{4}%2C%22birth_day%22%3A{5}%2C%22birth_month%22%3A{6}%2C%22birth_year%22%3A{7}%2C%22online%22%3A{8}%2C%22interests%22%3A%22{9}%22%2C%22position%22%3A%22{10}%22%7D%29.items@.id%3B", 
                    countOfRequests, parameters.City, parameters.Country, parameters.Sex, parameters.Status, parameters.BirthDay, parameters.BirthMonth, parameters.BirthYear, parameters.Online, parameters.Interests, parameters.Position);

                countOfRequests++;
                
                if (countOfRequests == 25 || date == countOfDates-1)
                {
                    var rndToken = randomGen.Next(0, qtyTokens);    // Generate rnd number of token
                    parameters.Token = tokens[rndTokenArray[rndToken]];

                    var answer = VkRequest.VkExecuteResponse(parameters, countOfRequests, requestData);
                    var usersId = JsonConvert.DeserializeObject<VkId>(answer);
                    Utils.WriteIdInFile(usersId, outputFile, parameters.City);

                    Console.WriteLine("-- Parsing with {0} token:", rndTokenArray[rndToken]);
                    Console.WriteLine("Current Date number is: {0} ", date);
                    Console.WriteLine("Users founded : {0}", usersId.response.Length);

                    usersFound = usersFound + usersId.response.Length;

                    Thread.Sleep(randomGen.Next(delayFrom, delayTo));

                    #region Random token checking (simple edition):
                    if (usersId.response.Length == 0)
                    {
                            Thread.Sleep(delayTo * 2);

                            Console.WriteLine("----------------------------------------------------------------");
                            Console.WriteLine("Token with number {0} are banned & deleted from current session!",
                                    rndTokenArray[rndToken]);
                            Console.WriteLine("----------------------------------------------------------------");
                            rndTokenArray = Randomizer.RemoveValue(rndTokenArray, rndTokenArray[rndToken]);
                            qtyTokens--;
                            date = date - 24;
                    }
                    #endregion

                    countOfRequests = 1;
                    requestData = "";
                }
            }
            #endregion

            parsingStatus = "Parsing succesufully completed!";
        }

        // Parsing functions :: Only by Birthday :: Direct token list parsing
        public static void ParsingUsersByBdayDirect(
            out int usersFound,
            out string parsingStatus,
            RequestParams parameters,
            int birthYearFrom,
            int birthYearTo,
            int delayFrom,
            int delayTo,
            bool isAllStatusParsing, // If it true - parse all types of users status
            bool isDownParse,
            string outputFile) // If it true - parse from higher year to lower
        {
            usersFound = 0;
            var i = 0;

            var tokens = File.ReadAllLines(TokensFile);
            parameters.Token = File.ReadAllLines(TokensFile)[0];

            if (isDownParse)
            {
                #region Parsing cycle from down to up (ex. from 2005 to 1950)

                for (var bdayYear = birthYearTo; bdayYear >= birthYearFrom; bdayYear--)
                {
                    Console.WriteLine("\nCurrent year is   : {0}\n", bdayYear);

                    for (var bdayMonth = 1; bdayMonth < 13; bdayMonth++)
                    {
                        Console.WriteLine("\nCurrent month is   : {0}\n", bdayMonth);

                        for (var bdayDay = 1; bdayDay < Month.DaysInMonth[bdayMonth] + 1; bdayDay++)
                        {
                            parameters.BirthDay = bdayDay;
                            parameters.BirthMonth = bdayMonth;
                            parameters.BirthYear = bdayYear;

                            var users = new VkUsers();

                            // Parsing all users from current date by each status type:
                            if (isAllStatusParsing)
                            {
                                for (var statusType = 1; statusType < 8; statusType++)
                                {
                                    parameters.Status = statusType;
                                    users = Utils.GetWriteUsers(parameters, delayFrom, delayTo, outputFile, true);
                                    usersFound += users.response.Count;

                                    Console.WriteLine("Users with status {0} founded : {1}", statusType,
                                        users.response.Count);
                                }
                            }
                            // Parsing all users from current date
                            else
                            {
                                users = Utils.GetWriteUsers(parameters, delayFrom, delayTo, outputFile, true);
                                usersFound += users.response.Count;

                                Console.WriteLine("Users with B-Day {0}.{1}.{2} founded : {3}", bdayDay, bdayMonth,
                                    bdayYear, users.response.Count);
                            }

                            # region Token checking:
                            if (users.response.Count == 0)
                            {
                                i++;
                                var check = Utils.TokenCheckDirectVoid(i, tokens, parameters, bdayDay, users);

                                if (check == 1) bdayDay--;
                                if (check == 2 || check == 3)
                                {
                                    parsingStatus = "Parsing not full completed!";
                                    return;
                                }
                                if (check == 4) bdayDay--;
                            }
                            # endregion
                        }
                    }
                    Console.WriteLine("\nAfter {0} year, finded {1} users : {0}\n", bdayYear, usersFound);
                }
                #endregion
            }
            else
            {
                #region Parsing cycle from up to down (ex. from 1950 to 2005)
                for (var bdayYear = birthYearFrom; bdayYear <= birthYearTo; bdayYear++)
                {
                    Console.WriteLine("\nCurrent year is   : {0}\n", bdayYear);

                    for (var bdayMonth = 1; bdayMonth < 13; bdayMonth++)
                    {
                        Console.WriteLine("\nCurrent month is   : {0}\n", bdayMonth);

                        for (var bdayDay = 1; bdayDay < Month.DaysInMonth[bdayMonth] + 1; bdayDay++)
                        {
                            parameters.BirthDay = bdayDay;
                            parameters.BirthMonth = bdayMonth;
                            parameters.BirthYear = bdayYear;

                            var users = new VkUsers();

                            // Parsing all users from current date by each status type:
                            if (isAllStatusParsing)
                            {
                                for (var statusType = 1; statusType < 8; statusType++)
                                {
                                    parameters.Status = statusType;
                                    users = Utils.GetWriteUsers(parameters, delayFrom, delayTo, outputFile, true);
                                    usersFound += users.response.Count;

                                    Console.WriteLine("Users with staus {0} founded : {1}", statusType, users.response.Count);
                                }
                            }
                            // Parsing all users from current date
                            else
                            {
                                users = Utils.GetWriteUsers(parameters, delayFrom, delayTo, outputFile, true);
                                usersFound += users.response.Count;

                                Console.WriteLine("Users with B-Day {0}.{1}.{2} founded : {3}", bdayDay, bdayMonth, bdayYear, users.response.Count);
                            }

                            # region Token checking:
                            if (users.response.Count == 0)
                            {
                                i++;
                                var check = Utils.TokenCheckDirectVoid(i, tokens, parameters, bdayDay, users);

                                if (check == 1) bdayDay--;
                                if (check == 2 || check == 3)
                                {
                                    parsingStatus = "Parsing not full completed!";
                                    return;
                                }
                                if (check == 4) bdayDay--;
                            }
                            # endregion
                        }
                    }
                    Console.WriteLine("\nAfter {0} year, finded {1} users : {0}\n", bdayYear, usersFound);
                }
                #endregion
            }
            parsingStatus = "Parsing succesufully completed!";
        }

        // Parsing functions :: Only by Birthday :: Random token from list at each parsing
        public static void ParsingUsersByBdayRandom(
            out int usersFound,
            out string parsingStatus,
            RequestParams parameters,
            int birthYearFrom,
            int birthYearTo,
            int delayFrom,
            int delayTo,
            bool isAllStatusParsing, // If it true - parse all types of users status
            bool isDownParse,        // If it true - parse from higher year to lower
            string outputFile)
        {
            usersFound = 0;

            var randomGen = new Random();
            var tokens = File.ReadAllLines(TokensFile);
            var qtyTokens = File.ReadAllLines(TokensFile).Length;

            // Generate random array numbers of tokens for Token Checking
            var rndTokenArray = new int[qtyTokens];
            var rndTokenArrayNums = new Randomizer(0, qtyTokens);
            for (var rnd = 0; rnd < rndTokenArrayNums.Count; rnd++)
                rndTokenArray[rnd] = rndTokenArrayNums.Next();

            if (isDownParse)
            {
                #region Parsing cycle from down to up (ex. from 2005 to 1950)
                for (var bdayYear = birthYearTo; bdayYear >= birthYearFrom; bdayYear--)
                //for (var bdayYear = birthYearFrom; bdayYear <= birthYearTo; bdayYear++)
                {
                    Console.WriteLine("\nCurrent year is   : {0}\n", bdayYear);

                    for (var bdayMonth = 1; bdayMonth < 13; bdayMonth++)
                    {
                        Console.WriteLine("\nCurrent month is   : {0}\n", bdayMonth);

                        for (var bdayDay = 1; bdayDay < Month.DaysInMonth[bdayMonth] + 1; bdayDay++)
                        {
                            if (rndTokenArray.Length == 0)
                            {
                                Console.WriteLine("\nTokens are ended. Parsing is finished!\n");
                                Console.WriteLine("You want to continue parsing? Y/N?");

                                RndDownParse: var a = Console.ReadLine();
                                if (a == "Y" || a == "y")
                                {
                                    // Re-init array of random tokens
                                    qtyTokens = File.ReadAllLines(TokensFile).Length;
                                    rndTokenArray = new int[qtyTokens];
                                    rndTokenArrayNums = new Randomizer(0, qtyTokens);
                                    for (var rnd = 0; rnd < rndTokenArrayNums.Count; rnd++)
                                        rndTokenArray[rnd] = rndTokenArrayNums.Next();
                                }
                                else if (a == "N" || a == "n")
                                {
                                    parsingStatus = "Parsing not full completed!";
                                    return;
                                }
                                else
                                {
                                    Console.WriteLine("Please enter correct answer (Y or N)");
                                    goto RndDownParse;
                                }
                            }

                            var rndToken = randomGen.Next(0, qtyTokens);    // Generate rnd number of token
                            Console.WriteLine("-- Parsing with {0} token:", rndTokenArray[rndToken]);

                            parameters.Token = tokens[rndTokenArray[rndToken]];
                            parameters.BirthDay = bdayDay;
                            parameters.BirthMonth = bdayMonth;
                            parameters.BirthYear = bdayYear;

                            var users = new VkUsers();

                            // Parsing all users from current date by each status type:
                            if (isAllStatusParsing)
                            {
                                for (var statusType = 1; statusType < 8; statusType++)
                                {
                                    parameters.Status = statusType;
                                    users = Utils.GetWriteUsers(parameters, delayFrom, delayTo, outputFile, true);
                                    usersFound += users.response.Count;

                                    Console.WriteLine("Users with status {0} founded : {1}", statusType, users.response.Count);
                                }
                            }
                            // Parsing all users from current date
                            else
                            {
                                users = Utils.GetWriteUsers(parameters, delayFrom, delayTo, outputFile, true);
                                usersFound += users.response.Count;
                                Console.WriteLine("Users with B-Day {0}.{1}.{2} founded : {3}", bdayDay, bdayMonth, bdayYear, users.response.Count);
                            }
                            #region Random token checking:
                            if (users.response.Count == 0)
                            {
                                Thread.Sleep(delayTo * 3);
                                Console.WriteLine("\n Maybe token are blocked. Selected next token\n");
                                Console.WriteLine("Lets try again parsing date: {0}.{1}.{2} \n", bdayDay, bdayMonth, bdayYear);
                                bdayDay--;

                                if (Token.TokenCheckRandom(parameters, delayFrom))
                                {
                                    Console.WriteLine("----------------------------------------------------------------");
                                    Console.WriteLine("Token with number {0} are banned & deleted from current session!", rndTokenArray[rndToken]);
                                    Console.WriteLine("----------------------------------------------------------------");
                                    rndTokenArray = Randomizer.RemoveValue(rndTokenArray, rndTokenArray[rndToken]);
                                    qtyTokens--;
                                }
                                else
                                {
                                    Console.WriteLine("\n Maybe in this date no B-day?\n");
                                    Console.WriteLine("Lets try again parsing next date\n");
                                    bdayDay++;
                                }
                            }
                            #endregion
                        }
                    }
                    Console.WriteLine("\nAfter {0} year, finded {1} users : {0}\n", bdayYear, usersFound);
                }
                #endregion
            }
            else
            {
                #region Parsing cycle from up to down (ex. from 1950 to 2005)
                for (var bdayYear = birthYearFrom; bdayYear <= birthYearTo; bdayYear++)
                {
                    Console.WriteLine("\nCurrent year is   : {0}\n", bdayYear);

                    for (var bdayMonth = 1; bdayMonth < 13; bdayMonth++)
                    {
                        Console.WriteLine("\nCurrent month is   : {0}\n", bdayMonth);

                        for (var bdayDay = 1; bdayDay < Month.DaysInMonth[bdayMonth] + 1; bdayDay++)
                        {
                            if (rndTokenArray.Length == 0)
                            {
                                Console.WriteLine("\nTokens are ended. Parsing is finished!\n");
                                Console.WriteLine("You want to continue parsing? Y/N?");

                                RndUpParse: var a = Console.ReadLine();
                                if (a == "Y" || a == "y")
                                {
                                    // Re-init array of random tokens
                                    qtyTokens = File.ReadAllLines(TokensFile).Length;
                                    rndTokenArray = new int[qtyTokens];
                                    rndTokenArrayNums = new Randomizer(0, qtyTokens);
                                    for (var rnd = 0; rnd < rndTokenArrayNums.Count; rnd++)
                                        rndTokenArray[rnd] = rndTokenArrayNums.Next();
                                }
                                else if (a == "N" || a == "n")
                                {
                                    parsingStatus = "Parsing not full completed!";
                                    return;
                                }
                                else
                                {
                                    Console.WriteLine("Please enter correct answer (Y or N)");
                                    goto RndUpParse;
                                }
                            }

                            var rndToken = randomGen.Next(0, qtyTokens);    // Generate rnd number of token
                            Console.WriteLine("-- Parsing with {0} token:", rndTokenArray[rndToken]);

                            parameters.Token = tokens[rndTokenArray[rndToken]];
                            parameters.BirthDay = bdayDay;
                            parameters.BirthMonth = bdayMonth;
                            parameters.BirthYear = bdayYear;

                            var users = new VkUsers();

                            // Parsing all users from current date by each status type:
                            if (isAllStatusParsing)
                            {
                                for (var statusType = 1; statusType < 8; statusType++)
                                {
                                    parameters.Status = statusType;
                                    users = Utils.GetWriteUsers(parameters, delayFrom, delayTo, outputFile, true);
                                    usersFound += users.response.Count;

                                    Console.WriteLine("Users with status {0} founded : {1}", statusType, users.response.Count);
                                }
                            }
                            // Parsing all users from current date
                            else
                            {
                                users = Utils.GetWriteUsers(parameters, delayFrom, delayTo, outputFile, true);
                                usersFound += users.response.Count;

                                Console.WriteLine("Users with B-Day {0}.{1}.{2} founded : {3}", bdayDay, bdayMonth, bdayYear, users.response.Count);
                            }
                            #region Random token checking:
                            if (users.response.Count == 0)
                            {
                                Thread.Sleep(delayTo * 3);
                                Console.WriteLine("\n Maybe token are blocked. Selected next token\n");
                                Console.WriteLine("Lets try again parsing date: {0}.{1}.{2} \n", bdayDay, bdayMonth, bdayYear);
                                bdayDay--;

                                if (Token.TokenCheckRandom(parameters, delayFrom))
                                {
                                    rndTokenArray = Randomizer.RemoveValue(rndTokenArray, rndTokenArray[rndToken]);
                                    qtyTokens--;
                                }
                                else
                                {
                                    Console.WriteLine("----------------------------------------------------------------");
                                    Console.WriteLine("Token with number {0} are banned & deleted from current session!", rndTokenArray[rndToken]);
                                    Console.WriteLine("----------------------------------------------------------------");
                                    Console.WriteLine("\n Maybe in this date no B-day?\n");
                                    Console.WriteLine("Lets try again parsing next date\n");
                                    bdayDay++;
                                }
                            }
                            #endregion
                        }
                    }
                    Console.WriteLine("\nAfter {0} year, finded {1} users : {0}\n", bdayYear, usersFound);
                }
                #endregion
            }

            parsingStatus = "Parsing succesufully completed!";
        }

        // Parsing functions :: Only by Birthday :: Random token & date from list at each parsing
        public static void ParsingUsersByBdayFullRandom(
            out int usersFound,
            out string parsingStatus,
            RequestParams parameters,
            int birthYearFrom,
            int birthYearTo,
            int delayFrom,
            int delayTo,
            bool isAllStatusParsing,    // If it true - parse all types of users status
            string outputFile) 
        {
            usersFound = 0;
            var randomGen = new Random();
            var tokens = File.ReadAllLines(TokensFile);
            var qtyTokens = File.ReadAllLines(TokensFile).Length;
            var allDates = Month.SetAllDatesFromTo(birthYearFrom, birthYearTo);
            var countOfDates = Month.SetAllDatesFromTo(birthYearFrom, birthYearTo).Length;
            var currentNumOfDate = Randomizer.SetRandomDateNums(birthYearFrom, birthYearTo);
            const char delimeter = '.';

            // Generate random array numbers of tokens for Token Checking
            var rndTokenArray = new int[qtyTokens];
            var rndTokenArrayNums = new Randomizer(0, qtyTokens);
            for (var rnd = 0; rnd < rndTokenArrayNums.Count; rnd++)
                rndTokenArray[rnd] = rndTokenArrayNums.Next();

            #region Parsing cycle with random date & token
            for (var date = 0; date < countOfDates; date++)
            {
                if (rndTokenArray.Length == 0)
                {
                    Console.WriteLine("\nTokens are ended. Parsing is finished!\n");
                    Console.WriteLine("You want to continue parsing? Y/N?");

                    RndFull: var a = Console.ReadLine();
                    if (a == "Y" || a == "y")
                    {
                        // Re-init array of random tokens
                        qtyTokens = File.ReadAllLines(TokensFile).Length;
                        rndTokenArray = new int[qtyTokens];
                        rndTokenArrayNums = new Randomizer(0, qtyTokens);
                        for (var rnd = 0; rnd < rndTokenArrayNums.Count; rnd++)
                            rndTokenArray[rnd] = rndTokenArrayNums.Next();
                    }
                    else if (a == "N" || a == "n")
                    {
                        parsingStatus = "Parsing not full completed!";
                        return;
                    }
                    else
                    {
                        Console.WriteLine("Please enter correct answer (Y or N)");
                        goto RndFull;
                    }
                }

                var rndToken = randomGen.Next(0, qtyTokens);    // Generate rnd number of token

                Console.WriteLine("-- Parsing with {0} token:", rndTokenArray[rndToken]);
                Console.WriteLine("Current Date is: {0} ", allDates[currentNumOfDate[date]]);
                var splitDate = allDates[currentNumOfDate[date]].Split(delimeter);

                parameters.Token = tokens[rndTokenArray[rndToken]];
                parameters.BirthDay = Convert.ToInt32(splitDate[0]);
                parameters.BirthMonth = Convert.ToInt32(splitDate[1]);
                parameters.BirthYear = Convert.ToInt32(splitDate[2]);

                var users = new VkUsers();

                #region Parsing all users from current date by each status type:
                if (isAllStatusParsing)
                {
                    for (var statusType = 1; statusType < 8; statusType++)
                    {
                        parameters.Status = statusType;
                        users = Utils.GetWriteUsers(parameters, delayFrom, delayTo, outputFile, true);
                        usersFound += users.response.Count;

                        Console.WriteLine("Users with status {0} founded : {1}", statusType, users.response.Count);
                    }
                }
                // Parsing all users from current date
                else
                {
                    users = Utils.GetWriteUsers(parameters, delayFrom, delayTo, outputFile, true);
                    usersFound += users.response.Count;

                    Console.WriteLine("Users with B-Day {0} founded : {1}", allDates[currentNumOfDate[date]], users.response.Count);
                }
                #endregion

                #region Random token checking:
                if (users.response.Count == 0)
                {
                    Thread.Sleep(delayTo * 3);
                    Console.WriteLine("\n Maybe token are blocked. Selected next token\n");
                    Console.WriteLine("Lets try again parsing date: {0} \n", allDates[currentNumOfDate[date]]);
                    date--;

                    if (Token.TokenCheckRandom(parameters, delayFrom))
                    {
                        Console.WriteLine("----------------------------------------------------------------");
                        Console.WriteLine("Token with number {0} are banned & deleted from current session!", rndTokenArray[rndToken]);
                        Console.WriteLine("----------------------------------------------------------------");
                        rndTokenArray = Randomizer.RemoveValue(rndTokenArray, rndTokenArray[rndToken]);
                        qtyTokens--;
                    }
                    else
                    {
                        Console.WriteLine("\n Maybe in this date no B-day?\n");
                        Console.WriteLine("Lets try again parsing next date\n");
                        date++;
                    }
                }
                #endregion
            }
            #endregion

            parsingStatus = "Parsing succesufully completed!";
        }

        // Parsing functions :: By Names in Dictionary :: By years old (optional)
        public static void ParsingUsersByDictionaryName(
            out int usersFound,
            out string parsingStatus,
            RequestParams parameters,
            int ageFrom,
            int ageTo,
            int delayFrom,
            int delayTo,
            bool isAllStatusParsing, // If it true - parse all types of users status
            string outputFile)
        {
            usersFound = 0;
            var parsingNames = Utils.GetUsersNames();
            var tokens = File.ReadAllLines(TokensFile);
            parameters.Token = File.ReadAllLines(TokensFile)[0];
            var i = 0;

            foreach (var name in parsingNames)
            {
                for (var age = ageFrom; age <= ageTo; age++)
                {
                    Console.WriteLine("Current username is   : {0}", name);
                    Console.WriteLine("Current user age is   : {0}", age);

                    parameters.Name = name;
                    parameters.AgeFrom = age;
                    parameters.AgeTo = age;

                    var users = new VkUsers();

                    // Parsing all users with current age by each status type:
                    if (isAllStatusParsing)
                    {
                        for (var statusType = 1; statusType < 8; statusType++)
                        {
                            parameters.Status = statusType;
                            users = Utils.GetWriteUsers(parameters, delayFrom, delayTo, outputFile, true);
                            usersFound += users.response.Count;

                            Console.WriteLine("Users with name {0} and status {1} founded : {2}", name, statusType, users.response.Count);
                        }
                    }
                    else
                    {
                        users = Utils.GetWriteUsers(parameters, delayFrom, delayTo, outputFile, true);
                        usersFound += users.response.Count;

                        Console.WriteLine("Users with name {0} founded : {1}", name, users.response.Count);
                    }

                    #region Token checking
                    if (users.response.Count == 0 && !Token.TokenCheck(parameters, delayFrom))
                    {
                        i++;
                        // First tokens selecting
                        if (i < tokens.Length)
                        {
                            parameters.Token = tokens[i];
                            Console.WriteLine("\nToken are blocked. Selected next {0} token\n", i);
                            Console.WriteLine("Lets try again parsing age: {0}\n", age);
                            age--;
                        }
                        // If all tokens in list are lock - check token list again
                        else
                        {
                            i = 0;
                            while (
                                Token.TokenCheck(parameters, delayFrom) == false &&
                                i < tokens.Length)
                            {
                                parameters.Token = tokens[i];
                                i++;

                                // No more working tokens -> exit
                                if (i == tokens.Length)
                                {
                                    Console.WriteLine("\nTokens are ended. Parsing is finished!\n");
                                    parsingStatus = "Parsing not full completed!";
                                    return;
                                }
                            }
                            Console.WriteLine("\nTokens are ended. Try start from {0} token again\n", i);
                            Console.WriteLine("Lets try again parsing age: {0}\n", age);
                            age--;
                        }
                    }
                    #endregion
                }
            }
            parsingStatus = "Parsing succesufully completed!";
        }
    }
}