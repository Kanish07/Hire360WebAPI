﻿using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Hire360WebAPI.Entities;

namespace Hire360WebAPI.Models
{
    public partial class Candidate
    {
        public Candidate()
        {
            JobApplieds = new HashSet<JobApplied>();
            Qualifications = new HashSet<Qualification>();
            Skills = new HashSet<Skill>();
        }

        public Guid CandidateId { get; set; }
        public string CandidateName { get; set; } = null!;
        public string CandidateEmail { get; set; } = null!;
        public string CandidatePassword { get; set; } = null!;
        public string CandidatePhoneNumber { get; set; } = null!;
        public string CandidateCity { get; set; } = null!;
        public string CandidateTotalExp { get; set; } = null!;
        public string? CandidateResume { get; set; }
        public string? CandidatePhotoUrl { get; set; }
        public string? CandidateDescription { get; set; }
        public Role UserRole { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? Active { get; set; }

        [JsonIgnore]
        public virtual ICollection<JobApplied> JobApplieds { get; set; }
        [JsonIgnore]
        public virtual ICollection<Qualification> Qualifications { get; set; }
        [JsonIgnore]
        public virtual ICollection<Skill> Skills { get; set; }
    }
}
