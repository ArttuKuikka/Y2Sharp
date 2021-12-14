using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Y2Sharp.youtube
{
    internal class Info
    {
        public static async Task<string> GetY2MateID(string videoid) //gets _id from y2mate ajax to be used for downloading the video
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


                var id = (GetBetween.GetBetweenStrings(result, @"var k__id = \", "; var video_service"));

                id = id.Replace(quote.ToString(), string.Empty);
                id = id.Replace(@"\", string.Empty);



                if (id == string.Empty)
                {
                    throw new Exception("Error getting __id from y2mate.com");
                }

                return (id);

            }
        }
    }
}
