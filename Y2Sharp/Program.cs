using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Y2Sharp
{
    public class youtube
    {
     
        
       

        public static async Task Main(string[] args)
        {
            
        }

        

        public static bool DebugMode = false;


        public static async Task<List<string>> ResolutionsAsync(string videoid)
        {
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
            
            if(videoid == null) { throw new Exception("videoid was null"); }
            
            return ("https://i.ytimg.com/vi/" + videoid + "/0.jpg");
        }

        public static async Task<string> VideotitleAsync(string videoid)
        {
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
        public static async Task DownloadAsync(string videoid, string path, string type = "mp3", string quality = "128")
        {
            var uri = "https://www.y2mate.com/mates/convert";


            


            var formContent = new FormUrlEncodedContent(new[]
            {
    new KeyValuePair<string, string>("_id", await getvidAsync(videoid)),
    new KeyValuePair<string, string>("ajax", "1"),
    new KeyValuePair<string, string>("fquality", quality),
    new KeyValuePair<string, string>("ftype", type),
    new KeyValuePair<string, string>("token", ""),
    new KeyValuePair<string, string>("type", "youtube"),
    new KeyValuePair<string, string>("v_id", videoid)
});
            

            var myHttpClient = new HttpClient();
            var response = await myHttpClient.PostAsync(uri.ToString(), formContent);

            if (DebugMode)
            {
                Console.WriteLine(await response.Content.ReadAsStringAsync());
            }

            var errorcontent = await response.Content.ReadAsStringAsync();
            if(errorcontent.Contains("Try again in"))
            {
                throw new Exception("y2mate.com returned Try again in 5 seconds. Might be cause by wrong video id");
            }
            if(errorcontent.Contains("Press f5 to try again."))
            {
                throw new Exception("y2mate.com returned Press f5 to try again. Might be caused by wrong quality or type");
            }
            



            
            
            using (var streamReader = new StreamReader(await response.Content.ReadAsStreamAsync(), encoding: Encoding.UTF8))
            {
                    string result = streamReader.ReadToEnd();


                    char quote = '\u0022';

                    var link = (GetBetween(result, @"href=\" + quote, quote + " rel="));
                    //Console.WriteLine(link);
                link = link.Replace(@"\", string.Empty);
                if (DebugMode)
                {
                    Console.WriteLine(link);
                }



                var httpClient = new HttpClient();

                    using (var stream = await httpClient.GetStreamAsync(link))
                    {
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                        await stream.CopyToAsync(fileStream);

                            if (DebugMode)
                        {
                            Console.WriteLine(Path.Combine(Directory.GetCurrentDirectory(), path).ToString());
                        }
                        
                    }

                }

                using (var errorstream = await httpClient.GetStreamAsync(link))
                {
                  
                        using (var errorreader = new StreamReader(errorstream))
                        {
                            var error = await errorreader.ReadToEndAsync();
                            Console.BackgroundColor = ConsoleColor.Red;
                           // Console.WriteLine(error);
                            Console.ResetColor();
                            if (error == "Your session has expired.")
                            {
                                throw new Exception(@"y2mate.com returned Your session has expired. This happends sometimes and i dont know why but its because of y2mate :D");
                            }
                        }

                }

            }
       

        }

        private static async Task<string> getvidAsync(string videoid)
        {
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

                if (DebugMode)
                {
                    Console.WriteLine(id);
                }

                if(id == string.Empty)
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
