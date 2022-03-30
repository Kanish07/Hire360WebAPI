using System;
using System.Collections.Generic;

namespace Hire360WebAPI.Models
{
    public partial class Qualification
    {
        public Guid QualificationId { get; set; }
        public Guid CandidateId { get; set; }
        public string DegreeName { get; set; } = null!;
        public int QualificationPercentage { get; set; }
        public string YearOfGraduation { get; set; } = null!;
        public string? Active { get; set; }

        public virtual Candidate? Candidate { get; set; } = null!;
    }
}
