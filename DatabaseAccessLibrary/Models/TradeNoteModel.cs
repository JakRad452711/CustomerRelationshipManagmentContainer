using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DatabaseAccessLibrary.Models
{
    public class TradeNoteModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [MaxLength(800)]
        public string Contents { get; set; }
        [Required]
        public int AssociatedCompany { get; set; }
        [Required]
        public int CreatorId { get; set; }
        [Required]
        public bool IsDeleted { get; set; }
    }
}
