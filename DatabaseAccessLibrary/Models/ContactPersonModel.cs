using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DatabaseAccessLibrary.Models
{
    public class ContactPersonModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [MaxLength(30)]
        public string Name { get; set; }
        [Required]
        [MaxLength(70)]
        public string Surname { get; set; }
        [Required]
        [MaxLength(20)]
        public string PhoneNumber { get; set; }
        [Required]
        [MaxLength(100)]
        public string EmailAddress { get; set; }
        [Required]
        [MaxLength(30)]
        public string Position { get; set; }
        [Required]
        public int AssociatedCompanyId { get; set; }
        [Required]
        public int CreatorId { get; set; }
        [Required]
        public bool IsDeleted { get; set; }
    }
}