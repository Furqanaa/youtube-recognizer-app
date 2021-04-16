using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Models;
using YoutubeUtils;
using System.IO;
using Presentaion.Helpers;

namespace Presentaion.Controllers
{
    public class HomeController : Controller
    {
        ILog log = log4net.LogManager.GetLogger(typeof(HomeController));
        private readonly IViewRenderService _viewRenderService;

        public HomeController(IViewRenderService viewRenderService)
        {
            _viewRenderService = viewRenderService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ParseUrl([FromBody]SearchForm searchForm)
        {
                Console.WriteLine(searchForm.youtubeUrl);
            try
            {
                YoutubeProxy YLogic = new YoutubeProxy();
                Video video = await YLogic.GetVideoInfoByUrl(searchForm.youtubeUrl);
                var result =  await _viewRenderService.RenderToStringAsync("_VideoResult", video);
                Console.WriteLine(video.Title);
                return Json(new
                {
                    status = true,
                    videoViewModel = video,
                    partialViewData = Content(result)
                });
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return Json(new { status = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult> DownloadAudioFile([FromBody]SearchForm searchForm)
        {
            try
            {
                YoutubeProxy YLogic = new YoutubeProxy();
                AudioFile audioFile = await YLogic.DownloadAudioByUrl(searchForm.youtubeUrl);
                audioFile.fileWebsitePath = Request.Scheme + "://" + Request.Host + Request.PathBase + "/Downloads/Audio/" + audioFile.fileName;
                var result = await _viewRenderService.RenderToStringAsync("_AudioResult", audioFile);
                return Json(new
                {
                    status = true,
                    audioFileViewModel = audioFile,
                    partialViewData = Content(result)
                });
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return Json(new { status = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult> SplitAudioFile([FromBody]AudioFile audioFile)
        {
            try
            {
                YoutubeProxy YLogic = new YoutubeProxy();
                AudioFile splitAudioFile = YLogic.SplitAudioFile(audioFile);
                splitAudioFile.fileWebsitePath = Request.Scheme + "://" + Request.Host + Request.PathBase + "/Downloads/Audio/" + splitAudioFile.fileName;
                var result = await _viewRenderService.RenderToStringAsync("_AudioResult", splitAudioFile);
                return Json(new
                {
                    status = true,
                    audioFileViewModel = splitAudioFile,
                    partialViewData = Content(result)
                });
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return Json(new { status = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<JsonResult> RecognizeAudioFile([FromBody]AudioFile audioFile)
        {
            try
            {
                bool operationStatus = false;
                YoutubeProxy YLogic = new YoutubeProxy();
                Artist artistViewModel = await YLogic.RecognizeAudioFile(audioFile);
                var result = "";
                if (artistViewModel == null || string.IsNullOrWhiteSpace(artistViewModel.artist))
                    result = "Sorry, We couldn't recognize the artist!!";
                else
                {
                    result = await _viewRenderService.RenderToStringAsync("_ArtistResult", artistViewModel);
                    operationStatus = true;
                }
                return Json(new
                {
                    status = operationStatus,
                    artistViewModel = artistViewModel,
                    partialViewData = Content(result)
                });
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return Json(new { status = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult> SerachYoutube([FromBody]Artist artistViewModel)
        {
            try
            {
                YoutubeProxy YLogic = new YoutubeProxy();
                List<Snippet> snippetViewModelList = await YLogic.SearchYoutubeSnippets(artistViewModel.artist);
                var result = await _viewRenderService.RenderToStringAsync("_SearchResult", snippetViewModelList);
                return Json(new
                {
                    status = true,
                    snippetViewModelList = snippetViewModelList,
                    partialViewData = Content(result)
                });
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return Json(new { status = false, message = ex.Message });
            }
        }


        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }


    }
}
