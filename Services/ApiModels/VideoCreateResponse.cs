using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Services.ApiModels
{
    public class VideoCreateResponse
    {
        public string guid { get; set; }
        public int videoLibraryId { get; set; }
        public string title { get; set; }
    }
}
