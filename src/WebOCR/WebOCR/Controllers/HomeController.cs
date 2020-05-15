using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebOCR.Models;
using WebOCR.Services;

namespace WebOCR.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        static Dictionary<string, ProcessedImageViewModel[]> cachedProcessedImageViewModel = new Dictionary<string, ProcessedImageViewModel[]>();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        public async Task<IActionResult> GetResults(string version, string format)
        {

            string cacheKey = version + format;
            bool isV2 = version == "2";
            bool textOnly = format == "Text";

            cachedProcessedImageViewModel[cacheKey] = (await GetProcessedImages(textOnly, isV2)).ToArray();
            if (!cachedProcessedImageViewModel.ContainsKey(cacheKey))
            {
                cachedProcessedImageViewModel[cacheKey] = (await GetProcessedImages(textOnly, isV2)).ToArray();
            }
 

            ViewBag.ProcessedImages = cachedProcessedImageViewModel[cacheKey];
            return View();
        }

        private async Task<ConcurrentBag<ProcessedImageViewModel>> GetProcessedImages(bool returnTextOnly, bool use_Api_V2)
        {
            var cognitiveservices = new AzureCognitiveServicesOcrService();

            string[] imagesFilesPaths = Directory.GetFiles(@"wwwroot\images\");

            var processedImages = new ConcurrentBag<ProcessedImageViewModel>();
            List<Task> ProcessImageTasks = new List<Task>(imagesFilesPaths.Length);


            foreach (string filePath in imagesFilesPaths)
            {
                ProcessImageTasks.Add(Task.Run(async () =>
                {
                    string fileName = Path.GetFileName(filePath);
                    processedImages.Add(new ProcessedImageViewModel()
                    {
                        FilePath = @$"/images/{fileName}",
                        OcrText = await cognitiveservices.MakeOcrRequest(filePath, returnTextOnly, use_Api_V2)
                    });
                }));
            };

            await Task.WhenAll(ProcessImageTasks);

            return processedImages;
        }
    }
}
