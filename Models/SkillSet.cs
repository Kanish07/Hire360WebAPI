using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

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

        [JsonIgnore]
        public virtual ICollection<Skill> Skills { get; set; }
    }
}
