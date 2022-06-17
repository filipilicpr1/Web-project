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
    public class GroupTrainingsController : ApiController
    {
        public GroupTraining Get(int id)
        {
            return GroupTrainings.FindById(id);
        }

        public List<GroupTraining> GetByFitnessCenterId(int fitnessId)
        {
            // za sad vraca sve, verovatno treba logika da vraca samo buduce
            // moguce da postoji polje koje ce da kaze da li je trening u buducnosti
            return GroupTrainings.FindAllByFitnessCenterId(fitnessId);
        }

        [Route("api/grouptrainings/apply")]
        [HttpPut]
        [AllowAnonymous]
        public IHttpActionResult ApplyForTraining(GroupTraining groupTraining)
        {
            string sessionId = "";
            CookieHeaderValue cookieRecv = Request.Headers.GetCookies("session-id").FirstOrDefault();
            if (cookieRecv != null)
            {
                sessionId = cookieRecv["session-id"].Value;
            }
            if(sessionId == "")
            {
                return BadRequest("Not logged in");
            }
            User u = Users.FindById(int.Parse(sessionId));
            GroupTraining gt = GroupTrainings.FindById(groupTraining.Id);
            string errorMessage;
            bool isApplicationValid = ValidateApplicationForTraining(u, gt, out errorMessage);
            if (!isApplicationValid)
            {
                return BadRequest(errorMessage);
            }
            GroupTrainings.RegisterUserForTraining(u, gt);
            return Ok("Prijavili ste se na trening");
        }

        private bool ValidateApplicationForTraining(User u, GroupTraining gt, out string errorMessage)
        {
            errorMessage = "";
            if (u == null)
            {
                errorMessage = "Invalid user";
                return false;
            }

            if (u.UserType != EUserType.POSETILAC)
            {
                errorMessage = "Not authorized";
                return false;
            }
            
            if (gt == null)
            {
                errorMessage = "Invalid group training";
                return false;
            }

            if (gt.Visitors.Contains(u) || u.VisitingGroupTrainings.Contains(gt))
            {
                errorMessage = "User has already applied for this training";
                return false;
            }

            if (gt.VisitorCount == gt.VisitorCapacity)
            {
                errorMessage = "Full capacity reached for this training";
                return false;
            }
            return true;
        }
        
    }
}
