using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.FengShuiDocument
{
    public class FengShuiDocumentResponse
    {
        public string FengShuiDocumentId { get; set; }

        public string Version { get; set; }

        public string DocNo { get; set; }

        public string DocumentName { get; set; }
    }
}
