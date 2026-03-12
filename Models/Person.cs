using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
namespace AdmissionInquiryRecord.Models
{
    [Index(nameof(CNIC), IsUnique = true)]
    public class Person
    {
        public int PersonId { get; set; }

        [Required]
        public int VisitNo { get; set; }

        public DateTime EntryDate { get; set; } = DateTime.Now;

        [Required]
        public string? Name { get; set; }

        public string? CNIC { get; set; }
        public string? CellNo { get; set; }
        public string? WhatsAppNo { get; set; }

        public int? CityId { get; set; }
        public int? DistrictId { get; set; }
        public int? TehsilId { get; set; }

        public string? ReferredBy { get; set; }
        public bool NotReferred { get; set; }

        public int? ProgramInterestedId { get; set; }
        public int? AlternativeProgramId { get; set; }

        public int? SessionId { get; set; }

        public string? HowDidCandidate { get; set; }
        public int? CounsellorId { get; set; }      // nullable
        public Counsellor? Counsellor { get; set; } // navigation property (optional)

      
    }
}