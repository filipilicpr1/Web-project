using MyWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace MyWebApp.Controllers
{
    public class CommentsController : ApiController
    {
        public Comment Get(int id)
        {
            return Comments.FindById(id);
        }

        public List<Comment> GetByFitnessCenterId(int fitnessId)
        {
            CookieHeaderValue cookieRecv = Request.Headers.GetCookies("session-id").FirstOrDefault();
            bool userIsOwner = CheckIfUserIsOwner(cookieRecv, fitnessId);
            if (userIsOwner)
            {
                return Comments.FindAllByFitnessCenterId(fitnessId);
            }
            return Comments.FindAllVisibleByFitnessCenterId(fitnessId);
        }

        public bool CheckIfUserIsOwner(CookieHeaderValue cookieRecv, int fitnessId)
        {
            string sessionId = "";
            if (cookieRecv == null)
            {
                return false;
            }
            sessionId = cookieRecv["session-id"].Value;
            if (sessionId == "")
            {
                return false;
            }
            User u = Users.FindById(int.Parse(sessionId));
            // ako jeste ulogovan, provera da li je posetilac
            if (u.UserType != EUserType.VLASNIK)
            {
                return false;
            }

            foreach(var fc in u.FitnessCentersOwned)
            {
                if(fc.Id == fitnessId)
                {
                    return true;
                }
            }

            return false;
        }

        public IHttpActionResult Post(CommentDTO commentDTO)
        {
            string sessionId = "";
            CookieHeaderValue cookieRecv = Request.Headers.GetCookies("session-id").FirstOrDefault();
            if (cookieRecv != null)
            {
                sessionId = cookieRecv["session-id"].Value;
            }
            if (sessionId == "")
            {
                return BadRequest("Not logged in");
            }
            User u = Users.FindById(int.Parse(sessionId));
            if(u.UserType != EUserType.POSETILAC)
            {
                return BadRequest("Not authorized");
            }

            if(commentDTO.Text.Trim() == "")
            {
                return BadRequest("Morate uneti komentar");
            }
            if (commentDTO.Text.Contains(";"))
            {
                return BadRequest("Komentar ne sme da sadrzi specijalne znakove");
            }

            FitnessCenter fc = FitnessCenters.FindById(commentDTO.FitnessCenterId);
            Comment c = Comments.CreateComment(commentDTO.Text, commentDTO.Rating, u, fc);
            Comments.AddComment(c);
            return Ok("Komentar poslat");
        }

        public IHttpActionResult Put(Comment comment)
        {
            Comment c = Comments.FindById(comment.Id);
            CookieHeaderValue cookieRecv = Request.Headers.GetCookies("session-id").FirstOrDefault();
            bool userIsOwner = CheckIfUserIsOwner(cookieRecv, c.RelatedFitnessCenter.Id);
            if (!userIsOwner)
            {
                return BadRequest("Not authorized");
            }
            Comments.UpdateComment(comment);
            return Ok("Komentar azuriran");
        }

    }
}
