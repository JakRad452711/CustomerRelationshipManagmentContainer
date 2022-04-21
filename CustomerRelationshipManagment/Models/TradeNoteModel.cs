using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerRelationshipManagment.Models
{
    public class TradeNoteModel
    {
        public string AssociatedCompany;
        public string Creator;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(800)]
        public string Contents { get; set; }

        [Required]
        public int AssociatedCompanyId { get; set; }

        [Required]
        public int CreatorId { get; set; }

        [Required]
        public bool IsDeleted { get; set; }
    }
}