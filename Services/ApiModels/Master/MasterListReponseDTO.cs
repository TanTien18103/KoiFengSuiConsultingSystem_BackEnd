using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Master
{
    public class MasterListReponseDTO
    {
        public string MasterId { get; set; }

        public string MasterName { get; set; }

        public byte? Rating { get; set; }
    }
}
