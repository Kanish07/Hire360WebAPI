using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hire360WebAPI.Models
{
    public class WelcomeRequest
    {
        public string? ToEmail { get; set; }
        public string? UserName { get; set; }
    }
}