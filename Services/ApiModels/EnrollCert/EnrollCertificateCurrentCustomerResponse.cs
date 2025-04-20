using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.EnrollCert
{
    public class EnrollCertificateCurrentCustomerResponse
    {
        public string EnrollCertId { get; set; }
        public string CertificateId { get; set; }
        public string CustomerId { get; set; }
        public string RegisterCourseId { get; set; }
        public string CourseId { get; set; }
        public DateTime? CreateDate { get; set; }
        public decimal Point { get; set; }
        public string CourseName { get; set; }
        public string Description { get; set; }
        public string Introduction { get; set; }
        public string CourseImageUrl { get; set; }
        public string MasterName { get; set; }
        public string CertificateImageUrl { get; set; }
    }
}
