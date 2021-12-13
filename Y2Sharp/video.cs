using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Y2Sharp.youtube
{
    internal class video
    {
        public static async Task<Stream> GetStreamAsync(string videoid, string type = "mp3", string quality = "128")
        {
            var uri = "https://www.y2mate.com/mates/convert";

            var formContent = new FormUrlEncodedContent(new[]
            {
    new KeyValuePair<string, string>("_id", await Info.GetYInfoAsync(videoid)),
    new KeyValuePair<string, string>("ajax", "1"),
    new KeyValuePair<string, string>("fquality", quality),
    new KeyValuePair<string, string>("ftype", type),
    new KeyValuePair<string, string>("token", ""),
    new KeyValuePair<string, string>("type", "youtube"),
    new KeyValuePair<string, string>("v_id", videoid)
});


            var myHttpClient = new HttpClient();
            var response = await myHttpClient.PostAsync(uri.ToString(), formContent);


            var errorcontent = await response.Content.ReadAsStringAsync();
            if (errorcontent.Contains("Try again in"))
            {
                throw new Exception("y2mate.com returned Try again in 5 seconds. Might be cause by wrong video id");
            }
            if (errorcontent.Contains("Press f5 to try again."))
            {
                throw new Exception("y2mate.com returned Press f5 to try again. Might be caused by wrong quality or type");
            }


            using (var streamReader = new StreamReader(await response.Content.ReadAsStreamAsync(), encoding: Encoding.UTF8))
            {
                string result = streamReader.ReadToEnd();

                char quote = '\u0022';

                var link = (GetBetween.GetBetweenStrings(result, @"href=\" + quote, quote + " rel="));

                link = link.Replace(@"\", string.Empty);

                if (link == string.Empty) { throw new Exception("Error getting file link"); }

                var httpClient = new HttpClient();

                using (var httpStream = await httpClient.GetStreamAsync(link))
                {
                    var stream = new MemoryStream();

                    await httpStream.CopyToAsync(stream);
                    stream.Flush();
                    stream.Position = 0;

                    return stream;

                }
            }
        }

        public video()
        {
            

          
           

        }
    }
}
