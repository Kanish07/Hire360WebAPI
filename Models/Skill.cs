using System;
using System.Collections.Generic;

namespace Hire360WebAPI.Models
{
    public partial class Skill
    {
        public Guid SkillId { get; set; }
        public Guid CandidateId { get; set; }
        public Guid SkillSetId { get; set; }
        public string SkillLevel { get; set; } = null!;
        public string? Active { get; set; }

        public virtual Candidate? Candidate { get; set; } = null!;
        public virtual SkillSet? SkillSet { get; set; } = null!;
    }
}
