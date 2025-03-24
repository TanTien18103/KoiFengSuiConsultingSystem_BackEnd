using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.KoiPond
{
    public class KoiPondRequest
    {
        public string ShapeId { get; set; }

        public string PondName { get; set; }

        public string Direction { get; set; }
    }
}
