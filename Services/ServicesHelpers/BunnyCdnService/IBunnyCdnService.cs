using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ServicesHelpers.BunnyCdnService
{
    public interface IBunnyCdnService
    {
        Task<string> UploadVideoAsync(IFormFile file);
    }
}
