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
        public static double ConvertBytesToMegabytes(long bytes)
        {
            return (bytes / 1024f) / 1024f;
        }
        public static async Task Main(string[] args)
        {

            var videoid = Console.ReadLine();

            await Y2Sharp.Youtube.Video.GetInfo(videoid);

            var video = new Y2Sharp.Youtube.Video();

            Console.WriteLine(video.Title);
            Console.WriteLine(video.ThumbnailURL);
            Console.WriteLine(video.Url);
            Console.Write(video.Id);
            Console.WriteLine(video.Resolutions.Max().ToString());

            Console.BackgroundColor = ConsoleColor.Red;
            foreach(var res in video.Resolutions)
            {
                Console.WriteLine(res.res + " " + res.sizeasmb + "MB");
            }
            Console.ResetColor();

            video.ProgressChanged += (FinalFileSize, CurrentFileSize, progressPercentage) =>
            {

                Console.WriteLine(Math.Round(ConvertBytesToMegabytes((long)CurrentFileSize), 2).ToString() + "MB / " + Math.Round(ConvertBytesToMegabytes((long)FinalFileSize), 2).ToString() + "MB " + progressPercentage.ToString() + "%");
            };



            await video.DownloadAsync("test.mp4", "mp4", "720");

            






        }

        


    }   
}            