namespace Vk_Parser_Console.Classes
{
    public class Month
    {
        public static int[] DaysInMonth =
        {       
            0,  // Offset
            31, // Jan
            28, // Feb    
            31, // Mar
            30, // Apr
            31, // May
            30, // Jun
            31, // Jul
            31, // Aug
            30, // Sep
            31, // Oct
            30, // Nov
            31  // Dec
        };

        // Generate array with dates From To year
        public static string[] SetAllDatesFromTo(int yearFrom, int yearTo)
        {
            var count = 0;
            var maxRandom = (yearTo - yearFrom + 1) * 365;
            var datesArray = new string[maxRandom];

            for (var bdayYear = yearFrom; bdayYear <= yearTo; bdayYear++)
            {
                for (var bdayMonth = 1; bdayMonth < 13; bdayMonth++)
                {
                    for (var bdayDay = 1; bdayDay < DaysInMonth[bdayMonth] + 1; bdayDay++)
                    {
                        datesArray[count] = bdayDay + "." + bdayMonth + "." + bdayYear;
                        count++;
                    }
                }
            }
            return datesArray;
        }
    }
}