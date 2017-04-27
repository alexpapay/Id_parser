using System;
using System.IO;
using System.Net;
using System.Text;

namespace Vk_Parser_Console.Classes
{
    public class VkRequest
    {
        // Path to file with proxy list
        private static readonly string ProxyFile = string.Format(Environment.CurrentDirectory + @"\Tokens\Proxy.txt");

        // Vk response full version. Parameters in class RequestParams
        public static string VkSearchResponse(RequestParams parameters)
        {
            // Search init URL:
            var url = "https://api.vk.com/method/users.search?";

            // Adding parameters to request
            if (parameters.Name != "0") url += string.Format("&q={0}", parameters.Name);
            url += string.Format("&count={0}", parameters.Count);
            if (parameters.City != 0) url += string.Format("&city={0}", parameters.City);
            if (parameters.Country != 0) url += string.Format("&country={0}", parameters.Country);
            if (parameters.Sex != 0) url += string.Format("&sex={0}", parameters.Sex);
            if (parameters.Status != 0) url += string.Format("&status={0}", parameters.Status);
            if (parameters.AgeFrom != 0) url += string.Format("&age_from={0}", parameters.AgeFrom);
            if (parameters.AgeTo != 0) url += string.Format("&age_to={0}", parameters.AgeTo);
            if (parameters.BirthDay != 0) url += string.Format("&birth_day={0}", parameters.BirthDay);
            if (parameters.BirthMonth != 0) url += string.Format("&birth_month={0}", parameters.BirthMonth);
            if (parameters.BirthYear != 0) url += string.Format("&birth_year={0}", parameters.BirthYear);
            if (parameters.Online != 0) url += string.Format("&online={0}", parameters.Online);
            if (parameters.Interests != "0") url += string.Format("&interests={0}", parameters.Interests);
            if (parameters.Position != "0") url += string.Format("&position={0}", parameters.Position);

            // Adding token to request
            url = url + string.Format("&access_token={0}", parameters.Token);

            var request = (HttpWebRequest)WebRequest.Create(url);

            // Working with proxy servers :: Turn On
            const char delimeter = ';';
            var allProxy = File.ReadAllLines(ProxyFile);
            var proxyRnd = new Random();
            var proxyNum = proxyRnd.Next(0, allProxy.Length);
            var splitProxy = allProxy[proxyNum].Split(delimeter);

            var proxy = new WebProxy(splitProxy[0], Convert.ToInt32(splitProxy[1]));
            proxy.Credentials = new NetworkCredential(splitProxy[2], splitProxy[3]);
            //request.Proxy = proxy;

            var response = (HttpWebResponse)request.GetResponse();
            var reader = new StreamReader(response.GetResponseStream());
            var responseText = reader.ReadToEnd();

            return responseText;
        }

        // Vk response full version. Parameters in class RequestParams
        public static string VkExecuteResponse(RequestParams parameters, int countOfRequest, string requestDate)
        {
            // Search init URL:
            var url = "https://api.vk.com/method/execute?";
            // Adding token & API version to request:
            var urlPost = string.Format("access_token={0}&v=5.60", parameters.Token);
            // Adding all dates:
            urlPost += "&code=";
            urlPost += requestDate;
            // Adding result line:
            urlPost += "return%20result1";
            for (var i = 2; i < countOfRequest; i++)
            {
                urlPost += string.Format("%2bresult{0}", i);
            }
            urlPost += "%3b";

            var request = (HttpWebRequest)WebRequest.Create(url);

            // Working with proxy servers :: Turn On
            const char delimeter = ';';
            var allProxy = File.ReadAllLines(ProxyFile);
            var proxyRnd = new Random();
            var proxyNum = proxyRnd.Next(0, allProxy.Length);
            var splitProxy = allProxy[proxyNum].Split(delimeter);

            var proxy = new WebProxy(splitProxy[0], Convert.ToInt32(splitProxy[1]));
            proxy.Credentials = new NetworkCredential(splitProxy[2], splitProxy[3]);
            //request.Proxy = proxy;

            request.Method = "POST";
            byte[] byteArray = Encoding.GetEncoding(1251).GetBytes(urlPost);
            request.ContentLength = byteArray.Length;
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            var response = (HttpWebResponse)request.GetResponse();
            var reader = new StreamReader(response.GetResponseStream());
            var responseText = reader.ReadToEnd();

            return responseText;
        }
    }
}