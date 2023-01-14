
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;


namespace Y2Sharp.Youtube
{
    public class Video
    {

        public string Title { get; }
        public string ThumbnailURL { get; }
        public List<Resolutions> Resolutions { get; }
        public string Url { get; }

        public string Id { get; }

        private static string video_id { get; set; }
        
        private static string resp { get; set; }

        private static string y2id { get; set; }

        public delegate void ProgressChangedHandler(long? FinalFileSize, long CurrentFileSize, double? progressPercentage);

        public event ProgressChangedHandler ProgressChanged;





        public Video()
        {
            if (video_id == null) { throw new Exception("videoid was null. did you forget to GetInfo before running this"); }

            Id = video_id;

            var yturl = "https://www.youtube.com/watch?v=" + video_id;

            Title = GetTitle(resp);

            ThumbnailURL = VideoThumbnailURL(video_id);
            
            Resolutions = ResList(resp);

            Url = yturl;
           
        }

    

        public static async Task GetInfo(string videoid) //Gets the raw Y2Mate response containing video title, thumbnail url and resolutions
        {
            if (videoid == null) { throw new Exception("videoid was null"); }

            if(videoid.Length != 11) { throw new Exception("videoid was too short or long"); }

            video_id = videoid;

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
                resp = result;

                char quote = '\u0022';


                var id = (Tools.GetBetween(result, @"var k__id = \", "; var video_service"));

                id = id.Replace(quote.ToString(), string.Empty);
                id = id.Replace(@"\", string.Empty);



                if (id == string.Empty)
                {
                    throw new Exception("Error getting __id from y2mate.com. did you run GetInfo with await?");
                }

               y2id = id;

            }

        }

        private string GetTitle(string HttpResponse)
        {
            char quote = '\u0022';
            char backslash = '\u005c';

            var title = Tools.GetBetween(HttpResponse, "k_data_vtitle = ", ";");
            title = title.Replace(quote.ToString(), string.Empty);
            title = title.Replace(backslash.ToString() + backslash.ToString(), backslash.ToString());



            var unicodetitle = System.Net.WebUtility.HtmlDecode(title);

            return unicodetitle;

        }
        private string VideoThumbnailURL(string videoid)
        {
            return "https://i.ytimg.com/vi/" + videoid + "/0.jpg";
        }



       private List<Resolutions> ResList(string HttpResponse)
        {
            var resp = HttpResponse;
            
            

            char backslash = '\u005c';

            resp = resp.Replace(backslash.ToString(), string.Empty);

            resp = resp.Replace("<thead> <tr> <th>Resolution</th> <th>FileSize</th> <th>Download</th> </tr> </thead>", string.Empty);


            

            var combos = new List<Resolutions>();

            for(int i = 0; i < 10; i++)
            {
                var fullresp = Tools.GetBetween(resp, "<tr>", "/tr>");
                

                
                

                char quote = '\u0022';

                var rep1 = $"rel={quote}nofollow{quote}> ";
                var rep2 = " (.mp4)</a>";
                var rep3 = $"data-fquality={quote}";
                var rep4 = $"{quote}>";
                var rep5 = "</td> <td>";
                var rep6 = " MB</td>";

                string parsedres = "";

                string size = Tools.GetBetween(fullresp, rep5, rep6);

                
                size = size.Replace(".", ",");


                if (fullresp.Contains($"(.mp4) <span class={quote}label"))
                {
                    parsedres = Tools.GetBetween(fullresp, rep3, rep4);
                }
                else
                {
                    parsedres = Tools.GetBetween(fullresp, rep3, rep4);
                }

                if(size == "")
                {
                    size = "0";
                }

                if(parsedres.Length > 1)
                {
                    if (parsedres[parsedres.Length - 1] != 'p')
                    {
                        parsedres += "p";
                    }
                }
               
                    var res = new Resolutions()
                    {
                        res = parsedres,
                        sizeasmb = Convert.ToDouble(size)
                        
                    };

                    combos.Add(res);
                

                


                string deletestring = "<tr>" + fullresp + "/tr>";
                

                resp = resp.Replace(deletestring, string.Empty);

                

            }

            combos.RemoveAll(item => item.res == string.Empty);
            combos.RemoveAll(item => item.res == "128p");
            return combos;
        }



        public async Task<Stream> GetStreamAsync(string type = "mp3", string quality = "128")
        {
            
            var link = await GetDownloadURL(type, quality);
            using (var client = new HttpClientDownloadWithProgress(link, "myvideo.mp4"))
            {
                if (ProgressChanged != null)
                {
                    client.ProgressChanged += (totalFileSize, totalBytesDownloaded, progressPercentage) => {
                        ProgressChanged(totalFileSize, totalBytesDownloaded, progressPercentage);
                    };
                }

                await client.StartDownload();


                var striimi = client.MyStream;
                striimi.Position = 0;

                return striimi;
            }
        }

        public async Task<string> GetDownloadURL(string type = "mp3", string quality = "128")
        {
            var uri = "https://www.y2mate.com/mates/convert";

            var formContent = new FormUrlEncodedContent(new[]
            {
    new KeyValuePair<string, string>("_id", y2id),
    new KeyValuePair<string, string>("ajax", "1"),
    new KeyValuePair<string, string>("fquality", quality),
    new KeyValuePair<string, string>("ftype", type),
    new KeyValuePair<string, string>("token", ""),
    new KeyValuePair<string, string>("type", "youtube"),
    new KeyValuePair<string, string>("v_id", video_id)
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

                var link = (Tools.GetBetween(result, @"href=\" + quote, quote + " rel="));

                link = link.Replace(@"\", string.Empty);

                if (link == string.Empty) { throw new Exception("Error getting file link"); }

                return link;



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
