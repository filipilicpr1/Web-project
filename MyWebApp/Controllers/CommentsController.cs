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
        public HttpResponseMessage Get(int id)
        {
            return Request.CreateResponse(HttpStatusCode.OK, Comments.FindById(id));
        }

        public HttpResponseMessage GetByFitnessCenterId(int fitnessId)
        {
            CookieHeaderValue cookieRecv = Request.Headers.GetCookies("session-id").FirstOrDefault();
            if (FitnessCenters.FindById(fitnessId).Deleted)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Taj fitnes centar ne postoji");
            }
            bool userIsOwner = CheckIfUserIsOwner(cookieRecv, fitnessId);
            if (userIsOwner)
            {
                return Request.CreateResponse(HttpStatusCode.OK, Comments.FindAllByFitnessCenterId(fitnessId));
            }
            return Request.CreateResponse(HttpStatusCode.OK, Comments.FindAllVisibleByFitnessCenterId(fitnessId));
        }

        private bool CheckIfUserIsOwner(CookieHeaderValue cookieRecv, int fitnessId)
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
                if((fc.Id == fitnessId) && !fc.Deleted)
                {
                    return true;
                }
            }

            return false;
        }

        public HttpResponseMessage Post(CommentDTO commentDTO)
        {
            string sessionId = "";
            CookieHeaderValue cookieRecv = Request.Headers.GetCookies("session-id").FirstOrDefault();
            if (cookieRecv != null)
            {
                sessionId = cookieRecv["session-id"].Value;
            }
            if (sessionId == "")
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized, "Not logged in");
            }
            User u = Users.FindById(int.Parse(sessionId));
            if(u.UserType != EUserType.POSETILAC)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, "Not authorized");
            }

            if(commentDTO.Text.Trim() == "")
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Morate uneti komentar");
            }
            if (commentDTO.Text.Contains(";"))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Komentar ne sme da sadrzi specijalne znakove");
            }

            FitnessCenter fc = FitnessCenters.FindById(commentDTO.FitnessCenterId);
            if (fc.Deleted)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Fitnes centar ne postoji");
            }
            Comment c = Comments.CreateComment(commentDTO.Text, commentDTO.Rating, u, fc);
            Comments.AddComment(c);
            return Request.CreateResponse(HttpStatusCode.OK, "Komentar poslat");
        }

        public HttpResponseMessage Put(Comment comment)
        {
            Comment c = Comments.FindById(comment.Id);
            CookieHeaderValue cookieRecv = Request.Headers.GetCookies("session-id").FirstOrDefault();
            bool userIsOwner = CheckIfUserIsOwner(cookieRecv, c.RelatedFitnessCenter.Id);
            if (!userIsOwner)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, "Not authorized");
            }
            Comments.UpdateComment(comment);
            return Request.CreateResponse(HttpStatusCode.OK, "Komentar azuriran");
        }

    }
}
