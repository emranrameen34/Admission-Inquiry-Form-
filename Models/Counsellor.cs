using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AdmissionInquiryRecord.Models
{
    public class Counsellor
    {
        [Key]
        public int CounsellorId { get; set; }

        [Required]
        [StringLength(100)]
        public string? CounsellorName { get; set; }

        [Required]
        [EmailAddress]
        public string? CounsellorEmail { get; set; }

        [Required]
        public string? CounsellorPassword { get; set; }

        public ICollection<Person>? Persons { get; set; }
    }
}