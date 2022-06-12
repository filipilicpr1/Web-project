using MyWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MyWebApp.Controllers
{
    public class FitnessCentersController : ApiController
    {
        public List<FitnessCenter> Get()
        {
            return FitnessCenters.FitnessCentersList;
        }
        
        public List<FitnessCenter> Get(string name, string address, int minYear, int maxYear)
        {
            bool searchByName = !String.Equals(name, "noName");
            bool searchByAddress = !String.Equals(address, "noAddress");

            if (searchByAddress)
            {
                return SearchByAddress(name, address, minYear, maxYear);
            }

            if (searchByName)
            {
                return SearchByName(name, minYear, maxYear);
            }

            return SearchByYear(minYear, maxYear);
            
        }

        public FitnessCenter Get(string address)
        {
            address = Uri.UnescapeDataString(address);
            address = address.Replace("+", " ");
            return FitnessCenters.FindByAddress(address);
        }



        private List<FitnessCenter> SearchByAddress(string name, string address, int minYear, int maxYear)
        {
            bool searchByName = !String.Equals(name, "noName");
            List<FitnessCenter> retVal = new List<FitnessCenter>();
            FitnessCenter fc = FitnessCenters.FindByAddress(address);
            if (fc == null)
            {
                return retVal;
            }
            if (fc.YearCreated < minYear || fc.YearCreated > maxYear)
            {
                return retVal;
            }
            if (searchByName && !String.Equals(fc.Name, name))
            {
                return retVal;
            }
            retVal.Add(fc);
            return retVal;
        }

        private List<FitnessCenter> SearchByName(string name, int minYear, int maxYear)
        {
            List<FitnessCenter> retVal = new List<FitnessCenter>();
            var temp = FitnessCenters.FindByName(name);
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
                if (fc.YearCreated >= minYear && fc.YearCreated <= maxYear)
                {
                    retVal.Add(fc);
                }
            }
            return retVal;
        }
    }
}
