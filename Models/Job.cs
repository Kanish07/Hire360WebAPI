using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Hire360WebAPI.Models
{
    public partial class Job
    {
        public Job()
        {
            JobApplieds = new HashSet<JobApplied>();
        }

        public Guid JobId { get; set; }
        public Guid Hrid { get; set; }
        public string JobTitle { get; set; } = null!;
        public string? JobDescription { get; set; }
        public string JobCity { get; set; } = null!;
        public int NoOfVacancy { get; set; }
        public int Package { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? Active { get; set; }

        public virtual HumanResource Hr { get; set; } = null!;
        [JsonIgnore]
        public virtual ICollection<JobApplied> JobApplieds { get; set; }
    }
}
