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

        
        // pretragu radi tako da ime fitnes centra sadrzi ovo ime, isto i za adresu
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

        public FitnessCenter Get(int id)
        {
            //address = Uri.UnescapeDataString(address);
            //address = address.Replace("+", " ");
            return FitnessCenters.FindById(id);
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
                if (fc.YearCreated >= minYear && fc.YearCreated <= maxYear)
                {
                    retVal.Add(fc);
                }
            }
            return retVal;
        }
    }
}
