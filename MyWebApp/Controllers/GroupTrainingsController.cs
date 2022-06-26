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
            // prvo update za svaki grupni trening da li je u buducnosti
            GroupTrainings.UpdateGroupTrainings();
            return GroupTrainings.FindAllUpcomingByFitnessCenterId(fitnessId);
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

            if (!gt.Upcoming)
            {
                errorMessage = "That training has already happened";
                return false;
            }
            return true;
        }


        [Route("api/grouptrainings/visitedtrainings")]
        [HttpGet]
        [AllowAnonymous]
        public List<GroupTraining> GetVisitedTrainings()
        {
            // provera da li je korisnik ulogovan
            // ako nije vrati null, ako jeste vrati tog korisnika
            CookieHeaderValue cookieRecv = Request.Headers.GetCookies("session-id").FirstOrDefault();
            User u = ValidateUser(cookieRecv);
            if(u == null)
            {
                return null;
            }
            return GroupTrainings.FindVisitedGroupTrainings(u);
        }

        public List<GroupTraining> Get(string fitnessCenter, string name, string trainingType)
        {
            // provera da li je korisnik ulogovan
            // ako nije vrati null, ako jeste vrati tog korisnika
            CookieHeaderValue cookieRecv = Request.Headers.GetCookies("session-id").FirstOrDefault();
            User u = ValidateUser(cookieRecv);
            if (u == null)
            {
                return null;
            }
            List<GroupTraining> retVal = SearchVisitedTrainings(name, trainingType, fitnessCenter, u);
            return retVal;
        }

        public User ValidateUser(CookieHeaderValue cookieRecv)
        {
            string sessionId = "";
            if (cookieRecv == null)
            {
                return null;
            }
            sessionId = cookieRecv["session-id"].Value;
            if (sessionId == "")
            {
                return null;
            }
            User u = Users.FindById(int.Parse(sessionId));
            // ako jeste ulogovan, provera da li je posetilac
            if (u.UserType != EUserType.POSETILAC)
            {
                return null;
            }
            return u;
        }

        private List<GroupTraining> SearchVisitedTrainings(string name, string trainingType, string fitnessCenter, User u)
        {
            bool searchByName = !String.Equals(name, "noName");
            bool searchByType = !String.Equals(trainingType, "noType");
            bool searchByFitnessCenter = !String.Equals(fitnessCenter, "noFitnessCenter");
            List<GroupTraining> retVal = GroupTrainings.FindVisitedGroupTrainings(u);

            if (searchByName)
            {
                retVal = SearchByName(retVal, name);
                if (searchByType)
                {
                    retVal = SearchByType(retVal, trainingType);
                }
                if (searchByFitnessCenter)
                {
                    retVal = SearchByFitnessCenter(retVal, fitnessCenter);
                }
                return retVal;
            } else if (searchByType)
            {
                retVal = SearchByType(retVal, trainingType);
                if (searchByFitnessCenter)
                {
                    retVal = SearchByFitnessCenter(retVal, fitnessCenter);
                }
                return retVal;
            } else if (searchByFitnessCenter)
            {
                retVal = SearchByFitnessCenter(retVal, fitnessCenter);
                return retVal;
            }

            return retVal;
        }

        private List<GroupTraining> SearchByName(List<GroupTraining> list, string name)
        {
            List<GroupTraining> retVal = new List<GroupTraining>();
            foreach(var item in list)
            {
                if(item.Name.ToLower().Contains(name.ToLower()))
                    {
                    retVal.Add(item);
                }
            }
            return retVal;
        }

        private List<GroupTraining> SearchByType(List<GroupTraining> list, string trainingType)
        {
            List<GroupTraining> retVal = new List<GroupTraining>();
            foreach (var item in list)
            {
                if (item.TrainingType.ToLower().Contains(trainingType.ToLower()))
                {
                    retVal.Add(item);
                }
            }
            return retVal;
        }

        private List<GroupTraining> SearchByFitnessCenter(List<GroupTraining> list, string fitnessCenter)
        {
            List<GroupTraining> retVal = new List<GroupTraining>();
            foreach (var item in list)
            {
                if (item.FitnessCenterLocation.Name.ToLower().Contains(fitnessCenter.ToLower()))
                {
                    retVal.Add(item);
                }
            }
            return retVal;
        }
    }
}
