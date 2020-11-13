using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Kubernetes.Models;
using System.Text.RegularExpressions;
using System.Globalization;


namespace Kubernetes.Controllers
{
    
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TrainController : Controller
    {

        //[Route(@"daterange/{startDateString}/{endDateString}")]
        //[HttpGet]
        //public IActionResult GetByDateRange([FromUri] string startDateString, [FromUri] string endDateString)
        //{
        //    var startDate = BuildDateTimeFromYAFormat(startDateString);
        //    var endDate = BuildDateTimeFromYAFormat(endDateString);
        //    return null;
           
        //}

        [Route("{dateString}")]
        [HttpGet(Name = "GetStationSchedule")]
        public  IActionResult GetStationSchedule(string dateString)   
        {
  
            try
            {
                var startDate = ParseISO8601String(dateString).ToUniversalTime();
                var result = new Station<DateTime>().GetStationSchedules(startDate);

                return Ok(JsonSerializer.Serialize(result));
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }

            finally
            {   
                 
            }
        }




        [HttpPost(Name = "SaveStationSchedule")]
        public IActionResult SaveStationSchedule(StationSchedule<string> stationSchedule)
        {
            try
            {

                if (stationSchedule == null)
                {
                    throw new FormatException("Not a valid Input");
                }

                var dataResult =  new Station<String>().SaveStationSchedules(stationSchedule);
                return Ok(JsonSerializer.Serialize(dataResult));
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }

            finally
            {

            }
        }



        /// <summary>
        /// Convert a UTC Date String of format yyyyMMddThhmmZ into a Local Date
        /// </summary>
        /// <param name="dateString"></param>
        /// <returns></returns>
        private DateTime BuildDateTimeFromYAFormat(string dateString)
        {
            Regex r = new Regex(@"^\d{4}\d{2}\d{2}T\d{2}\d{2}Z$");
            if (!r.IsMatch(dateString))
            {
                throw new FormatException(
                    string.Format("{0} is not the correct format. Should be yyyyMMddTHHmmZ", dateString));
            }

            DateTime dt = DateTime.ParseExact(dateString, "yyyyMMddTHHmmZ", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);

            return dt;
        }


        static readonly string[] formats = { 
            // Basic formats
            "yyyyMMddTHHmmsszzz",
            "yyyyMMddTHHmmsszz",
            "yyyyMMddTHHmmssZ",
            // Extended formats
            "yyyy-MM-ddTHH:mm:sszzz",
            "yyyy-MM-ddTHH:mm:sszz",
            "yyyy-MM-ddTHH:mm:ssZ",
            // All of the above with reduced accuracy
            "yyyyMMddTHHmmzzz",
            "yyyyMMddTHHmmzz",
            "yyyyMMddTHHmmZ",
            "yyyy-MM-ddTHH:mmzzz",
            "yyyy-MM-ddTHH:mmzz",
            "yyyy-MM-ddTHH:mmZ",
            // Accuracy reduced to hours
            "yyyyMMddTHHzzz",
            "yyyyMMddTHHzz",
            "yyyyMMddTHHZ",
            "yyyy-MM-ddTHHzzz",
            "yyyy-MM-ddTHHzz",
            "yyyy-MM-ddTHHZ"
        };

        public static DateTime ParseISO8601String(string dateString)
        {
            try
            {

                Regex r = new Regex(@"^\d{4}\d{2}\d{2}T\d{2}\d{2}Z$");
                if (!r.IsMatch(dateString))
                {
                    throw new FormatException(
                        string.Format("{0} is not the correct format. Should be yyyyMMddTHHmmZ", dateString));
                }


                return DateTime.ParseExact(dateString, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }


    }
}
    