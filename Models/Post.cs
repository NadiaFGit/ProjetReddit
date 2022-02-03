using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
#nullable disable

namespace PROJET_FIN.Models
{
    public partial class Post
    {
        public Post()
        {
            Comments = new HashSet<Comment>();
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Entrez un nom lien")]
        public string Link { get; set; }

        [Required(ErrorMessage = "Entrez une description")]
        public string Description { get; set; }
        public DateTime? PublicationDate { get; set; }
        public int? UpVote { get; set; }
        public int? DownVote { get; set; }
        public int? UserId { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
}
