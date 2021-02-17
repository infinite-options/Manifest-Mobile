using System;
using System.Collections.Generic;
using System.Net.Http;
using Manifest.Config;
using Newtonsoft.Json;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using Manifest.Models;

namespace Manifest.RDS
{
    public class RdsConnect
    {
        static HttpClient client = new HttpClient();

        //Use this function to send the guid of a user to the database
        public static async void storeGUID(string guid, string uid)
        {
            string url = RdsConfig.BaseUrl + RdsConfig.addGuid;
            Console.WriteLine("guid = " + guid + ", uid = " + uid);
            if (guid == null || guid == "")
            {
                return;
            }
            try
            {
                IDictionary<string, string> userDict = new Dictionary<string, string>();
                userDict.Add("user_unique_id", uid);
                userDict.Add("guid", guid);
                userDict.Add("notification", "TRUE");
                string json_data = JsonConvert.SerializeObject(userDict);
                //Now, send a PUT request to the end point
                var httpContent = new StringContent(json_data, Encoding.UTF8);
                var response = await client.PostAsync(url, httpContent);
                Debug.WriteLine("IN GUID TRY BLOCK");
                //response.Wait();
            }
            catch (Exception e)
            {
                Debug.WriteLine("IN GUID CATCH BLOCK");
                Debug.WriteLine(e);
                throw;
            }
            //Below is format to post
            //{     "user_unique_id": "100-000045",     "guid": "ndbfndbfnbn",     "notification": "FALSE" }
        }

        //
        public static async Task<string> getUser(string uid)
        {
            try
            {
                string url = RdsConfig.BaseUrl + RdsConfig.aboutMeUrl + "/" + uid;
                var res = await client.GetStringAsync(url);
                return res;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return "Failure";
            }
        }

        //Used to update a goal/routine
        public static async Task<string> updateOccurance(Occurance currOccurance, bool inprogress, bool iscomplete)
        {
            string url = RdsConfig.BaseUrl + RdsConfig.updateGoalAndRoutine;
            currOccurance.updateIsInProgress(inprogress);
            currOccurance.updateIsComplete(iscomplete);
            //Now, write to the database
            currOccurance.DateTimeStarted = DateTime.Now;
            Debug.WriteLine("Should be changed to in progress. InProgress = " + currOccurance.IsInProgress);
            //string toSend = updateOccurance(currOccurance);
            UpdateOccurance updateOccur = new UpdateOccurance()
            {
                id = currOccurance.Id,
                datetime_completed = currOccurance.DateTimeCompleted,
                datetime_started = currOccurance.DateTimeStarted,
                is_in_progress = currOccurance.IsInProgress,
                is_complete = currOccurance.IsComplete
            };
            string toSend = updateOccur.updateOccurance();
            var content = new StringContent(toSend);
            var res = await client.PostAsync(url, content);
            if (res.IsSuccessStatusCode)
            {
                Debug.WriteLine("Wrote to the datebase");
                return "Success";
            }
            else
            {
                Debug.WriteLine("Some error");
                Debug.WriteLine(toSend);
                Debug.WriteLine(res.ToString());
                return "Failure";
            }
            //return "Failure";
        }

        public static async Task<List<Occurance>> getOccurances(string url)
        {
            try
            {
                var response = await client.GetStringAsync(url);
                Debug.WriteLine("Getting user. User info below:");
                Debug.WriteLine(response);
                OccuranceResponse occuranceResponse = JsonConvert.DeserializeObject<OccuranceResponse>(response);
                //Debug.WriteLine(occuranceResponse);
                var toReturn = ToOccurances(occuranceResponse);
                return toReturn;
                //SortRoutines();
                //CreateList();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                //await DisplayAlert("Alert", "Error in TodaysListTest initialiseTodaysOccurances. Error: " + e.ToString(), "OK");
            }
            return null;
        }

