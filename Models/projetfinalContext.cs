using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq;
using System.Collections.Generic;

#nullable disable

namespace PROJET_FIN.Models
{
    public partial class projetfinalContext : DbContext
    {
        public projetfinalContext()
        {
        }

        public projetfinalContext(DbContextOptions<projetfinalContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<Post> Posts { get; set; }
        public virtual DbSet<User> Users { get; set; }
        //manipuler la base de données:
        public List<User> getUser(string email)
        {
            return Users.Where(c => c.Email== email).ToList();
        }

        public List<Post> getPostForUser(int userid)
        {   
            return Posts.Where(a => a.UserId == userid).ToList();
        }

        public List<Comment> getCommentsForPost(int postid)
        {   
            return Comments.Where(c => c.PostId == postid).ToList();
        }



        //methode pour ajouter un user:
        public void AddNewUser(User user){
            Add<User>(user);
            SaveChanges();
        }

        //methode pour ajouter un post ou link:
        public void AddNewPost(Post post){
            Add<Post>(post);
            SaveChanges();
        }

        //methode pour ajouter un post ou link:
        public void AddNewComment(Comment comment){
            Add<Comment>(comment);
            SaveChanges();
        }


        // delete de Post:
        public void DeletePost (Post post){
            Remove<Post>(post);
            SaveChanges();
 }

            // Supprimer les commentaires:
        public void DeleteComment (Comment comment){
            Remove<Comment>(comment);
            SaveChanges();
 }




        public void UpdatePost(Post post){
        Update<Post>(post);
          SaveChanges();
        
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasCharSet("latin1")
                .UseCollation("latin1_swedish_ci");

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.ToTable("comment");

                entity.HasIndex(e => e.PostId, "PostID");

                entity.HasIndex(e => e.UserId, "UserID");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("ID");

                entity.Property(e => e.Description).HasMaxLength(200);

                entity.Property(e => e.PostId)
                    .HasColumnType("int(11)")
                    .HasColumnName("PostID");

                entity.Property(e => e.PublicationDate).HasColumnType("date");

                entity.Property(e => e.UserId)
                    .HasColumnType("int(11)")
                    .HasColumnName("UserID");

                entity.HasOne(d => d.Post)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.PostId)
                    .HasConstraintName("comment_ibfk_2");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("comment_ibfk_1");
            });

            modelBuilder.Entity<Post>(entity =>
            {
                entity.ToTable("post");

                entity.HasIndex(e => e.UserId, "UserID");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("ID");

                entity.Property(e => e.Description).HasMaxLength(200);

                entity.Property(e => e.DownVote)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Link).HasMaxLength(100);

                entity.Property(e => e.PublicationDate).HasColumnType("date");

                entity.Property(e => e.UpVote)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.UserId)
                    .HasColumnType("int(11)")
                    .HasColumnName("UserID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Posts)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("post_ibfk_1");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("ID");

                entity.Property(e => e.Email).HasMaxLength(30);

                entity.Property(e => e.Password).HasMaxLength(30);

                entity.Property(e => e.UserName).HasMaxLength(30);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
