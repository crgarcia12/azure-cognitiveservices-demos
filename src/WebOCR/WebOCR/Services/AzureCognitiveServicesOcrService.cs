namespace WebOCR.Services
{
    using Newtonsoft.Json.Linq;
    using System;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;

    public class AzureCognitiveServicesOcrService
    {
        private readonly string SubscriptionKey;
        private readonly string CognitiveServicesApiUrl;
        private readonly string VisionV2Url;
        private readonly string VisionV3Url;

        public AzureCognitiveServicesOcrService()
        {
            SubscriptionKey = "KEY";
            CognitiveServicesApiUrl = "https://westeurope.api.cognitive.microsoft.com/";
            VisionV2Url = "vision/v2.1/ocr/";
            VisionV3Url = "vision/v3.0-preview/read/";
        }

        public async Task<string> MakeOcrRequest(string imageFilePath, bool returnTextOnly)
        {
            try
            {
                //return await MakeOcrRequest_V2(imageFilePath);
                string text = await MakeOcrRequest_V3(imageFilePath);
                if (returnTextOnly)
                {
                    text = ExtractTextFromResult(text);
                }
                return text;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        private async Task<string> MakeOcrRequest_V2(string imageFilePath)
        {
            try
            {
                HttpClient client = new HttpClient();

                // Request headers.
                client.DefaultRequestHeaders.Add(
                    "Ocp-Apim-Subscription-Key", SubscriptionKey);

                // Request parameters. 
                // The language parameter doesn't specify a language, so the 
                // method detects it automatically.
                // The detectOrientation parameter is set to true, so the method detects and
                // and corrects text orientation before detecting text.
                string requestParameters = "language=unk&detectOrientation=true";

                // Assemble the URI for the REST API method.
                string uri = CognitiveServicesApiUrl + VisionV2Url + "?" + requestParameters;

                HttpResponseMessage response;

                // Read the contents of the specified local image
                // into a byte array.
                byte[] byteData = GetImageAsByteArray(imageFilePath);

                // Add the byte array as an octet stream to the request body.
                using (ByteArrayContent content = new ByteArrayContent(byteData))
                {
                    // This example uses the "application/octet-stream" content type.
                    // The other content types you can use are "application/json"
                    // and "multipart/form-data".
                    content.Headers.ContentType =
                        new MediaTypeHeaderValue("application/octet-stream");

                    // Asynchronously call the REST API method.
                    response = await client.PostAsync(uri, content);
                }

                // Asynchronously get the JSON response.
                string contentString = await response.Content.ReadAsStringAsync();
                return JToken.Parse(contentString).ToString();
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        /// <summary>
        /// Gets the text visible in the specified image file by using
        /// the Computer Vision REST API.
        /// </summary>
        /// <param name="imageFilePath">The image file with printed text.</param>
        private async Task<string> MakeOcrRequest_V3(string imageFilePath)
        {
            try
            {
                HttpClient client = new HttpClient();

                // Request headers.
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", SubscriptionKey);

                // Assemble the URI for the REST API method.
                string uri = CognitiveServicesApiUrl + VisionV3Url + "analyze/";

                HttpResponseMessage response;

                // Read the contents of the specified local image
                // into a byte array.
                byte[] byteData = GetImageAsByteArray(imageFilePath);

                // Add the byte array as an octet stream to the request body.
                using (ByteArrayContent content = new ByteArrayContent(byteData))
                {
                    // This example uses the "application/octet-stream" content type.
                    // The other content types you can use are "application/json"
                    // and "multipart/form-data".
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                    // Asynchronously call the REST API method.
                    response = await client.PostAsync(uri, content);
                }

                string operationid = response.Headers.GetValues("Operation-Location").First();

                return await CheckOperationResponseAsync(client, operationid);

            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        private async Task<string> CheckOperationResponseAsync(HttpClient client, string operationId)
        {
            HttpResponseMessage response;
            while (true)
            {
                await Task.Delay(1000);
                response = await client.GetAsync(operationId);

                // Asynchronously get the JSON response.
                string contentString = await response.Content.ReadAsStringAsync();
                var answer = JToken.Parse(contentString);
                if (answer["status"].ToString() != "running")
                {
                    return answer.ToString();
                
                }
            }
        }

        private string ExtractTextFromResult(string text)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach(string line in text.Split(new[] { '\r', '\n' }))
            {
                string trimmmedLine = line.Trim();
                if (trimmmedLine.StartsWith("\"text\"") || trimmmedLine.StartsWith("\"status\""))
                {
                    stringBuilder.AppendLine(trimmmedLine);
                }
            }
            return stringBuilder.ToString();
        }


        /// <summary>
        /// Returns the contents of the specified file as a byte array.
        /// </summary>
        /// <param name="imageFilePath">The image file to read.</param>
        /// <returns>The byte array of the image data.</returns>
        private byte[] GetImageAsByteArray(string imageFilePath)
        {
            // Open a read-only file stream for the specified file.
            using (FileStream fileStream =
                new FileStream(imageFilePath, FileMode.Open, FileAccess.Read))
            {
                // Read the file's contents into a byte array.
                BinaryReader binaryReader = new BinaryReader(fileStream);
                return binaryReader.ReadBytes((int)fileStream.Length);
            }
        }
    }
}
