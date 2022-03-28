using System;
using System.Collections.Generic;

namespace Hire360WebAPI.Models
{
    public partial class HumanResource
    {
        public HumanResource()
        {
            Jobs = new HashSet<Job>();
        }

        public Guid Hrid { get; set; }
        public string Hrname { get; set; } = null!;
        public string Hremail { get; set; } = null!;
        public string Hrpassword { get; set; } = null!;
        public string HrphoneNumber { get; set; } = null!;
        public string CompanyName { get; set; } = null!;
        public string City { get; set; } = null!;
        public string? Hrdescription { get; set; }
        public int UserRole { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? Active { get; set; }

        public virtual ICollection<Job> Jobs { get; set; }
    }
}
