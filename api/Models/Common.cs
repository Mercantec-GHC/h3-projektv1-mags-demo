using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public abstract class Common
    {
        [Key]
        public string Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
