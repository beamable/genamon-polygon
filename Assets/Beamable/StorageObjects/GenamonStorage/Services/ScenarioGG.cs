using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public static class ScenarioGG
{
    private const string URL = "https://api.cloud.scenario.gg/v1/models/<SCENARIO-GG-MODEL-ID>/inferences";
    private const string KEY = "<SCENARIO-GG-API-KEY>";

    private static readonly HttpClient _httpClient = new HttpClient();

    public static async Task<InferenceResponse> CreateInference(string prompt)
    {
        var requestBody = new CreateInferenceRequest
        {
            parameters = new InferenceParameters
            {
                prompt = prompt
            }
        };
        var json = JsonConvert.SerializeObject(requestBody);

        var req = new HttpRequestMessage(HttpMethod.Post, URL);
        req.Headers.Add("Authorization", $"Basic {KEY}");
        req.Content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(req, HttpCompletionOption.ResponseHeadersRead);
        var responseJson = await response.Content.ReadAsStringAsync();
        var completionResponse = JsonConvert.DeserializeObject<InferenceResponse>(responseJson);

        Debug.Log(responseJson);

        return completionResponse;
    }

    public static async Task<InferenceResponse> GetInference(string inferenceId)
    {
        var req = new HttpRequestMessage(HttpMethod.Get, $"{URL}/{inferenceId}");
        req.Headers.Add("Authorization", $"Basic {KEY}");

        var response = await _httpClient.SendAsync(req, HttpCompletionOption.ResponseHeadersRead);
        var responseJson = await response.Content.ReadAsStringAsync();
        var completionResponse = JsonConvert.DeserializeObject<InferenceResponse>(responseJson);
        return completionResponse;
    }

    public class CreateInferenceRequest
    {
        public InferenceParameters parameters;
    }

    public class InferenceParameters
    {
        public string prompt;
        public string type = "txt2img";
        public double guidance = 7.0;
        public int width = 512;
        public int height = 512;
        public int numInferenceSteps = 50;
        public int numSamples = 1;
        public bool enableSafetyCheck = false;
    }

    public class InferenceResponse
    {
        public Inference inference;
    }

    public class Inference
    {
        public string id;
        public string userId;
        public string authorId;
        public string modelId;
        public string createdAt;
        public InferenceParameters parameters;
        public string status;
        public InferenceImage[] images;
        public int imagesNumber;
        public string displayPrompt;
        public double progress;

        public bool IsCompleted
        {
            get { return status == "succeeded";  }
        }

        public bool InProgress
        {
            get { return status == "in-progress"; }
        }
    }

    public class InferenceImage
    {
        public string id;
        public string url;
    }
}
