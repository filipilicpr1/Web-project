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
            return Request.CreateResponse(HttpStatusCode.OK, Users.FindById(id));
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
            user.UserType = EUserType.POSETILAC;
            user.VisitingGroupTrainings = new List<GroupTraining>();
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
            if(Users.FindById(int.Parse(sessionId)).Id != user.Id)
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
            if (cookieRecv == null)
            {
                var r = new HttpResponseMessage(HttpStatusCode.BadRequest);
                r.Content = new StringContent("Niste prijavljeni");
                return r;
            }
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
    }
}
