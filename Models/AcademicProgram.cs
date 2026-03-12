using System.ComponentModel.DataAnnotations;

namespace AdmissionInquiryRecord.Models
{
    public class AcademicProgram
    {

        [Key]
        public int ProgramId { get; set; }
        public string? ProgramName { get; set; }
    }
}