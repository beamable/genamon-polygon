using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public static class OpenAI
{
    private const string URL = "https://api.openai.com/v1/completions";
    private const string KEY = "<DA-VINCI-MODEL-API_KEY>";

    private static readonly HttpClient _httpClient = new HttpClient();

    public static async Task<CompletionResponse> Send(string prompt)
    {
        var model = new CompletionRequest
        {
            prompt = prompt
        };
        var json = JsonConvert.SerializeObject(model);

        var req = new HttpRequestMessage(HttpMethod.Post, URL);
        req.Headers.Add("Authorization", $"Bearer {KEY}");
        req.Content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(req, HttpCompletionOption.ResponseHeadersRead);
        var responseJson = await response.Content.ReadAsStringAsync();
        var completionResponse = JsonConvert.DeserializeObject<CompletionResponse>(responseJson);

        Debug.Log(responseJson);

        return completionResponse;
    }

    public class CompletionResponse
    {
        public string id;
        [JsonProperty("object")]
        public string objectType;

        public string model;
        public long created;

        public CompletionResponseChoice[] choices;
        public CompletionResponseUsage usage;
    }

    public class CompletionResponseChoice
    {
        public string text;
        private int index;
        public string finish_reason;
    }


    public class CompletionResponseUsage
    {
        public int total_tokens;
        private int prompt_tokens;
        public int completion_tokens;
    }
    class CompletionRequest
    {
        public string model = "text-davinci-003";
        public string prompt = "";
        public int max_tokens = 256;
        public double frequency_penalty = 1.3;
        public double presence_penalty = 0.7;
        public double temperature = 0.9;
        public int top_p = 1;
        public bool stream = false;
        public int best_of = 4;
        public int n = 4;
        public string logprobs = null;
        public string[] stop = new[] { "AI:", "Human:" };
    }
}