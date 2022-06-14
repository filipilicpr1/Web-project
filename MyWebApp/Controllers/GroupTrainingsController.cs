using MyWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MyWebApp.Controllers
{
    public class GroupTrainingsController : ApiController
    {
        public List<GroupTraining> GetByFitnessCenterId(int fitnessId)
        {
            // za sad vraca sve, verovatno treba logika da vraca samo buduce
            // moguce da postoji polje koje ce da kaze da li je trening u buducnosti
            return GroupTrainings.FindAllByFitnessCenterId(fitnessId);
        }

    }
}
