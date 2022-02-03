using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PROJET_FIN.Models;
using Microsoft.AspNetCore.Http;
using PROJET_FIN_2108.Models;


namespace PROJET_FIN.Controllers
{
    public class HomeController : Controller
    {
        private readonly projetfinalContext _context;
        private readonly ISession _session;

        public HomeController(projetfinalContext context, IHttpContextAccessor accessor)
        {
            this._context = context;
            this._session = accessor.HttpContext.Session;
        }

        private bool validation()
        {
            var validate = true;
            var email = _session.GetString("Email");
            var password = _session.GetString("Password");
            if (email != null && password != null)
            {

                return validate;
            }
            validate = false;
            return validate;
        }

        public IActionResult Index()
        {

            if (validation() == true)
            {
                ViewBag.Email = _session.GetString("Email");
                ViewBag.Password = _session.GetString("Password");
                ViewBag.UserName = _session.GetString("UserName");
                ViewBag.comments = _context.Comments.ToList();
                ViewBag.users = _context.Users.ToList();
                var sortRating = _context.Posts.ToList().OrderByDescending(p => p.UpVote - p.DownVote);// Sets the links in rating descending order
                return View(sortRating);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        //Login
        [HttpGet]
        public IActionResult Login(string? message)
        {
            ViewBag.message = message;
            User user = new User();
            return View(user);
        }

        [HttpPost]
        public IActionResult Login(User user)
        {
            var validation = _context.Users.Where(v => v.Email == user.Email && v.Password == user.Password).FirstOrDefault();
            if (validation != null)
            {
                _session.SetString("Email", user.Email);
                _session.SetString("Password", user.Password);
                _session.SetString("UserName", validation.UserName);
                _session.SetInt32("Userid", validation.Id);

                return RedirectToAction("Index");
            }
            else
            {
                /* ViewBag.message3="Courriel ou mot de passe invalide"; */
                return View(user);
            }
        }

        public IActionResult Register()
        {

            return View();
        }



        [HttpPost]
        public IActionResult Register(User user)
        { 
        
            if (user.UserName == null || user.Password == null || user.Email == null)
            {
                return View();
            }
            else
            {
                string message = "";
                var validation = _context.Users.Where(v => v.Email == user.Email).FirstOrDefault();
                
                if (validation == null)
                {
                    _context.AddNewUser(user);
                    message = "Votre inscription est bien réussite";
                    return RedirectToAction("Login", new { message = message });
                }
                else
                {
                    ViewBag.message2 = "Cet Email existe déja! SVP Entrez un nouvel Email";
                    return View();
                }
            }
        }



        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult MyLinks()
        {
            if (validation() == true)
            {
                ViewBag.UserName = _session.GetString("UserName");
                int id = _context.Users.ToList().Where(u => u.Email == _session.GetString("Email")).FirstOrDefault().Id;
                ViewBag.comments = _context.Comments.ToList();
                var result = _context.Posts.ToList().OrderByDescending(x => x.PublicationDate).Where(u => u.UserId == id);
                return View(result);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        //Ajouter des links et des postes:
        public IActionResult AddLink()
        {
            if (validation() == true)
            {

                ViewBag.UserName = _session.GetString("UserName");
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
        public IActionResult Addlink(string link, string description)
        {
            if (link==null || description==null)
            {
                ViewBag.message3 = "Remplir tous les champs";
             
                return View();
            } else {
            if (validation() == true)
            {
                ViewBag.Id = _session.GetInt32("Userid");
                Post post = new Post()
                {
                    Link = link,
                    Description = description,
                    PublicationDate = System.DateTime.Now,
                    UpVote = 0,
                    DownVote = 0,
                    UserId = _session.GetInt32("Userid"),
                };
                _session.SetInt32("CurrentPostId", post.Id);
                _context.AddNewPost(post);
                ViewBag.UserName = _session.GetString("UserName");
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Login");
            }
            }

        }

        public IActionResult Link(int postid)
        {
            if (validation() == true)       
            {
                var userid= _context.Posts.Find(postid).UserId;

                _session.SetString("currentPost", _context.Posts.Find(postid).Link);
                ViewBag.Linkpost = _context.Posts.Find(postid).Link;
                ViewBag.Description = _context.Posts.Find(postid).Description;
                ViewBag.UpVote = _context.Posts.Find(postid).UpVote;
                ViewBag.DownVote = _context.Posts.Find(postid).DownVote;
                ViewBag.PublicationDate=_context.Posts.Find(postid).PublicationDate;
                _session.SetInt32("currentPostId", _context.Posts.Find(postid).Id);
                ViewBag.users = _context.Users.ToList();
                ViewBag.UserName= _context.Users.Find(userid).UserName;
                ViewBag.howManyComments = _context.Comments.Where(c => c.PostId == postid).Count();
                ViewBag.PostID = postid;
                ViewBag.userid = _session.GetInt32("Userid");
                var result = _context.getCommentsForPost(postid).OrderByDescending(v => v.PublicationDate).ToList();
                LinkViewmodel model = new LinkViewmodel();
                model.listcomment = result;
                model.nbCommentaire = _context.Comments.Where(c => c.PostId == postid).Count();
                model.description = _context.Posts.Find(postid).Description;
                return View(model);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }


        //DeletePost:
        public IActionResult DeletePost(int postid)
        {

            var post = _context.Posts.Find(postid);
            var comment = _context.Comments.Where(c => c.PostId == postid).ToList();
            foreach (var item in comment)
            {
                _context.DeleteComment(item);
            }

            _context.DeletePost(post);
            return RedirectToAction("MyLinks");
        }



        //Logout
        public IActionResult Logout()
        {
            _session.Clear();
            return RedirectToAction("Login");
        }


        [HttpPost]
        public IActionResult AddComment(string description_c, int userid_c, int postid_c)
        {
            if (description_c!=null){

            
            
            var userid = _session.GetInt32("Userid");
            Comment comment = new Comment()
            {
                Description = description_c,
                PublicationDate = DateTime.Now,
                UserId = userid,
                PostId = _session.GetInt32("currentPostId")
            };
            ViewBag.users = _context.Users.ToList();
            ViewBag.UserName = _context.Users.Find(userid).UserName;
            _context.AddNewComment(comment);
            return RedirectToAction("Index");
            } else {
                return RedirectToAction("Link",new {postid=postid_c});
            }
        }



        [HttpPost]
        public IActionResult Likes(string op, int postid)
        {
            var post = _context.Posts.Find(postid);
            if (op == "downvote")
            {
                post.DownVote += 1;
            }
            else if (op == "upvote")
            {
                post.UpVote += 1;
            }

            _context.UpdatePost(post);


            return RedirectToAction("Link", new { PostID = postid });
        }





        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
