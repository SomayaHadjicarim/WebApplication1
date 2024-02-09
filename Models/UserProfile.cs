using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class UserProfile
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int SharedUsers { get; set; }
        public string? RateLimit { get; set; }
        public string? SessionTimeout { get; set; }
    }
}


