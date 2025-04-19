using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Certificate
{
    public class CertificateResponse
    {
        public string CertificateId { get; set; }
        public string CourseId { get; set; }
        public string CourseName { get; set; }
        public DateOnly? IssueDate { get; set; }
        public string Description { get; set; }
        public string CertificateImage { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
