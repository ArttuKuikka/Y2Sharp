using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Y2Sharp
{
    internal class program
    {

        public static async Task Main(string[] args)
        {
            try
            {
                var videoid = Console.ReadLine();

                var video = new youtube.Video(videoid);

            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                using (var stream = await GetStreamAsync(videoid, type, quality))
                {
                    await stream.CopyToAsync(fileStream);
                }

            }
        }

        private static async Task<string> GetYInfoAsync(string videoid)
        {
            if (videoid == null) { throw new Exception("videoid was null"); }

            var url = "https://www.y2mate.com/mates/analyze/ajax";
            var yturl = "https://www.youtube.com/watch?v=" + videoid;

            var formContent = new FormUrlEncodedContent(new[]
          {

    new KeyValuePair<string, string>("url", yturl),
    new KeyValuePair<string, string>("q_auto", "1"),
    new KeyValuePair<string, string>("ajax", "1")
});

            var myHttpClient = new HttpClient();
            var response = await myHttpClient.PostAsync(url.ToString(), formContent);

            using (var streamReader = new StreamReader(await response.Content.ReadAsStreamAsync(), encoding: Encoding.UTF8))
            {
                string result = streamReader.ReadToEnd();


                char quote = '\u0022';


                var id = (GetBetween(result, @"var k__id = \", "; var video_service"));

                id = id.Replace(quote.ToString(), string.Empty);
                id = id.Replace(@"\", string.Empty);

               

                if (id == string.Empty)
                {
                    throw new Exception("Error getting __id from y2mate.com");
                }

                return (id);

            }
        }

        private static string GetBetween(string strSource, string strStart, string strEnd)
        {
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                int Start, End;
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }

            return "";
        }


    }
}