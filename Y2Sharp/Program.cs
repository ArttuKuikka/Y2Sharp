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

                var video = new youtube.Video(videoid);

                Console.WriteLine(video.Title);
                Console.WriteLine(video.ThumbnailURL);
                foreach(var res in video.Resolutions)
                {
                    Console.WriteLine(res);
                }
                Console.WriteLine(video.Url);

                await video.DownloadAsync(video.Title + ".mp3");

               
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}