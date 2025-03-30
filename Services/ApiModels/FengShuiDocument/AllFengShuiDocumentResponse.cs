using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.FengShuiDocument
{
    public class AllFengShuiDocumentResponse
    {
        public string FengShuiDocumentId { get; set; }

        public string Version { get; set; }

        public string Status { get; set; }

        public string DocNo { get; set; }

        public string DocumentName { get; set; }

        public string DocumentUrl { get; set; }
    }
}
