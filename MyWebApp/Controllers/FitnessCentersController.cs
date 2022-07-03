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
    public class FitnessCentersController : ApiController
    {
        public HttpResponseMessage Get()
        {
            return Request.CreateResponse(HttpStatusCode.OK, FitnessCenters.FitnessCentersList.FindAll(item => !item.Deleted));
        }


        // pretragu radi tako da ime fitnes centra sadrzi ovo ime, isto i za adresu
        [Route("api/fitnesscenters/search")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage Get([FromUri]FitnessCenterSearchDTO fitnessCenterSearchDTO)
        {
            string name = fitnessCenterSearchDTO.Name;
            string address = fitnessCenterSearchDTO.Address;
            int minYear = fitnessCenterSearchDTO.MinYear;
            int maxYear = fitnessCenterSearchDTO.MaxYear;
            bool searchByName = !String.Equals(name, "noName");
            bool searchByAddress = !String.Equals(address, "noAddress");

            if (searchByAddress)
            {
                return Request.CreateResponse(HttpStatusCode.OK, SearchByAddress(name, address, minYear, maxYear));
            }

            if (searchByName)
            {
                return Request.CreateResponse(HttpStatusCode.OK, SearchByName(name, minYear, maxYear));
            }
            return Request.CreateResponse(HttpStatusCode.OK, SearchByYear(minYear, maxYear));
        }

        public HttpResponseMessage Get(int id)
        {
            //address = Uri.UnescapeDataString(address);
            //address = address.Replace("+", " ");
            FitnessCenter fc = FitnessCenters.FindById(id);
            if(fc == null || fc.Deleted)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Fitnes centar ne postoji");
            }
            return Request.CreateResponse(HttpStatusCode.OK, fc);
        }

        [Route("api/fitnesscenters/getowned")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetOwnedFitnessCenters()
        {
            CookieHeaderValue cookieRecv = Request.Headers.GetCookies("session-id").FirstOrDefault();
            User u = GetLoggedInUser(cookieRecv);
            if (u == null || !u.LoggedIn)
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized, "Not logged in");
            }
            if (u.UserType != EUserType.VLASNIK)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, "Not authorized");
            }
            return Request.CreateResponse(HttpStatusCode.OK, FitnessCenters.FindAllOwned(u));
        }

        public HttpResponseMessage Post(FitnessCenter fitnessCenter)
        {
            CookieHeaderValue cookieRecv = Request.Headers.GetCookies("session-id").FirstOrDefault();
            User u = GetLoggedInUser(cookieRecv);
            if (u == null || !u.LoggedIn)
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized, "Not logged in");
            }
            if (u.UserType != EUserType.VLASNIK)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, "Not authorized");
            }
            string errorMessage = "";
            bool update = false;
            HttpStatusCode code;
            bool isFitnessCenterValid = ValidateFitnessCenter(fitnessCenter, null, out errorMessage, out code, update);   // null je user jer on treba za proveru u POST vec u PUT
            if (!isFitnessCenterValid)
            {
                return Request.CreateResponse(code, errorMessage);
            }
            FitnessCenters.AddFitnessCenter(fitnessCenter, u);
            return Request.CreateResponse(HttpStatusCode.OK, "Fitnes centar kreiran");
        }

        public HttpResponseMessage Put(FitnessCenter fitnessCenter)
        {
            CookieHeaderValue cookieRecv = Request.Headers.GetCookies("session-id").FirstOrDefault();
            User u = GetLoggedInUser(cookieRecv);
            if (u == null || !u.LoggedIn)
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized, "Not logged in");
            }
            if (u.UserType != EUserType.VLASNIK)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, "Not authorized");
            }
            string errorMessage = "";
            bool update = true;
            HttpStatusCode code;
            bool isFitnessCenterValid = ValidateFitnessCenter(fitnessCenter, u, out errorMessage, out code, update);
            if (!isFitnessCenterValid)
            {
                return Request.CreateResponse(code, errorMessage);
            }
            FitnessCenters.UpdateFitnessCenter(fitnessCenter);
            return Request.CreateResponse(HttpStatusCode.OK, "Fitnes centar izmenjen");
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

        private bool ValidateFitnessCenter(FitnessCenter fc, User u, out string errorMessage, out HttpStatusCode code, bool update)
        {
            errorMessage = "";
            code = HttpStatusCode.BadRequest;
            var reg = @"^[A-Z][a-zA-Z0-9 ]{2,19}$";
            if (!Regex.IsMatch(fc.Name, reg))
            {
                errorMessage = "Nevalidan naziv fitnes centra";
                return false;
            }
            string[] streetParts = fc.Address.Split(',')[0].Split(' ');
            string street = "";
            for(int i = 0; i < streetParts.Length -1; i++)
            {
                street += streetParts[i] + " ";
            }
            street = street.Trim();
            int number = int.Parse(streetParts[streetParts.Length - 1]);
            string place = fc.Address.Split(',')[1].Trim();
            int zip = int.Parse(fc.Address.Split(',')[2].Trim());
            if (!Regex.IsMatch(street, reg))
            {
                errorMessage = "Nevalidan naziv ulice";
                return false;
            }
            if (!Regex.IsMatch(place, reg))
            {
                errorMessage = "Nevalidan naziv mesta";
                return false;
            }
            if (number < 0 || number > 999)
            {
                errorMessage = "Nevalidno broj ulice";
                return false;
            }
            if (zip < 10000 || zip > 99999)
            {
                errorMessage = "Nevalidan postanski broj";
                return false;
            }
            if(fc.MonthlySubscription < 0 || fc.MonthlySubscription > 1000000)
            {
                errorMessage = "Nevalidna mesecna clanarina";
                return false;
            }
            if (fc.YearlySubscription < 0 || fc.YearlySubscription > 1000000)
            {
                errorMessage = "Nevalidna godisnja clanarina";
                return false;
            }
            if (fc.TrainingCost < 0 || fc.TrainingCost > 100000)
            {
                errorMessage = "Nevalidna cena treninga";
                return false;
            }
            if (fc.GroupTrainingCost < 0 || fc.GroupTrainingCost > 100000)
            {
                errorMessage = "Nevalidna cena grupnog treninga";
                return false;
            }
            if (fc.PersonalTrainingCost < 0 || fc.PersonalTrainingCost > 100000)
            {
                errorMessage = "Nevalidna cena personalnog treninga";
                return false;
            }
            if (!update)
            {
                code = HttpStatusCode.OK;
                return true;
            }
            FitnessCenter originalFc = FitnessCenters.FindById(fc.Id);
            if (originalFc.Deleted)
            {
                errorMessage = "Fitnes centar ne postoji";
                return false;
            }
            if(originalFc.Owner.Id != u.Id)
            {
                code = HttpStatusCode.Forbidden;
                errorMessage = "Not authorized";
                return false;
            }
            code = HttpStatusCode.OK;
            return true;
        }

        public HttpResponseMessage Delete(int id)
        {
            GroupTrainings.UpdateGroupTrainings();
            CookieHeaderValue cookieRecv = Request.Headers.GetCookies("session-id").FirstOrDefault();
            User u = GetLoggedInUser(cookieRecv);
            FitnessCenter fc = FitnessCenters.FindById(id);
            string errorMessage;
            HttpStatusCode code;
            bool isFitnessCenterValid = ValidateDeleteFitnessCenter(fc, u, out errorMessage, out code);
            if (!isFitnessCenterValid)
            {
                return Request.CreateResponse(code, errorMessage);
            }
            FitnessCenters.DeleteFitnessCenter(fc);
            return Request.CreateResponse(HttpStatusCode.OK, "Fitnes centar obrisan");
        }

        private bool ValidateDeleteFitnessCenter(FitnessCenter fc, User u, out string errorMessage, out HttpStatusCode code)
        {
            errorMessage = "";
            code = HttpStatusCode.BadRequest;
            if (u == null || !u.LoggedIn)
            {
                code = HttpStatusCode.Unauthorized;
                errorMessage = "Not logged in";
                return false;
            }
            if (u.UserType != EUserType.VLASNIK)
            {
                code = HttpStatusCode.Forbidden;
                errorMessage = "Not authorized";
                return false;
            }
            if(fc.Owner.Id != u.Id)
            {
                code = HttpStatusCode.Forbidden;
                errorMessage = "Not authorized";
                return false;
            }
            if (GroupTrainings.FitnessCenterHasUpcomingGroupTrainings(fc))
            {
                errorMessage = "Ne mozete brisati fitnes centar koji ima zakazane grupne treninge";
                return false;
            }

            code = HttpStatusCode.OK;
            return true;
        }

        private List<FitnessCenter> SearchByAddress(string name, string address, int minYear, int maxYear)
        {
            bool searchByName = !String.Equals(name, "noName");
            List<FitnessCenter> retVal = new List<FitnessCenter>();
            var temp = FitnessCenters.SearchAllByAddress(address);
            foreach(var fc in temp)
            {
                if(fc.YearCreated >= minYear && fc.YearCreated <= maxYear)
                {
                    retVal.Add(fc);
                }
            }
            if (!searchByName)
            {
                return retVal;
            }
            temp = retVal;
            retVal = new List<FitnessCenter>();
            foreach(var fc in temp)
            {
                if (fc.Name.ToLower().Contains(name.ToLower()))
                {
                    retVal.Add(fc);
                }
            }
            return retVal;
        }

        private List<FitnessCenter> SearchByName(string name, int minYear, int maxYear)
        {
            List<FitnessCenter> retVal = new List<FitnessCenter>();
            var temp = FitnessCenters.SearchAllByName(name);
            foreach (var item in temp)
            {
                if (item.YearCreated >= minYear && item.YearCreated <= maxYear)
                {
                    retVal.Add(item);
                }
            }
            return retVal;
        }

        private List<FitnessCenter> SearchByYear(int minYear, int maxYear)
        {
            List<FitnessCenter> retVal = new List<FitnessCenter>();
            foreach (var fc in FitnessCenters.FitnessCentersList)
            {
                if (fc.Deleted)
                {
                    continue;
                }
                if (fc.YearCreated >= minYear && fc.YearCreated <= maxYear)
                {
                    retVal.Add(fc);
                }
            }
            return retVal;
        }
    }
}
