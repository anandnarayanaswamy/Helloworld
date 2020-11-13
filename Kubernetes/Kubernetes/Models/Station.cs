using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using System.Text.Json;
using System.Threading.Tasks;

namespace Kubernetes.Models
{

    public class StationSchedule<T>
    {

        public Train Train { get; set; } = new Train();

        public List<T> ScheduleTime { get; set; } = new List<T>();

    }




    public class Station<T>
    {


        public int Id { get; set; } = 1;

        public String Name { get; set; } = "Fulton Street";

        public List<StationSchedule<T>> Schedules { get; set; } = new List<StationSchedule<T>>();


        public Station<DateTime> GetStationSchedules(DateTime timeStamp)
        {
            try
            {
                var data = this.Fetch();
                var dataResult = new Station<DateTime>();
                var scheduleList = new List<Tuple<int, String, DateTime>>();

               

                if ( data != null)
                {
                    dataResult.Id = data.Id;
                    dataResult.Name = data.Name;
                    dataResult.Schedules = new List<StationSchedule<DateTime>>();

                    foreach (var item in data?.Schedules)
                    {

                        foreach (var schedule in item?.ScheduleTime)
                        {
                            var scheduleDateTime = DateTime.ParseExact(schedule, formats, null, System.Globalization.DateTimeStyles.None);
                            Tuple<int, String, DateTime> TrainList = new Tuple<int, string, DateTime>(item.Train.Id, item.Train.Number, scheduleDateTime);
                            scheduleList.Add(TrainList);
                        }

                    }

                    if (scheduleList.Any())
                    {
                        var maxSchedule  = scheduleList.Max(x => x.Item3);
                        var minSchedule  = scheduleList.Min(x => x.Item3);
                        var nextSchedule = new List<Tuple<int, String, DateTime>>();

                        var isScheudleInRange = timeStamp != DateTime.MinValue && timeStamp >= minSchedule && timeStamp <= maxSchedule;
                        

                        if (!isScheudleInRange)
                            nextSchedule = scheduleList.GroupBy(z => z.Item3).OrderBy(x => x.Key).Where(a => a.Count() > 1).FirstOrDefault().ToList();
                        else
                        {
                            var filter = scheduleList.Where(x => x.Item3 == timeStamp).GroupBy(z => z.Item3).OrderBy(a => a.Key);
                            
                            if (filter.Any() && filter.SelectMany(x => x).Count() > 1)
                                nextSchedule = filter.FirstOrDefault().ToList();
                        }

                        if (nextSchedule != null)
                        {
                            
                            foreach (var schedule in nextSchedule)
                            {
                                var trainSchedule = new StationSchedule<DateTime>
                                {
                                    Train = new Train { Id = (int)(schedule?.Item1), Number = schedule?.Item2 },
                                    ScheduleTime = new List<DateTime>() { (!isScheudleInRange)? schedule.Item3.AddDays(1) : schedule.Item3 }
                                };

                                dataResult.Schedules.Add(trainSchedule);
                            }
                        }
                    }
                }

                return dataResult;
            }
            catch(Exception ex)
            {
                throw ex;
            }
 
        }

        public Station<string> SaveStationSchedules(StationSchedule<string> stationSchedule)
        {
            try
            {
                var data = this.Fetch();
                bool hasExists = false;

                if (stationSchedule != null)
                {

                    foreach ( var schedule in data.Schedules)
                    {
                        if (schedule.Train.Number == stationSchedule.Train.Number)
                        {
                            hasExists = true;
                            schedule.ScheduleTime.AddRange(stationSchedule.ScheduleTime);
                        }
                    }

                    if (!hasExists)
                        data.Schedules.Add(stationSchedule);
                }
                    

                var serializeOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var dataResult = JsonSerializer.Serialize(data,serializeOptions);

                System.IO.File.WriteAllText(@".\Data\TrainSchedule.json",dataResult);


                data = this.Fetch();

                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private Station<String> Fetch()
        {
            try
            {
                var dataFile = System.IO.File.ReadAllText(@".\Data\TrainSchedule.json");

                var serializeOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var data = JsonSerializer.Deserialize<Station<String>>(dataFile, serializeOptions);
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }



        static readonly string[] formats = { 
            // Basic formats
            "h:mm tt",
            "H:mm tt"
        };




    }





}
