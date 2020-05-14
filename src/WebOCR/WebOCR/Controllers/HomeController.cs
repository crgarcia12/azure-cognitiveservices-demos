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

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
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
                        FilePath = @$"images/{fileName}",
                        OcrText = await cognitiveservices.MakeOcrRequest_V3(filePath)
                    });
                }));
            };

            await Task.WhenAll(ProcessImageTasks);
            ViewBag.ProcessedImages = processedImages.ToArray();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
