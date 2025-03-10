using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Services.ApiModels.Payment
{
    public class PaymentLinkResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("orderCode")]
        public string OrderCode { get; set; }

        [JsonPropertyName("amount")]
        public int Amount { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("checkoutUrl")]
        public string CheckoutUrl { get; set; }

        [JsonPropertyName("qrCode")]
        public string QrCode { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("expiredAt")]
        public DateTime ExpiredAt { get; set; }

        [JsonPropertyName("cancellable")]
        public bool Cancellable { get; set; }
    }
}
