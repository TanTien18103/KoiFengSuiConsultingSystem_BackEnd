using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Models
{
    public class BunnyCdnSettings
    {
        public string StorageZoneName { get; set; }
        public string ApiKey { get; set; }
        public string CdnUrl { get; set; } = "https://storage.bunnycdn.com"; 
    }
}
