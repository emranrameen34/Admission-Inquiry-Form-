using System;
using System.ComponentModel.DataAnnotations;

namespace AdmissionInquiryRecord.Models
{
    public class CounsellingRecord
    {
        [Key]
        public int CounsellingId { get; set; }

        public int PersonId { get; set; }
        public Person? Person { get; set; }

        public int CounsellorId { get; set; }
        public Counsellor? Counsellor { get; set; }

        [Display(Name = "Visit Date")]
        public DateTime VisitDate { get; set; } = DateTime.Now;

        [Display(Name = "Information / Package Given")]
        public string? InfoGiven { get; set; }

        [Display(Name = "Scholarship / Fee Offer")]
        public string? FeeOffer { get; set; }

        [Display(Name = "Prospectus Purchased")]
        public bool ProspectusPurchased { get; set; } // yes/no

        [Display(Name = "Expectations")]
        public string? Expectations { get; set; }

        [Display(Name = "Special Comments")]
        public string? Comments { get; set; }

        [Display(Name = "Followup Required")]
        public bool FollowupRequired { get; set; }

        [Display(Name = "Followup Date")]
        public DateTime? FollowupDate { get; set; } // new column

        [Display(Name = "Contact Method")]
        public string? ContactMethod { get; set; } // Call, WhatsApp, Email

        [Display(Name = "Status")]
        public string? Status { get; set; } // Converted, Considering, Not Interested, No Response
    }
}