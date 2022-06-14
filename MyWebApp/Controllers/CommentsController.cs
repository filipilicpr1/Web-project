using MyWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MyWebApp.Controllers
{
    public class CommentsController : ApiController
    {
        public List<Comment> GetByFitnessCenterId(int fitnessId)
        {
            return Comments.FindAllByFitnessCenterId(fitnessId);
        }
    }
}
