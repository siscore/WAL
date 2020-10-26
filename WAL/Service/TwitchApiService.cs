using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WAL.Models;

namespace WAL.Service
{
    public class TwitchApiService
    {
        private readonly Uri _baseAddress = new Uri("https://addons-ecs.forgesvc.net");
        

        public async Task<List<SearchResponceModel>> GetAddOnsList(SearchRequestModel model)
        {
            using (var httpClient = new HttpClient { BaseAddress = _baseAddress })
            {
                {
                    using (var response = await httpClient.GetAsync(model.ToString()))
                    {
                        string responseHeaders = response.Headers.ToString();
                        string responseData = await response.Content.ReadAsStringAsync();

                        if (response.StatusCode==System.Net.HttpStatusCode.OK)
                        {
                            var result = JsonConvert.DeserializeObject<List<SearchResponceModel>>(responseData);
                            return result;
                        }

                        return null;
                    }
                }
            }
        }

        public async Task<GameModel> GetGame(int gameId)
        {
            using (var httpClient = new HttpClient { BaseAddress = _baseAddress })
            {
                {
                    using (var response = await httpClient.GetAsync(string.Format("api/v2/game/{0}", gameId)))
                    {
                        string responseHeaders = response.Headers.ToString();
                        string responseData = await response.Content.ReadAsStringAsync();

                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            var result = JsonConvert.DeserializeObject<GameModel>(responseData);
                            return result;
                        }

                        return null;
                    }
                }
            }
        }

        public async Task<FingerprintMatchResultModel> GetAddonsByFingerprint(List<string> fingerprints)
        {
            using (var httpClient = new HttpClient { BaseAddress = _baseAddress })
            {
                using (var content = new StringContent($@"[{ string.Join(",", fingerprints) }]", System.Text.Encoding.Default, "application/json"))
                {
                    content.Headers.ContentType.CharSet = string.Empty;
                    using (var response = await httpClient.PostAsync("/api/v2/fingerprint", content))
                    {
                        string responseHeaders = response.Headers.ToString();
                        string responseData = await response.Content.ReadAsStringAsync();

                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            var result = JsonConvert.DeserializeObject<FingerprintMatchResultModel>(responseData);
                            return result;
                        }

                        return null;
                    }
                }
            }
        }

        public async Task<SearchResponceModel> GetAddonInfo(int addonId)
        {
            using (var httpClient = new HttpClient { BaseAddress = _baseAddress })
            {
                using (var response = await httpClient.GetAsync(string.Format("/api/v2/addon/{0}", addonId)))
                {
                    string responseHeaders = response.Headers.ToString();
                    string responseData = await response.Content.ReadAsStringAsync();

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var result = JsonConvert.DeserializeObject<SearchResponceModel>(responseData);
                        return result;
                    }

                    return null;
                }
            }
        }

        public async Task<bool> DownloadAddon(Uri downloadUrl, string fileName)
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(downloadUrl))
                {
                    if (response != null && response.StatusCode == HttpStatusCode.OK)
                    {
                        using (var stream = await response.Content.ReadAsStreamAsync())
                        {
                            using (var memStream = new MemoryStream())
                            {
                                await stream.CopyToAsync(memStream);
                                memStream.Position = 0;
                                using (FileStream file = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write))
                                {
                                    memStream.WriteTo(file);
                                    return true;
                                }
                            }
                        }
                    }

                    return false;
                }
            }
        }

        public async Task<List<SearchResponceModel>> GetAddonsInfo(List<string>  addonIds)
        {
            using (var httpClient = new HttpClient { BaseAddress = _baseAddress })
            {
                using (var content = new StringContent($@"[{ string.Join(",", addonIds) }]", System.Text.Encoding.Default, "application/json"))
                {
                    content.Headers.ContentType.CharSet = string.Empty;
                    using (var response = await httpClient.PostAsync("/api/v2/addon", content))
                    {
                        string responseHeaders = response.Headers.ToString();
                        string responseData = await response.Content.ReadAsStringAsync();

                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            var result = JsonConvert.DeserializeObject<List<SearchResponceModel>>(responseData);
                            return result;
                        }

                        return null;
                    }
                }
            }
        }

        public async Task<Bitmap> GetCategoryBitmap(Uri uri)
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(uri))
                {
                    if (response != null && response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        using (var stream = await response.Content.ReadAsStreamAsync())
                        {
                            using (var memStream = new MemoryStream())
                            {
                                await stream.CopyToAsync(memStream);
                                memStream.Position = 0;
                                return new Bitmap(memStream);
                            }
                        }
                    }

                    return null;
                }
            }
        }
    }
}
