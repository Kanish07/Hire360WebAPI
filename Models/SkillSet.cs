using System;
using System.Collections.Generic;

namespace Hire360WebAPI.Models
{
    public partial class SkillSet
    {
        public SkillSet()
        {
            Skills = new HashSet<Skill>();
        }

        public Guid SkillSetId { get; set; }
        public string SkillSetName { get; set; } = null!;
        public string? Active { get; set; }

        public virtual ICollection<Skill> Skills { get; set; }
    }
}
