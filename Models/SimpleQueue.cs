using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class SimpleQueue
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Target { get; set; }
        public string? UploadMaxLimit { get; set; }
        public string? DownloadMaxLimit { get; set; }
    }
}
