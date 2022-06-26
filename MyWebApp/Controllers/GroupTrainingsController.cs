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
            GroupTrainings.UpdateGroupTrainings();
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
            GroupTrainings.UpdateGroupTrainings();
            // provera da li je korisnik ulogovan
            // ako nije vrati null, ako jeste vrati tog korisnika
            CookieHeaderValue cookieRecv = Request.Headers.GetCookies("session-id").FirstOrDefault();
            User u = GetLoggedInUser(cookieRecv);
            if(u == null)
            {
                return null;
            }
            if (u.UserType != EUserType.POSETILAC)
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
            User u = GetLoggedInUser(cookieRecv);
            if (u == null)
            {
                return null;
            }
            if (u.UserType != EUserType.POSETILAC)
            {
                return null;
            }
            return SearchVisitedTrainings(name, trainingType, fitnessCenter, u);
        }


        [Route("api/grouptrainings/completedtrainings")]
        [HttpGet]
        [AllowAnonymous]
        public List<GroupTraining> GetCompletedTrainings()
        {
            GroupTrainings.UpdateGroupTrainings();
            // provera da li je korisnik ulogovan
            // ako nije vrati null, ako jeste vrati tog korisnika
            CookieHeaderValue cookieRecv = Request.Headers.GetCookies("session-id").FirstOrDefault();
            User u = GetLoggedInUser(cookieRecv);
            if (u == null)
            {
                return null;
            }
            if (u.UserType != EUserType.TRENER)
            {
                return null;
            }
            return GroupTrainings.FindCompletedTrainingsByTrainer(u);
        }

        [Route("api/grouptrainings/trainersearch")]
        [HttpGet]
        [AllowAnonymous]
        public List<GroupTraining> GetTrainerSearch([FromUri]GroupTrainingSearchDTO groupTrainingSearchDTO)
        {
            GroupTrainings.UpdateGroupTrainings();
            CookieHeaderValue cookieRecv = Request.Headers.GetCookies("session-id").FirstOrDefault();
            User u = GetLoggedInUser(cookieRecv);
            if (u == null)
            {
                return null;
            }
            if (u.UserType != EUserType.TRENER)
            {
                return null;
            }
            return SearchCompletedTrainings(groupTrainingSearchDTO.Name, groupTrainingSearchDTO.TrainingType, groupTrainingSearchDTO.MinDate, groupTrainingSearchDTO.MaxDate,u);
        }

        public User GetLoggedInUser(CookieHeaderValue cookieRecv)
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
            return Users.FindById(int.Parse(sessionId));
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
            }
            if (searchByType)
            {
                retVal = SearchByType(retVal, trainingType);
            }
            if (searchByFitnessCenter)
            {
                retVal = SearchByFitnessCenter(retVal, fitnessCenter);
            }
            return retVal;
        }

        private List<GroupTraining> SearchCompletedTrainings(string name,string trainingType, DateTime minDate, DateTime maxDate, User u)
        {
            bool searchByName = !String.Equals(name, "noName");
            bool searchByType = !String.Equals(trainingType, "noType");
            List<GroupTraining> retVal = GroupTrainings.FindCompletedTrainingsByTrainer(u);
            if (searchByName)
            {
                retVal = SearchByName(retVal, name);
            }
            if (searchByType)
            {
                retVal = SearchByType(retVal, trainingType);
            }
            retVal = SearchByDate(retVal, minDate, maxDate);
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

        private List<GroupTraining> SearchByDate(List<GroupTraining> list, DateTime minDate, DateTime maxDate)
        {
            List<GroupTraining> retVal = new List<GroupTraining>();
            foreach(var item in list)
            {
                if(CompareDates(minDate,item.DateOfTraining) && CompareDates(item.DateOfTraining, maxDate))
                {
                    retVal.Add(item);
                }
            }
            return retVal;
        }

        // vraca true ake je date1 pre date2
        private bool CompareDates(DateTime date1, DateTime date2)
        {
            var day1 = date1.Day;
            var month1 = date1.Month;
            var year1 = date1.Year;
            var hour1 = date1.Hour;
            var minute1 = date1.Minute;

            var day2 = date2.Day;
            var month2 = date2.Month;
            var year2 = date2.Year;
            var hour2 = date2.Hour;
            var minute2 = date2.Minute;
            
            if (year1 < year2)
            {
                return true;
            }

            if (year1 > year2)
            {
                return false;
            }

            if (month1 < month2)
            {
                return true;
            }

            if (month1 > month2)
            {
                return false;
            }

            if (day1 < day2)
            {
                return true;
            }

            if (day1 > day2)
            {
                return false;
            }

            if (hour1 < hour2)
            {
                return true;
            }

            if (hour1 > hour2)
            {
                return false;
            }

            if (minute1 < minute2)
            {
                return true;
            }

            if (minute1 > minute2)
            {
                return false;
            }
            return true;    // ako su isti datumi vrati true
        }
    }
}
