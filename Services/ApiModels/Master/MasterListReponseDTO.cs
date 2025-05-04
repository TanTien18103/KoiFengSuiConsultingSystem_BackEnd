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

        public string Title { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string ImageUrl { get; set; }

        public string Expertise { get; set; }

        public string Experience { get; set; }

        public string Biography { get; set; }
    }
}
