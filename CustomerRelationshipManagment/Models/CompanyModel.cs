using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerRelationshipManagment.Models
{
    public class CompanyModel
    {
        public string Creator;
        public string Industry;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MaxLength(10)]
        [Column(TypeName = "varchar(10)")]
        public string Nip { get; set; }

        [Required]
        public int IndustryId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Address { get; set; }

        [Required]
        [MaxLength(100)]
        public string City { get; set; }

        [Required]
        public int CreatorId { get; set; }

        [Required]
        public DateTime WhenAdded { get; set; }

        [Required]
        public bool IsDeleted { get; set; }
    }
}
