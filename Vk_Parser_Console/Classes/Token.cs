using System.Threading;

namespace Vk_Parser_Console.Classes
{
    public class Token
    {
        // Token checking for Name parsing :: Find one Ivan in Moscow
        public static bool TokenCheck(RequestParams parameters, int delayFrom)
        {
            Thread.Sleep(delayFrom);

            VkUsers.RequestParamsInit(parameters);
            parameters.Name = "Иван";
            parameters.City = 1;
            parameters.Count = 1;

            var response = VkRequest.VkSearchResponse(parameters);
            var users = VkUsers.VkDeserealiseResponse(response);
            if (users == null || users.response.Count == 0) return false;
            return users.response.Count > 0;
        }

        // Token checking for Random parsing :: Check nearest day
        public static bool TokenCheckRandom(RequestParams parameters, int delayFrom)
        {
            Thread.Sleep(delayFrom);

            // Check for start or end of month:
            if (parameters.BirthDay < Month.DaysInMonth[parameters.BirthMonth]) ++parameters.BirthDay;
            else --parameters.BirthDay;

            var response = VkRequest.VkSearchResponse(parameters);
            var users = VkUsers.VkDeserealiseResponse(response);
            if (users == null || users.response.Count == 0) return true;
            return users.response.Count == 0;
        }
    }
}