using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        const string apiUrl = "https://dog.ceo/api/breeds/image/random";
        const int numberOfRequests = 10;
        const string saveDirectory = "D:\\study\\pp\\pplw71\\";

        await RunSynchronousRequests(apiUrl, numberOfRequests, saveDirectory);
        await RunAsynchronousRequests(apiUrl, numberOfRequests, saveDirectory);
    }

    static async Task RunSynchronousRequests(string apiUrl, int numberOfRequests, string saveDirectory)
    {
        Console.WriteLine("Starting synchronous requests...");
        Stopwatch syncStopwatch = Stopwatch.StartNew();
        await MakeSynchronousRequests(apiUrl, numberOfRequests, saveDirectory);
        syncStopwatch.Stop();
        Console.WriteLine($"Synchronous requests completed in {syncStopwatch.ElapsedMilliseconds} ms");
    }

    static async Task RunAsynchronousRequests(string apiUrl, int numberOfRequests, string saveDirectory)
    {
        Console.WriteLine("\nStarting asynchronous requests...");
        Stopwatch asyncStopwatch = Stopwatch.StartNew();
        await MakeAsynchronousRequests(apiUrl, numberOfRequests, saveDirectory);
        asyncStopwatch.Stop();
        Console.WriteLine($"Asynchronous requests completed in {asyncStopwatch.ElapsedMilliseconds} ms");
    }

    static async Task MakeSynchronousRequests(string apiUrl, int numberOfRequests, string saveDirectory)
    {
        using (HttpClient client = new HttpClient())
        {
            for (int i = 0; i < numberOfRequests; i++)
            {
                await MakeSingleRequestAsync(client, apiUrl, i + 1, "Synchronous", saveDirectory);
            }
        }
    }

    static async Task MakeAsynchronousRequests(string apiUrl, int numberOfRequests, string saveDirectory)
    {
        using (HttpClient client = new HttpClient())
        {
            List<Task> requestTasks = new List<Task>();

            for (int i = 0; i < numberOfRequests; i++)
            {
                Task requestTask = MakeSingleRequestAsync(client, apiUrl, i + 1, "Asynchronous", saveDirectory);
                requestTasks.Add(requestTask);
            }

            await Task.WhenAll(requestTasks);
        }
    }

    static async Task MakeSingleRequestAsync(HttpClient client, string apiUrl, int requestNumber, string requestType, string saveDirectory)
    {
        try
        {
            string jsonResponse = await client.GetStringAsync(apiUrl);
            var responseObject = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiResponse>(jsonResponse);

            if (responseObject != null && responseObject.Status == "success")
            {
                string imageUrl = responseObject.Message.Replace("\\/", "/");
                string fileName = $"{requestType}_dog_{requestNumber}.jpg";
                string filePath = Path.Combine(saveDirectory, fileName);

                using (var responseStream = await client.GetStreamAsync(imageUrl))
                using (var fileStream = File.Create(filePath))
                {
                    await responseStream.CopyToAsync(fileStream);
                }

                Console.WriteLine($"{requestType} request {requestNumber}: Image saved to {filePath}");
            }
            else
            {
                Console.WriteLine($"{requestType} request {requestNumber}: Invalid response format");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{requestType} request {requestNumber}: Exception - {ex.Message}");
        }
    }

    public class ApiResponse
    {
        public string Message { get; set; }
        public string Status { get; set; }
    }
}