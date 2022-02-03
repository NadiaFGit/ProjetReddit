using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace PROJET_FIN.Models
{
    public partial class Comment
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Entrer une description")]
        public string Description { get; set; }

        public DateTime? PublicationDate { get; set; }
        public int? UserId { get; set; }
        public int? PostId { get; set; }

        public virtual Post Post { get; set; }
        public virtual User User { get; set; }
    }
}
