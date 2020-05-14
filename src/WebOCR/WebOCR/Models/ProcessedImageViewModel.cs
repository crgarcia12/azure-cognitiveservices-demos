using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebOCR.Models
{
    public class ProcessedImageViewModel
    {
        public string FilePath { get; set; }
        public string OcrText { get; set; }
    }
}
