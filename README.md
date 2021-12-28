# Y2Sharp

![logo](https://user-images.githubusercontent.com/75498768/145097969-f49278db-7412-4368-b462-43e11cc50c86.png)

 
 
 

Fastest Youtube downloader api for C# using Y2Mate.com

Created because i found all other youtube downloader apis were too slow for my needs so i decided to create my own using the y2mate.com website.

Sample code

```
await Y2Sharp.Youtube.Video.GetInfo("videoid !!change this to your video id!!");

var video = new Y2Sharp.Youtube.Video();

await video.DownloadAsync("Music.mp3");
```

other info:
```
video.Title

video.ThumbnailURL

video.Url

```


There is also a WPF demo of this project under the folder Y2SharpWPF

install the latest nuget from https://www.nuget.org/packages/Y2Sharp/ and you can use the command above. 

explanations to all the parameters

```
DownloadAsync(path, type, quality);
```
1. videoid is the part of youtube link after watch?v=
2. path. where you want your downloaded file to be stored (by default force creates file so be carefull)
3. type. either mp3 or mp4. determines if downloaded content is audio or video.
4. quality. for music the quality should allways be 128 but videos can be downloaded at any of youtubes default resulutions.


If you are having problems please open a new issue or message me in discord Arttu#6180
