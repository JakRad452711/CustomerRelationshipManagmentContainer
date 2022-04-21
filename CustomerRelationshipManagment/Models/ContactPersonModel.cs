using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerRelationshipManagment.Models
{
    public class ContactPersonModel
    {
        public string AssociatedCompany;
        public string Inviter;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MaxLength(100)]
        public string Surname { get; set; }

        [Required]
        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        [Required]
        [MaxLength(100)]
        public string EmailAddress { get; set; }

        [Required]
        [MaxLength(50)]
        public string Position { get; set; }

        [Required]
        public int AssociatedCompanyId { get; set; }

        [Required]
        public int InviterId { get; set; }

        [Required]
        public bool IsDeleted { get; set; }
    }
}
