using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Globalization;
using QRCoder;

namespace ApiTesteRisePay.Classes
{
    // Resposta da RisePay
    public class PixResponse
    {
        [JsonProperty("qrCode")]
        public string qrCode { get; set; }

        [JsonProperty("value")]
        public decimal value { get; set; }
    }

    // Classe que vai desserializar o JSON todo
       
    public class RisePayApiResponse
    {
        [JsonProperty("object")]
        public PixObject Object { get; set; }
    }

    public class PixObject
    {
        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("pix")]
        public PixInfo Pix { get; set; }
    }

    public class PixInfo
    {
        [JsonProperty("qrCode")]
        public string QrCode { get; set; }
    }

    // DTO para retorno da API
    public class PixResponseDto
    {
        public string QrCode { get; set; }
        public decimal Value { get; set; }
        public string QrCodeImage { get; set; } // Base64 do QRCode
    }

    public class RisePayService
    {
        private readonly HttpClient _httpClient;
        private string Token = "8db531a94237fbff2abb718cb82eb4e4a954d097e78c465ec4cb28cd2199b1d0"; 

        public RisePayService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Token);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // Gera PIX com o novo formato de JSON
        public async Task<PixResponse> GerarPixAsync(decimal amount)
        {

            var payload = new
            {
                amount = amount,
                payment = new
                {
                    method = "pix"
                }
            };

            var json = JsonConvert.SerializeObject(payload, Formatting.Indented, new JsonSerializerSettings
            {
                Culture = CultureInfo.InvariantCulture
            });

            Console.WriteLine("==== JSON ENVIADO PARA RISEPAY ====");
            Console.WriteLine(json);
            Console.WriteLine("===================================");

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            Console.WriteLine("==== RESPOSTA RISEPAY ====");
            Console.WriteLine(responseContent);
            Console.WriteLine("==========================");

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Erro ao gerar PIX: {response.StatusCode} - {responseContent}");

            // Desserializa o JSON "aninhado" da RisePay

            var apiResponse = JsonConvert.DeserializeObject<RisePayApiResponse>(responseContent);

            return new PixResponse
            {
                qrCode = apiResponse.Object?.Pix?.QrCode,
                value = apiResponse.Object?.Amount ?? 0
            };
        }


        // Esse gera o QR Code em Base64
        public static class QrCodeHelper
        {
            public static string GerarQrCodeBase64(string qrCodeText)
            {
                if (string.IsNullOrEmpty(qrCodeText))
                    return null;

                using (var qrGenerator = new QRCodeGenerator())
                {
                    var qrCodeData = qrGenerator.CreateQrCode(qrCodeText, QRCodeGenerator.ECCLevel.Q);
                    var qrCode = new QRCode(qrCodeData);

                    using (var qrBitmap = qrCode.GetGraphic(20))
                    using (var ms = new MemoryStream())
                    {
                        qrBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        var base64 = Convert.ToBase64String(ms.ToArray());
                        return $"data:image/png;base64,{base64}";
                    }
                }
            }
        }
    }
}
