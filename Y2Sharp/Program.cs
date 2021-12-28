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

            var videoid = Console.ReadLine();

            await Y2Sharp.Youtube.Video.GetInfo(videoid);

            var video = new Y2Sharp.Youtube.Video();

            Console.WriteLine(video.Title);
            Console.WriteLine(video.ThumbnailURL);
            Console.WriteLine(video.Url);

            await video.DownloadAsync("Music.mp3");


        }


    }   
}            