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
        static ProcessedImageViewModel[] cachedProcessedImageViewModelJson;
        static ProcessedImageViewModel[] cachedProcessedImageViewModelText;


        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        public async Task<IActionResult> TextViewer()
        {
            if (cachedProcessedImageViewModelText == null)
            {
                cachedProcessedImageViewModelText = (await GetProcessedImages(true)).ToArray();
            }

            ViewBag.ProcessedImages = cachedProcessedImageViewModelText;
            return View();
        }

        public async Task<IActionResult> JsonViewer()
        {
            if(cachedProcessedImageViewModelJson == null)
            {
                cachedProcessedImageViewModelJson = (await GetProcessedImages(false)).ToArray();
            }

            ViewBag.ProcessedImages = cachedProcessedImageViewModelJson;
            return View();
        }

        private async Task<ConcurrentBag<ProcessedImageViewModel>> GetProcessedImages(bool returnTextOnly)
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
                        OcrText = await cognitiveservices.MakeOcrRequest(filePath, returnTextOnly)
                    });
                }));
            };

            await Task.WhenAll(ProcessImageTasks);

            return processedImages;
        }
    }
}