        public static List<Occurance> ToOccurances(OccuranceResponse occuranceResponse)
        {
            try
            {
                //Clear the occurances, as we are going to get new one now
                List<Occurance> todaysRoutines = new List<Occurance>();
                if (occuranceResponse.result == null || occuranceResponse.result.Count == 0)
                {
                    Debug.WriteLine("No tasks today");
                    //DisplayAlert("No tasks today", "OK", "Cancel");
                }
                foreach (OccuranceDto dto in occuranceResponse.result)
                {
                    //Only add routines
                    if (dto.is_displayed_today == "True" && dto.is_persistent == "True")
                    {
                        Occurance toAdd = new Occurance();
                        if (dto.actions_tasks == null)
                        {
                            Debug.WriteLine("Actions and tasks are null");
                        }
                        toAdd.Id = dto.gr_unique_id;
                        toAdd.Title = dto.gr_title;
                        toAdd.PicUrl = dto.photo;
                        toAdd.IsPersistent = DataParser.ToBool(dto.is_persistent);
                        toAdd.IsInProgress = DataParser.ToBool(dto.is_in_progress);
                        toAdd.IsComplete = DataParser.ToBool(dto.is_complete);
                        toAdd.IsSublistAvailable = DataParser.ToBool(dto.is_sublist_available);
                        toAdd.ExpectedCompletionTime = DataParser.ToTimeSpan(dto.expected_completion_time);
                        toAdd.CompletionTime = dto.expected_completion_time;
                        toAdd.DateTimeCompleted = DataParser.ToDateTime(dto.datetime_completed);
                        toAdd.DateTimeStarted = DataParser.ToDateTime(dto.datetime_started);
                        toAdd.StartDayAndTime = DataParser.ToDateTime(dto.start_day_and_time);
                        toAdd.EndDayAndTime = DataParser.ToDateTime(dto.end_day_and_time);
                        toAdd.Repeat = DataParser.ToBool(dto.repeat);
                        toAdd.RepeatEvery = dto.repeat_every;
                        toAdd.RepeatFrequency = dto.repeat_frequency;
                        toAdd.RepeatType = dto.repeat_type;
                        toAdd.RepeatOccurences = dto.repeat_occurences;
                        toAdd.RepeatEndsOn = DataParser.ToDateTime(dto.repeat_ends_on);
                        //toAdd.RepeatWeekDays = ParseRepeatWeekDays(repeat_week_days);
                        toAdd.UserId = dto.user_id;
                        toAdd.IsEvent = false;
                        toAdd.NumSubOccurances = 0;
                        toAdd.SubOccurancesCompleted = 0;
                        toAdd.subOccurances = GetSubOccurances(dto.actions_tasks, toAdd);
                        todaysRoutines.Add(toAdd);
                    }
                }
                return todaysRoutines;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                //DisplayAlert("Alert", "Error in TodaysListTest ToOccurances(). Error: " + e.ToString(), "OK");
            }
            return null;
        }

        public static List<SubOccurance> GetSubOccurances(List<SubOccuranceDto> actions_tasks, Occurance parent)
        {
            List<SubOccurance> subTasks = new List<SubOccurance>();
            if (actions_tasks == null || actions_tasks.Count == 0)
            {
                return subTasks;
            }
            foreach (SubOccuranceDto dto in actions_tasks)
            {
                parent.NumSubOccurances++;
                //numTasks++;
                SubOccurance toAdd = new SubOccurance();
                toAdd.Id = dto.at_unique_id;
                toAdd.Title = dto.at_title;
                toAdd.GoalRoutineID = dto.goal_routine_id;
                toAdd.AtSequence = dto.at_sequence;
                toAdd.IsAvailable = DataParser.ToBool(dto.is_available);
                toAdd.IsComplete = DataParser.ToBool(dto.is_complete);
                if (toAdd.IsComplete)
                {
                    parent.SubOccurancesCompleted++;
                }
                toAdd.IsInProgress = DataParser.ToBool(dto.is_in_progress);
                toAdd.IsSublistAvailable = DataParser.ToBool(dto.is_sublist_available);
                toAdd.IsMustDo = DataParser.ToBool(dto.is_must_do);
                toAdd.PicUrl = dto.photo;
                toAdd.IsTimed = DataParser.ToBool(dto.is_timed);
                toAdd.DateTimeCompleted = DataParser.ToDateTime(dto.datetime_completed);
                toAdd.DateTimeStarted = DataParser.ToDateTime(dto.datetime_started);
                toAdd.ExpectedCompletionTime = DataParser.ToTimeSpan(dto.expected_completion_time);
                toAdd.AvailableStartTime = DataParser.ToDateTime(dto.available_start_time);
                toAdd.AvailableEndTime = DataParser.ToDateTime(dto.available_end_time);
                toAdd.instructions = GetInstructions(dto.instructions_steps);
                subTasks.Add(toAdd);
                Debug.WriteLine(toAdd.Id);
            }

            return subTasks;
        }

        public static List<Instruction> GetInstructions(List<InstructionDto> instruction_steps)
        {
            List<Instruction> instructions = new List<Instruction>();
            if (instruction_steps.Count == 0 || instruction_steps == null)
            {
                return instructions;
            }
            foreach (InstructionDto dto in instruction_steps)
            {
                Instruction toAdd = new Instruction();
                toAdd.unique_id = dto.unique_id;
                toAdd.title = dto.title;
                toAdd.at_id = dto.at_id;
                toAdd.IsSequence = int.Parse(dto.is_sequence);
                toAdd.IsAvailable = DataParser.ToBool(dto.is_available);
                toAdd.IsComplete = DataParser.ToBool(dto.is_complete);
                toAdd.IsInProgress = DataParser.ToBool(dto.is_in_progress);
                toAdd.IsTimed = DataParser.ToBool(dto.is_timed);
                toAdd.Photo = dto.photo;
                toAdd.expected_completion_time = DataParser.ToTimeSpan(dto.expected_completion_time);
                instructions.Add(toAdd);
            }

            return instructions;
        }
    }
}
