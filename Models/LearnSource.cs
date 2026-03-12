using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AdmissionInquiryRecord.Models
{
    public class LearnSource
    {
        [Key]
        public int SourceId { get; set; }
        public string? SourceName { get; set; }

        
    }
}