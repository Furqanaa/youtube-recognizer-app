using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace YoutubeUtils
{
    public class YoutubeApiLogic
    {
        string YoutubeApiKey = System.Configuration.ConfigurationManager.AppSettings["YoutubeApiKey"] ?? "AIzaSyDMwRt9o3YSbQ343-Lyk4Q8GBt8kG6sqAI";
        public async Task<List<Snippet>> Search(string artist)
        {
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = YoutubeApiKey,
                ApplicationName = this.GetType().ToString()
            });

            var searchListRequest = youtubeService.Search.List("snippet");
            searchListRequest.Q = artist; // Replace with your search term.
            searchListRequest.MaxResults = 50;

            // Call the search.list method to retrieve results matching the specified query term.
            var searchListResponse = await searchListRequest.ExecuteAsync();

            List<Snippet> results = new List<Snippet>();

            // Add each result to the appropriate list, and then display the lists of
            // matching videos, channels, and playlists.
            foreach (var searchResult in searchListResponse.Items)
            {
                Snippet currentSnippet = new Snippet()
                {
                    id = searchResult.Id.VideoId,
                    title = searchResult.Snippet.Title,
                };

                switch (searchResult.Id.Kind)
                {
                    case "youtube#video":
                        currentSnippet.kind = Shared.Enums.YoutubeSnippetKind.video;
                        break;

                    case "youtube#channel":
                        currentSnippet.kind = Shared.Enums.YoutubeSnippetKind.channel;
                        break;

                    case "youtube#playlist":
                        currentSnippet.kind = Shared.Enums.YoutubeSnippetKind.playlist;
                        break;
                }
                results.Add(currentSnippet);
            }

            return results;
        }
    }
}
