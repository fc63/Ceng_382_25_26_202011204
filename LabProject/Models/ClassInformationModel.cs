using System.ComponentModel.DataAnnotations;

namespace ClassManApp.Models
{
    public class ClassInformationModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public int PersonCount { get; set; }

        public string Description { get; set; } = string.Empty;

        [Required]
        public bool IsActive { get; set; } = true;
    }
}
