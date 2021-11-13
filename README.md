# Y2Sharp

 !!WARNING!! this api is still under development. it does work but doesn't give any error data and also some other things!!!
 
 [![.NET](https://github.com/ArttuKuikka/Y2Sharp/actions/workflows/dotnet.yml/badge.svg)](https://github.com/ArttuKuikka/Y2Sharp/actions/workflows/dotnet.yml)

Fastest Youtube downloader api for C# using Y2Mate.com

Created because i found all other youtube downloader apis were too slow for my need so i decided to create my own using the y2mate.com website.

Sample code

```
await Y2Sharp.youtube.DownloadAsync("gQjAEbWZEgU", "music.mp3");
```

install the latest nuget from https://github.com/ArttuKuikka/Y2Sharp/releases and you can use the command above. 

explanations to all the parameters

```
DownloadAsync(videoid, path, type, quality);
```
1. videoid is the part of youtube link after watch?v=
2. path. where you want your downloaded file to be stored (by default force creates file so be carefull)
3. type. either mp3 or mp4. determines if downloaded content is audio or video.
4. quality. for music the quality should allways be 128 but videos can be downloaded at any of youtubes default resulutions.
