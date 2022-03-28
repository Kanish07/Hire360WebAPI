using System;
using System.Collections.Generic;

namespace Hire360WebAPI.Models
{
    public partial class JobApplied
    {
        public Guid JobAppliedId { get; set; }
        public Guid JobId { get; set; }
        public Guid CandidateId { get; set; }
        public DateTime? AppliedOn { get; set; }
        public string? Active { get; set; }

        public virtual Candidate Candidate { get; set; } = null!;
        public virtual Job Job { get; set; } = null!;
    }
}
