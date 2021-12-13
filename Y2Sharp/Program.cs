using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Y2Sharp
{
    public class Youtube
    {

        public static async Task Main(string[] args)
        {
            try
            {
                var videoid = Console.ReadLine();

                var videoinfo = new youtube.VideoInfo(videoid);

                Console.WriteLine(videoinfo.Title);
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }


        public static async Task<List<string>> ResolutionsAsync(string videoid)
        {

            if (videoid == string.Empty) { throw new Exception("Videoid was empty"); }
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

                var acceptableResolutions = new List<string>
{
    "144p",
    "240p",
    "360p",
    "480p",
    "720p",
    "1080p",
};
                var resList = acceptableResolutions.Where(r => result.Contains(r)).ToList();


                resList.RemoveAll(item => item == string.Empty);

                return resList;

            }
        }

        public static string Videothumbnailurl(string videoid)
        {

            if (videoid == null) { throw new Exception("videoid was null"); }

            return "https://i.ytimg.com/vi/" + videoid + "/0.jpg";
        }

        public static async Task<string> VideotitleAsync(string videoid)
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
                char backslash = '\u005c';

                var beforetitle = "caption text-left" + backslash.ToString() + "> <b>";

                var title = GetBetween(result, "k_data_vtitle = ", ";");
                title = title.Replace(quote.ToString(), string.Empty);
                title = title.Replace(backslash.ToString(), string.Empty);

                return title;

            }


        }
        //download video
        public static async Task<Stream> GetStreamAsync(string videoid, string type = "mp3", string quality = "128")
        {
            var uri = "https://www.y2mate.com/mates/convert";

            var formContent = new FormUrlEncodedContent(new[]
            {
    new KeyValuePair<string, string>("_id", await GetYInfoAsync(videoid)),
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

                var link = (GetBetween(result, @"href=\" + quote, quote + " rel="));

                link = link.Replace(@"\", string.Empty);

                if(link == string.Empty) { throw new Exception("Error getting file link"); }

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

        public static async Task DownloadAsync(string videoid, string path, string type = "mp3", string quality = "128")
        {
            if (videoid == null) { throw new Exception("videoid was null"); }

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