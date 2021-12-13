using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Y2Sharp.youtube
{
    internal class VideoInfo
    {

        public string Title { get; set; }
        public string Description { get; set; }
        public string ThumbnailURL { get; set; }
        public string ChannelName { get; set; }
        public List<string> Resolutions { get; set; }
        public double Lenght { get; set; }

        private string Y2MateResponse;

        public VideoInfo(string videoid)
        {
            if (videoid == null) { throw new Exception("videoid was null"); }

            var ajaxurl = "https://www.y2mate.com/mates/analyze/ajax";
            var yturl = "https://www.youtube.com/watch?v=" + videoid;

            GetY2MateResponse(yturl, ajaxurl);

            Title = GetTitle(Y2MateResponse);
            
        }

        private async Task GetY2MateResponse(string yturl, string ajaxurl)
        {
            var formContent = new FormUrlEncodedContent(new[]
        {

    new KeyValuePair<string, string>("url", yturl),
    new KeyValuePair<string, string>("q_auto", "1"),
    new KeyValuePair<string, string>("ajax", "1")
});

            var myHttpClient = new HttpClient();
            var response = await myHttpClient.PostAsync(ajaxurl.ToString(), formContent);

            using (var streamReader = new StreamReader(await response.Content.ReadAsStreamAsync(), encoding: Encoding.UTF8))
            {
                string result = streamReader.ReadToEnd();

                Y2MateResponse = result;
            }
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
    }
}
