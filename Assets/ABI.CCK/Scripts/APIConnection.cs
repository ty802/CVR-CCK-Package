using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Abi.Newtonsoft.Json;
using Abi.Newtonsoft.Json.Linq;
using UnityEngine;
using Object = System.Object;

public static class APIConnection
{
    public static string APIAddress = "https://api.abinteractive.net";
    public static string APIVersion = "2";
    private static string _apiUserAgent = "ChilloutVR API-Requests";
    
    private static HttpClient _client = new HttpClient();
    
    private static string _username;
    private static string _accessKey;

    private static bool _initialized = false;
    public static bool Initialized
    {
        get => _initialized;
    }
    
    public static void Initialize(string username, string accessKey)
    {
        if (username == "" || accessKey == "") return;
        
        _username = username;
        _accessKey = accessKey;
            
        _client.DefaultRequestHeaders.Clear();
        
        _client.DefaultRequestHeaders.Add("Username", _username); 
        _client.DefaultRequestHeaders.Add("AccessKey", _accessKey);
        _client.DefaultRequestHeaders.Add("User-Agent", _apiUserAgent);

        _client.Timeout = TimeSpan.FromSeconds(30);
        _initialized = true;
    }
    
    public static async Task<BaseResponse<T>> MakeRequest<T>(string url, Object data = null, string apiVersion = null, bool put = false)
    {
        if (!_initialized) return default(BaseResponse<T>);
        
        //JObject currentRequestData;
        string currentRequestUrl = String.Empty;
        int currentRequestType = 0;

        if (apiVersion == null) apiVersion = APIVersion;
        
        currentRequestUrl = $"{APIAddress}/{apiVersion}/{url}";

        HttpResponseMessage response;
        if (data != null) currentRequestType = 1;
        if (put) currentRequestType = 2;
        
        if (currentRequestType == 0)
        {
            response = await _client.GetAsync(currentRequestUrl);
        }
        else if (currentRequestType == 2)
        {
            response = await _client.PutAsync(currentRequestUrl, null);
        }
        else
        {
            response = await _client.PostAsync(currentRequestUrl, new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json"));
        }
        

        if (response.IsSuccessStatusCode)
        {
            return JsonConvert.DeserializeObject<BaseResponse<T>>(await response.Content.ReadAsStringAsync());
        }
        else
        {
            Debug.LogError($"Result from API Request {currentRequestUrl} returned {response.StatusCode}");
        }

        try
        {
            BaseResponse<T> res = JsonConvert.DeserializeObject<BaseResponse<T>>(await response.Content.ReadAsStringAsync());
            if (res != null)
                return res;
        }
        catch
        {
            Debug.LogError($"Result from API Request {currentRequestUrl} could not be parsed");
        }

        return default(BaseResponse<T>);
    }
    
    //Responses
    public class BaseResponse<T>
    {
        public string Message { get; set; }
        public T Data { get; set; }

        public BaseResponse(string message = null, T data = default)
        {
            Message = message;
            Data = data;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
    
    public class UserinfoResponse
    {
        public bool IsAccountUnlocked { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string UserRank { get; set; }
    }
    
    public enum ContentTypes
    {
        Avatar,
        World,
        Spawnable
    }
    
    public class GenerateResponse
    {
        public Guid Id { get; set; }
        public ContentTypes Type { get; set; }
    }
    
    public class ContentInfoResponse
    {
        public string UploadLocation { get; set; }
        public ContentDataIni ContentData { get; set; }
    
        public class ContentDataIni : GenericINI
        {
            public ContentTypes Type { get; set; }
            public UgcTagsData Tags { get; set; }    
        }
    }
    
    public class GenericINI
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Uri Image { get; set; }
    }
    
    public struct UgcTagsData
    {
        public bool Gore;
        public bool Horror;
        public bool Jumpscare;
        public bool Nudity;
        public bool Suggestive;
        public bool Violence;
        public bool ContainsMusic;
        public bool ExtremelyBright;
        public bool ExtremelyHuge;
        public bool ExtremelySmall;
        public bool FlashingColors;
        public bool FlashingLights;
        public bool LoudAudio;
        public bool ParticleSystems;
        public bool ScreenEffects;
        public bool SpawnAudio;
        public bool LongRangeAudio;
    }
}