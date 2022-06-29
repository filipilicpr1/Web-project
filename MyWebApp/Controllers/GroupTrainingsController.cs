using MyWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Web.Http;

namespace MyWebApp.Controllers
{
    public class GroupTrainingsController : ApiController
    {
        public HttpResponseMessage Get(int id)
        {
            GroupTraining gt = GroupTrainings.FindById(id);
            if(gt == null || gt.Deleted)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Trazeni grupni trening ne postoji");
            }
            return Request.CreateResponse(HttpStatusCode.OK, gt);
        }

        public HttpResponseMessage GetByFitnessCenterId(int fitnessId)
        {
            // prvo update za svaki grupni trening da li je u buducnosti
            GroupTrainings.UpdateGroupTrainings();
            FitnessCenter fc = FitnessCenters.FindById(fitnessId);
            if (fc == null || fc.Deleted)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Fitnes centar ne postoji");
            }
            return Request.CreateResponse(HttpStatusCode.OK, GroupTrainings.FindAllUpcomingByFitnessCenterId(fitnessId));
        }

        public HttpResponseMessage Post(GroupTraining groupTraining)
        {
            CookieHeaderValue cookieRecv = Request.Headers.GetCookies("session-id").FirstOrDefault();
            User u = GetLoggedInUser(cookieRecv);
            if (u == null)
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized, "Not logged in");
            }
            if (u.UserType != EUserType.TRENER)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, "Not authorized");
            }
            if (u.Blocked)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, "Blocked access");
            }
            string errorMessage = "";
            bool update = false;
            HttpStatusCode code;
            bool isGroupTrainingValid = ValidateGroupTraining(groupTraining, null, out errorMessage, out code, update);   // null je user jer on treba za proveru u POST vec u PUT
            if (!isGroupTrainingValid)
            {
                return Request.CreateResponse(code, errorMessage);
            }
            GroupTrainings.AddGroupTraining(groupTraining, u);
            return Request.CreateResponse(HttpStatusCode.OK, "Grupni trening kreiran");
        }
        
        public HttpResponseMessage Put(GroupTraining groupTraining)
        {
            CookieHeaderValue cookieRecv = Request.Headers.GetCookies("session-id").FirstOrDefault();
            User u = GetLoggedInUser(cookieRecv);
            if (u == null)
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized, "Not logged in");
            }
            if (u.UserType != EUserType.TRENER)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, "Not authorized");
            }
            if (u.Blocked)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, "Blocked access");
            }
            string errorMessage = "";
            bool update = true;
            HttpStatusCode code;
            bool isGroupTrainingValid = ValidateGroupTraining(groupTraining, u, out errorMessage, out code, update);
            if (!isGroupTrainingValid)
            {
                return Request.CreateResponse(code, errorMessage);
            }
            GroupTrainings.UpdateGroupTraining(groupTraining);
            return Request.CreateResponse(HttpStatusCode.OK, "Grupni trening izmenjen");
        }

        private bool ValidateGroupTraining(GroupTraining gt, User u, out string errorMessage, out HttpStatusCode code, bool update)
        {
            errorMessage = "";
            code = HttpStatusCode.BadRequest;
            var nameReg = @"^[A-Z][a-zA-Z0-9 ]{2,19}$";
            var reg = @"^[a-zA-Z0-9 ]{3,20}$";
            if (!Regex.IsMatch(gt.Name, nameReg))
            {
                errorMessage = "Nevalidan naziv treninga";
                return false;
            }
            if (!Regex.IsMatch(gt.TrainingType, reg))
            {
                errorMessage = "Nevalidan naziv tipa treninga";
                return false;
            }
            if (gt.Duration < 0 || gt.Duration > 999)
            {
                errorMessage = "Nevalidno trajanje treninga";
                return false;
            }
            if (gt.VisitorCapacity < 0 || gt.VisitorCapacity > 999)
            {
                errorMessage = "Nevalidan kapacitet treninga";
                return false;
            }
            DateTime currentDate = update ? DateTime.Now : DateTime.Now.AddDays(3);
            if (!CompareDates(currentDate, gt.DateOfTraining))
            {
                errorMessage = update ? "Trening ne moze biti u proslosti" : "Trening mora biti kreiran bar 3 dana unapred";
                return false;
            }

            if (!update)
            {
                code = HttpStatusCode.OK;
                return true;
            }

            GroupTraining originalGt = GroupTrainings.FindById(gt.Id);

            if (originalGt.Deleted)
            {
                code = HttpStatusCode.NotFound;
                errorMessage = "Trening ne postoji";
                return false;
            }
            if (!originalGt.Upcoming)
            {
                errorMessage = "Nije moguce menjati vec odrzane treninge";
                return false;
            }
            if(gt.VisitorCapacity < originalGt.VisitorCount)
            {
                errorMessage = "Nije moguce zadati manji kapacitet od prijavljenog broja posetioca";
                return false;
            }
            if (originalGt.FitnessCenterLocation.Deleted)
            {
                errorMessage = "Fitnes centar ne postoji";
                return false;
            }
            if(originalGt.FitnessCenterLocation.Id != u.FitnessCenterTrainer.Id)
            {
                code = HttpStatusCode.Forbidden;
                errorMessage = "Ne radite u fitnes centru u kojem se odrzava trening";
                return false;
            }
            if (!CheckIfUserIsTrainerOnGroupTraining(u,originalGt))
            {
                code = HttpStatusCode.Forbidden;
                errorMessage = "Niste trener na tom treningu";
                return false;
            }
            code = HttpStatusCode.OK;
            return true;
        }

        private bool CheckIfUserIsTrainerOnGroupTraining(User u, GroupTraining gt)
        {
            foreach(var item in u.TrainingGroupTrainings)
            {
                if((item.Id == gt.Id) && !item.Deleted)
                {
                    return true;
                }
            }
            return false;
        }

        public HttpResponseMessage Delete(int id)
        {
            GroupTrainings.UpdateGroupTrainings();
            CookieHeaderValue cookieRecv = Request.Headers.GetCookies("session-id").FirstOrDefault();
            User u = GetLoggedInUser(cookieRecv);
            GroupTraining gt = GroupTrainings.FindById(id);
            string errorMessage;
            HttpStatusCode code;
            bool isGroupTrainingValid = ValidateDeleteGroupTraining(gt, u, out errorMessage, out code);
            if (!isGroupTrainingValid)
            {
                return Request.CreateResponse(code, errorMessage);
            }
            GroupTrainings.DeleteGroupTraining(gt, u);
            return Request.CreateResponse(HttpStatusCode.OK, "Grupni trening obrisan");
        }

        private bool ValidateDeleteGroupTraining(GroupTraining gt, User u, out string errorMessage, out HttpStatusCode code)
        {
            errorMessage = "";
            code = HttpStatusCode.BadRequest;
            if(u == null)
            {
                code = HttpStatusCode.Unauthorized;
                errorMessage = "Not logged in";
                return false;
            }
            if(u.UserType != EUserType.TRENER)
            {
                code = HttpStatusCode.Forbidden;
                errorMessage = "Not authorized";
                return false;
            }
            if (u.Blocked)
            {
                code = HttpStatusCode.Forbidden;
                errorMessage = "Blocked access";
                return false;
            }
            bool userIsTrainer = CheckIfUserIsTrainerOnGroupTraining(u, gt);
            if(!userIsTrainer)
            {
                // ako korisnik nije trener na tom treningu
                code = HttpStatusCode.Forbidden;
                errorMessage = "Not authorized";
                return false;
            }
            if (!gt.Upcoming)
            {
                errorMessage = "Ne mozete brisati vec odrzan trening";
                return false;
            }
            if (gt.Deleted)
            {
                code = HttpStatusCode.NotFound;
                errorMessage = "Trening ne postoji";
                return false;
            }
            if(gt.VisitorCount > 0)
            {
                errorMessage = "Ne mozete brisati trening ukoliko ima vec prijavljenih posetilaca";
                return false;
            }
            if (gt.FitnessCenterLocation.Deleted)
            {
                errorMessage = "Fitnes centar ne postoji";
                return false;
            }
            if (gt.FitnessCenterLocation.Id != u.FitnessCenterTrainer.Id)
            {
                code = HttpStatusCode.Forbidden;
                errorMessage = "Ne radite u fitnes centru u kojem se odrzava trening";
                return false;
            }
            code = HttpStatusCode.OK;
            return true;
        }

        [Route("api/grouptrainings/apply")]
        [HttpPut]
        [AllowAnonymous]
        public HttpResponseMessage ApplyForTraining(GroupTraining groupTraining)
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
                return Request.CreateResponse(HttpStatusCode.Unauthorized, "Not logged in");
            }
            User u = Users.FindById(int.Parse(sessionId));
            GroupTraining gt = GroupTrainings.FindById(groupTraining.Id);
            string errorMessage;
            HttpStatusCode code;
            bool isApplicationValid = ValidateApplicationForTraining(u, gt, out errorMessage, out code);
            if (!isApplicationValid)
            {
                return Request.CreateResponse(code, errorMessage);
            }
            GroupTrainings.RegisterUserForTraining(u, gt);
            return Request.CreateResponse(HttpStatusCode.OK, "Prijavili ste se na trening");
        }

        private bool ValidateApplicationForTraining(User u, GroupTraining gt, out string errorMessage, out HttpStatusCode code)
        {
            errorMessage = "";
            code = HttpStatusCode.BadRequest;
            if (u == null)
            {
                code = HttpStatusCode.Unauthorized;
                errorMessage = "Not logged in";
                return false;
            }

            if (u.UserType != EUserType.POSETILAC)
            {
                code = HttpStatusCode.Forbidden;
                errorMessage = "Not authorized";
                return false;
            }
            
            if (gt == null)
            {
                errorMessage = "Invalid group training";
                return false;
            }

            if (gt.Deleted)
            {
                code = HttpStatusCode.NotFound;
                errorMessage = "Group training doesnt exist";
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
            if (gt.FitnessCenterLocation.Deleted)
            {
                errorMessage = "Fitnes centar ne postoji";
                return false;
            }
            code = HttpStatusCode.OK;
            return true;
        }

        [Route("api/grouptrainings/trainedtrainings")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetAllTrainedGroupTrainings()
        {
            GroupTrainings.UpdateGroupTrainings();
            // provera da li je korisnik ulogovan
            // ako nije vrati null, ako jeste vrati tog korisnika
            CookieHeaderValue cookieRecv = Request.Headers.GetCookies("session-id").FirstOrDefault();
            User u = GetLoggedInUser(cookieRecv);
            if (u == null)
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized, "Not logged in");
            }
            if (u.UserType != EUserType.TRENER)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, "Not authorized");
            }
            if (u.Blocked)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, "Blocked access");
            }
            return Request.CreateResponse(HttpStatusCode.OK, GroupTrainings.FindAllTrainingsByTrainer(u));
        }

        [Route("api/grouptrainings/visitedtrainings")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetVisitedTrainings()
        {
            GroupTrainings.UpdateGroupTrainings();
            // provera da li je korisnik ulogovan
            // ako nije vrati null, ako jeste vrati tog korisnika
            CookieHeaderValue cookieRecv = Request.Headers.GetCookies("session-id").FirstOrDefault();
            User u = GetLoggedInUser(cookieRecv);
            if(u == null)
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized, "Not logged in");
            }
            if (u.UserType != EUserType.POSETILAC)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, "Not authorized");
            }
            return Request.CreateResponse(HttpStatusCode.OK, GroupTrainings.FindVisitedGroupTrainings(u));
        }

        public HttpResponseMessage Get(string fitnessCenter, string name, string trainingType)
        {
            // provera da li je korisnik ulogovan
            // ako nije vrati null, ako jeste vrati tog korisnika
            CookieHeaderValue cookieRecv = Request.Headers.GetCookies("session-id").FirstOrDefault();
            User u = GetLoggedInUser(cookieRecv);
            if (u == null)
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized, "Not logged in");
            }
            if (u.UserType != EUserType.POSETILAC)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, "Not authorized");
            }
            return Request.CreateResponse(HttpStatusCode.OK, SearchVisitedTrainings(name, trainingType, fitnessCenter, u));
        }


        [Route("api/grouptrainings/completedtrainings")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetCompletedTrainings()
        {
            GroupTrainings.UpdateGroupTrainings();
            // provera da li je korisnik ulogovan
            // ako nije vrati null, ako jeste vrati tog korisnika
            CookieHeaderValue cookieRecv = Request.Headers.GetCookies("session-id").FirstOrDefault();
            User u = GetLoggedInUser(cookieRecv);
            if (u == null)
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized, "Not logged in");
            }
            if (u.UserType != EUserType.TRENER)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, "Not authorized");
            }
            if (u.Blocked)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, "Blocked access");
            }
            return Request.CreateResponse(HttpStatusCode.OK, GroupTrainings.FindCompletedTrainingsByTrainer(u));
        }

        [Route("api/grouptrainings/trainersearch")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetTrainerSearch([FromUri]GroupTrainingSearchDTO groupTrainingSearchDTO)
        {
            GroupTrainings.UpdateGroupTrainings();
            CookieHeaderValue cookieRecv = Request.Headers.GetCookies("session-id").FirstOrDefault();
            User u = GetLoggedInUser(cookieRecv);
            if (u == null)
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized, "Not logged in");
            }
            if (u.UserType != EUserType.TRENER)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, "Not authorized");
            }
            if (u.Blocked)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, "Blocked access");
            }
            return Request.CreateResponse(HttpStatusCode.OK, SearchCompletedTrainings(groupTrainingSearchDTO.Name, groupTrainingSearchDTO.TrainingType, groupTrainingSearchDTO.MinDate, groupTrainingSearchDTO.MaxDate, u));
        }

        private User GetLoggedInUser(CookieHeaderValue cookieRecv)
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
