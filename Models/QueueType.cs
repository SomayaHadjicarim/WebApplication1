using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class QueueType
    {
        [NotMapped]
        public string? Id { get; set; }
        [NotMapped]
        public string? Name { get; set; }
        [NotMapped]
        public string? Kind { get; set; }
        [NotMapped]
        public string? Rate { get; set; }
        [NotMapped]
        public string? QueueSize { get; set; }
        [NotMapped]
        public string? TotalQueueSize { get; set; }
    }
}
