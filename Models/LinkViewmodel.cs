using System.Collections.Generic;
using PROJET_FIN.Models;

namespace PROJET_FIN_2108.Models
{
    public class LinkViewmodel
    {
        public List<Comment>listcomment = new List<Comment>();
        public string description {get; set; }
        public int nbCommentaire {get; set; }
        
    }
}