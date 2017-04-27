using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Vk_Parser_Console.Classes
{
    [JsonObject]
    public class VkId
    {
        [JsonProperty(PropertyName = "response")]
        public int[] response { get; set; }
    }

    [JsonObject]
    public class VkUsers
    {
        [JsonProperty(PropertyName = "response")]
        public List<UsersResponse> response { get; set; }

        public static VkUsers VkDeserealiseResponse(string str)
        {
            try
            {
                var jr = new JsonTextReader(new StringReader(str));
                var jObject = JObject.Load(jr);
                var m = new VkUsers
                {
                    response =
                        jObject["response"].Children()
                            .Where(c => c.HasValues)
                            .Select(c => c.ToObject<UsersResponse>())
                            .ToList()
                };
                return m;
            }
            catch (Exception ex)
            {
                Console.WriteLine("VkDeserealiseResponce Exception : {0}", ex);
                return null;
            }
        }

        public List<RequestParams> RequestList { get; set; }

        // Re-init request parameters
        public static void RequestParamsInit(RequestParams parameter)
        {
            parameter.Token = "0";
            parameter.Name = "0";
            parameter.Count = 1000;
            parameter.City = 0;
            parameter.Country = 0;
            parameter.Sex = 0;
            parameter.Status = 0;
            parameter.AgeFrom = 0;
            parameter.AgeTo = 0;
            parameter.BirthDay = 0;
            parameter.BirthMonth = 0;
            parameter.BirthYear = 0;
            parameter.Online = 0;
            parameter.Interests = "0";
            parameter.Position = "0";
        }
    }

    [JsonObject]
    public class UsersResponse
    {
        [JsonProperty(PropertyName = "uid")]
        public int uid { get; set; }

        [JsonProperty(PropertyName = "first_name")]
        public string first_name { get; set; }

        [JsonProperty(PropertyName = "last_name")]
        public string last_name { get; set; }
    }

    public class RequestParams
    {
        public string Token;        // Program token
        public string Name;         // Строка поискового запроса. Например, Вася Бабич
        public int Count;           // Количество возвращаемых пользователей 
        public int City;            // Идентификатор города
        public int Country;         // Идентификатор страны
        public int Sex;             // Пол
        public int Status;          // Семейное положение
        public int AgeFrom;         // Возраст от
        public int AgeTo;           // Возраст до
        public int BirthDay;        // День рождения
        public int BirthMonth;      // Месяц рождения
        public int BirthYear;       // Год рождения
        public int Online;          // Учитывать ли статус «онлайн»
        public string Interests;    // Интересы
        public string Position;     // Должность
    }
}