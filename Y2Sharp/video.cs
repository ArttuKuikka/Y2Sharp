using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Y2Sharp.Youtube
{
    public class Video
    {

        public string Title { get; }
        public string ThumbnailURL { get; }
        public List<string> Resolutions { get; }
        public string Url { get; }

        private string _videoid;
        
        

        public Video(string videoid)
        {
            if (videoid == null) { throw new Exception("videoid was null"); }

            _videoid = videoid;

            var ajaxurl = "https://www.y2mate.com/mates/analyze/ajax";
            var yturl = "https://www.youtube.com/watch?v=" + videoid;

            var Y2MateResponse = GetY2MateResponse(yturl, ajaxurl).GetAwaiter().GetResult();

            Title = GetTitle(Y2MateResponse);

            ThumbnailURL = VideoThumbnailURL(videoid);
            
            Resolutions = ResList(Y2MateResponse);

            Url = yturl;
           
        }

        private async Task<string> GetY2MateResponse(string yturl, string ajaxurl) //Gets the raw Y2Mate response containing video title, thumbnai url and resolutions
        {
            var formContent = new FormUrlEncodedContent(new[]
        {

    new KeyValuePair<string, string>("url", yturl),
    new KeyValuePair<string, string>("q_auto", "1"),
    new KeyValuePair<string, string>("ajax", "1")
});

            var myHttpClient = new HttpClient();
            var response = await myHttpClient.PostAsync(ajaxurl.ToString(), formContent);

            return await response.Content.ReadAsStringAsync();
        }

        private string GetTitle(string HttpResponse)
        {
            char quote = '\u0022';
            char backslash = '\u005c';

            var title = GetBetween.GetBetweenStrings(HttpResponse, "k_data_vtitle = ", ";");
            title = title.Replace(quote.ToString(), string.Empty);
            title = title.Replace(backslash.ToString(), string.Empty);

            return title;

        }
        private string VideoThumbnailURL(string videoid)
        {
            return "https://i.ytimg.com/vi/" + videoid + "/0.jpg";
        }



       private List<string> ResList(string HttpResponse)
        {
            var acceptableResolutions = new List<string>
{
    "144p",
    "240p",
    "360p",
    "480p",
    "720p",
    "1080p",
};
            var resList = acceptableResolutions.Where(r => HttpResponse.Contains(r)).ToList();


            resList.RemoveAll(item => item == string.Empty);

            return resList;
        }



        public async Task<Stream> GetStreamAsync(string type = "mp3", string quality = "128")
        {
            var uri = "https://www.y2mate.com/mates/convert";

            var formContent = new FormUrlEncodedContent(new[]
            {
    new KeyValuePair<string, string>("_id", await Info.GetY2MateID(_videoid)),
    new KeyValuePair<string, string>("ajax", "1"),
    new KeyValuePair<string, string>("fquality", quality),
    new KeyValuePair<string, string>("ftype", type),
    new KeyValuePair<string, string>("token", ""),
    new KeyValuePair<string, string>("type", "youtube"),
    new KeyValuePair<string, string>("v_id", _videoid)
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


        public async Task DownloadAsync(string path, string type = "mp3", string quality = "128")
        {
          

            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                using (var stream = await GetStreamAsync(type, quality))
                {
                    await stream.CopyToAsync(fileStream);
                }

            }
        }


    }
}
