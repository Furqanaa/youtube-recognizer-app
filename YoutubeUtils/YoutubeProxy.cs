using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeExplode;
using Models;
using System.IO;
using System.Reflection;
using YoutubeExplode.Videos.Streams;

namespace YoutubeUtils
{
    public class YoutubeProxy
    {
        private string baseDir = Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory);
        string CutInterval = System.Configuration.ConfigurationManager.AppSettings["CutInterval"] ?? "20";
        private YoutubeClient youTubeClient = new YoutubeClient();
        
        public async Task<Video> GetVideoInfoByUrl(string url)
        {
            var video = await this.youTubeClient.Videos.GetAsync(url);
            Video currentViewModel = new Video();
            currentViewModel.Id = video.Id;
            currentViewModel.Author = video.Author;
            currentViewModel.Description = video.Description;
            currentViewModel.Duration = video.Duration;
            currentViewModel.Keywords = video.Keywords;
            currentViewModel.Title = video.Title;
            currentViewModel.UploadDate = video.UploadDate;
            return currentViewModel;
        }

        // This function is not used by the code, it's just part of the functionality of this class.
        public async Task<VideoFile> DownloadVideoByUrl(string url)
        {
            Video currentViewModel = await this.GetVideoInfoByUrl(url);
            var streamManifest = await this.youTubeClient.Videos.Streams.GetManifestAsync(currentViewModel.Id);
            // Get highest quality muxed stream
            var streamInfo = streamManifest.GetMuxed().WithHighestVideoQuality();
            if (streamInfo != null) {
                // Get the actual stream
                var stream = await this.youTubeClient.Videos.Streams.GetAsync(streamInfo);
                // Download the stream to file
                await this.youTubeClient.Videos.Streams.DownloadAsync(streamInfo, baseDir + "\\wwwroot\\Downloads\\Audio\\file_" + currentViewModel.Id +".mp4");
            }
            return new VideoFile();        
        }


        public async Task<AudioFile> DownloadAudioByUrl(string url)
        {
            Video currentViewModel = await this.GetVideoInfoByUrl(url);
            var streamManifest = await this.youTubeClient.Videos.Streams.GetManifestAsync(currentViewModel.Id);
             // Get only audio 
            var streamInfo = streamManifest
                            .GetAudioOnly().WithHighestBitrate();
             if (streamInfo != null) {
                // Get the actual stream
                var stream = await this.youTubeClient.Videos.Streams.GetAsync(streamInfo);
                // Download the stream to file
                await this.youTubeClient.Videos.Streams.DownloadAsync(streamInfo, baseDir + "\\wwwroot\\Downloads\\Audio\\file_" + currentViewModel.Id + ".mp4");
             }

             return new AudioFile() {
                ext = ".mp4",
                fileName = "file_" + currentViewModel.Id + ".mp4" ,
                filePath = baseDir + "\\wwwroot\\Downloads\\Audio\\file_" + currentViewModel.Id + ".mp4"
             };
        }


        public AudioFile SplitAudioFile(AudioFile audioFileViewModel)
        {
            AudioProxy audioLogic = new AudioProxy();
            string newSplitedFileName = audioFileViewModel.fileName.Replace("." + audioFileViewModel.ext, "_20sec.mp3");
            string newSplitedFilePath = audioFileViewModel.filePath.Replace("." + audioFileViewModel.ext, "_20sec.mp3");
            int cutAudioInterval = int.Parse(CutInterval);
            audioLogic.TrimWavFile(audioFileViewModel.filePath, audioFileViewModel.ext,
                newSplitedFilePath, "mp3", 0, cutAudioInterval);
            return new AudioFile()
            {
                ext = "mp3",
                fileName = newSplitedFileName,
                filePath = newSplitedFilePath
            };
        }

        public async Task<Artist> RecognizeAudioFile(AudioFile audioFileViewModel)
        {
            RecognizeProxy recognizeLogic = new RecognizeProxy();
            Artist artistViewModel = recognizeLogic.GetArtist(audioFileViewModel.filePath);
            return artistViewModel;
        }


        public async Task<List<Snippet>> SearchYoutubeSnippets(string artistName)
        {
            YoutubeApiLogic youtubeApiLogic = new YoutubeApiLogic();
            List<Snippet> snippetViewModelList = await youtubeApiLogic.Search(artistName);
            return snippetViewModelList;
        }
    }
}
