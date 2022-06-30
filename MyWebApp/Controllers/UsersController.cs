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
    public class UsersController : ApiController
    {
        public HttpResponseMessage Get(int id)
        {
            User u = Users.FindById(id);
            if(u == null || u.Blocked)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Trazeni korisnik ne postoji");
            }
            return Request.CreateResponse(HttpStatusCode.OK, u);
        }

        public HttpResponseMessage Post(User user)
        {
            string errorMessage = "";
            bool update = false;
            HttpStatusCode code;
            bool isUserValid = ValidateUser(user, out errorMessage, out code, update);
            if (!isUserValid)
            {
                return Request.CreateResponse(code, errorMessage);
            }
            Users.AddUser(user);
            return Request.CreateResponse(HttpStatusCode.OK, "Korisnik uspesno registrovan");
        }

        public HttpResponseMessage Put(User user)
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
            if (!u.LoggedIn)
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized, "Not logged in");
            }
            if (u.Id != user.Id)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, "Not authorized");
            }
            string errorMessage = "";
            bool update = true;
            HttpStatusCode code;
            bool isUserValid = ValidateUser(user, out errorMessage, out code, update);
            if (!isUserValid)
            {
                return Request.CreateResponse(code, errorMessage);
            }
            Users.UpdateUser(user);
            return Request.CreateResponse(HttpStatusCode.OK, "Korisnik uspesno izmenjen");
        }

        // update dodat jer se ista metoda koristi za validaciju i kod PUT, gde username moze da je isti
        private bool ValidateUser(User user, out string errorMessage, out HttpStatusCode code, bool update)
        {
            errorMessage = "";
            code = HttpStatusCode.BadRequest;
            if (user == null)
            {
                errorMessage = "Greska prilikom prijema podataka";
                return false;
            }

            if (Users.FindByUsername(user.Username) != null && !update)
            {
                errorMessage = "Korisnik sa tim korisnickim imenom vec postoji";
                return false;
            }

            var usernameReg = @"^[\w -\.]{3,16}$";

            if (!Regex.IsMatch(user.Username, usernameReg, RegexOptions.IgnoreCase))
            {
                errorMessage = "Nevalidno korisnicko ime";
                return false;
            }

            var passwordReg = @"^[\w -\.]{3,16}$";

            if (!Regex.IsMatch(user.Password, passwordReg))
            {
                errorMessage = "Nevalidna lozinka";
                return false;
            }

            var emailReg = @"^([\w -\.]+@([\w -]+\.)+[\w-]{2,4})?$";

            if (!Regex.IsMatch(user.Email, emailReg))
            {
                errorMessage = "Nevalidan email";
                return false;
            }

            var nameReg = @"^[A-Z][a-zA-Z]{2,15}$";

            if (!Regex.IsMatch(user.Name, nameReg))
            {
                errorMessage = "Nevalidno ime";
                return false;
            }

            var lastNameReg = @"^[A-Z][a-zA-Z]{2,15}$";

            if (!Regex.IsMatch(user.LastName, lastNameReg))
            {
                errorMessage = "Nevalidno prezime";
                return false;
            }
            code = HttpStatusCode.OK;
            return true;
        }


        [Route("api/users/eligibletrainers")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetEligibleTrainers()
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
            return Request.CreateResponse(HttpStatusCode.OK, Users.FindEligibleTrainers());
        }

        [Route("api/users/registertrainer")]
        [HttpPut]
        [AllowAnonymous]
        public HttpResponseMessage RegisterTrainer(RegisterTrainerDTO registerTrainerDTO)
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
            User newTrainer = Users.FindById(registerTrainerDTO.UserId);
            FitnessCenter fc = FitnessCenters.FindById(registerTrainerDTO.FitnessCenterId);
            string errorMessage;
            HttpStatusCode code;
            bool isRegistrationValid = ValidateTrainerRegistration(newTrainer, u, fc, out errorMessage, out code);
            if (!isRegistrationValid)
            {
                return Request.CreateResponse(code, errorMessage);
            }
            Users.RegisterTrainer(newTrainer, fc);
            return Request.CreateResponse(HttpStatusCode.OK, "Trener registrovan");
        }

        private bool ValidateTrainerRegistration(User newTrainer, User owner, FitnessCenter fc, out string errorMessage, out HttpStatusCode code)
        {
            errorMessage = "";
            code = HttpStatusCode.BadRequest;

            if(newTrainer.UserType != EUserType.POSETILAC)
            {
                errorMessage = "User not eligible";
                return false;
            }
            if(newTrainer.VisitingGroupTrainings.Count != 0)
            {
                errorMessage = "User not eligible";
                return false;
            }
            if(fc.Owner.Id != owner.Id)
            {
                code = HttpStatusCode.Forbidden;
                errorMessage = "Not authorized";
                return false;
            }
            code = HttpStatusCode.OK;
            return true;
        }

        [Route("api/users/fitnesscentertrainers")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetFitnessCenterTrainers(int fitnessCenterId)
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
            return Request.CreateResponse(HttpStatusCode.OK, Users.FindFitnessCenterTrainers(FitnessCenters.FindById(fitnessCenterId)));
        }

        [Route("api/users/blocktrainer")]
        [HttpPut]
        [AllowAnonymous]
        public HttpResponseMessage BlockTrainer(User trainer)
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
            User blockTrainer = Users.FindById(trainer.Id);
            if(blockTrainer.UserType != EUserType.TRENER)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "User not a trainer");
            }
            if (blockTrainer.Blocked)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "User already blocked");
            }
            if (!CheckIfTrainerWorksForOwner(u, blockTrainer))
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, "Not authorized");
            }
            Users.BlockTrainer(blockTrainer);
            return Request.CreateResponse(HttpStatusCode.OK, "Trener blokiran");
        }

        private bool CheckIfTrainerWorksForOwner(User owner, User trainer)
        {
            foreach(var item in owner.FitnessCentersOwned)
            {
                if (item.Deleted)
                {
                    continue;
                }
                if(item.Id == trainer.FitnessCenterTrainer.Id)
                {
                    return true;
                }
            }
            return false;
        }

        [Route("api/users/login")]
        [HttpPost]
        [AllowAnonymous]
        public HttpResponseMessage Login(LoginDTO loginDTO)
        {
            string username = loginDTO.Username;
            string password = loginDTO.Password;
            string sessionId = "";
            CookieHeaderValue cookieRecv = Request.Headers.GetCookies("session-id").FirstOrDefault();
            if (cookieRecv != null)
            {
                sessionId = cookieRecv["session-id"].Value;
            }
            if(sessionId != "")
            {
                var r = new HttpResponseMessage(HttpStatusCode.BadRequest);
                r.Content = new StringContent("Already logged in");
                return r;
            }
            
            User u = Users.FindByUsername(username);
            if(u == null)
            {
                var r = new HttpResponseMessage(HttpStatusCode.BadRequest);
                r.Content = new StringContent("Invalid username");
                return r;
            }
            if(u.Password != password)
            {
                var r = new HttpResponseMessage(HttpStatusCode.BadRequest);
                r.Content = new StringContent("Invalid password");
                return r;
            }
            if (u.Blocked)
            {
                var r = new HttpResponseMessage(HttpStatusCode.BadRequest);
                r.Content = new StringContent("Access blocked");
                return r;
            }
            if (u.LoggedIn)
            {
                var r = new HttpResponseMessage(HttpStatusCode.BadRequest);
                r.Content = new StringContent("Already logged in, logout first");
                return r;
            }

            u.LoggedIn = true;
            var resp = new HttpResponseMessage();
            var cookie = GetCookie(u);
            resp.Headers.AddCookies(new CookieHeaderValue[] { cookie });
            return resp;
        }
        
        [Route("api/users/logout")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage Logout()
        {
            var resp = new HttpResponseMessage();
            CookieHeaderValue cookieRecv = Request.Headers.GetCookies("session-id").FirstOrDefault();
            User u = GetLoggedInUser(cookieRecv);
            if (u == null || !u.LoggedIn)
            {
                var r = new HttpResponseMessage(HttpStatusCode.BadRequest);
                r.Content = new StringContent("Niste prijavljeni");
                return r;
            }
            u.LoggedIn = false;
            resp.Headers.Remove("session-id");
            resp.Content = new StringContent("Uspesno ste se odjavili");
            var cookie = GetCookie(null);
            resp.Headers.AddCookies(new CookieHeaderValue[] { cookie });
            return resp;
        }

        private CookieHeaderValue GetCookie(User u)
        {
            string sessionId = u == null ? "" : u.Id.ToString();
            var cookie = new CookieHeaderValue("session-id", sessionId);
            // ako cookie ima expire date zadat, onda se ne brise kad se zatvori browser
            // ako nema zadat, onda se brise
            //cookie.Expires = DateTimeOffset.Now.AddDays(1); 
            cookie.Domain = Request.RequestUri.Host;
            cookie.Path = "/";
            return cookie;
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
    }
}
