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
            bool searchByName = !String.Equals(name,"noName");
            bool searchByAddress = !String.Equals(address, "noAddress");
            List<FitnessCenter> retVal = new List<FitnessCenter>();
            if (searchByName)
            {
                FitnessCenter fc = FitnessCenters.FindByName(name);
                if(fc == null)
                {
                    return retVal;
                }
                if (searchByAddress)
                {
                    if (String.Equals(fc.Address, address) && fc.YearCreated >= minYear && fc.YearCreated <= maxYear)
                    {
                        retVal.Add(fc);
                    }
                } else
                {
                    if(fc.YearCreated >= minYear && fc.YearCreated <= maxYear)
                    {
                        retVal.Add(fc);
                    }
                }
                return retVal;
            }

            if (searchByAddress)
            {
                FitnessCenter fc = FitnessCenters.FindByAddress(address);
                if (fc == null)
                {
                    return retVal;
                }
                if (fc.YearCreated >= minYear && fc.YearCreated <= maxYear)
                {
                    retVal.Add(fc);
                    return retVal;
                }
            }

            foreach(var fc in FitnessCenters.FitnessCentersList)
            {
                if(fc.YearCreated >= minYear && fc.YearCreated <= maxYear)
                {
                    retVal.Add(fc);
                }
            }

            return retVal;
        }


        public FitnessCenter Get(string name)
        {
            name = Uri.UnescapeDataString(name);
            name = name.Replace("+", " ");
            return FitnessCenters.FindByName(name);
        }
    }
}
