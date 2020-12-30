using System;
using System.Collections.Generic;
using System.Text;

namespace Manifest.Config
{
    public class RdsConfig
    {
        public const string BaseUrl = "https://3s3sftsr90.execute-api.us-west-1.amazonaws.com/dev";
        public const string listAllOtherTA = "/api/v2/listAllTA";
        public const string aboutMeUrl = "/api/v2/aboutme";
        public const string goalsAndRoutinesUrl = "/api/v2/getgoalsandroutines";
        public const string UserIdFromEmailUrl = "/api/v2/userLogin";
        public const string actionAndTaskUrl = "/api/v2/actionsTasks";
        public const string updateGoalAndRoutine = "/api/v2/udpateGRWatchMobile";
        public const string updateActionAndTask = "/api/v2/updateATWatchMobile";

        //Endpoints for guid update and add
        public const string updateGuid = "/api/v2/updateGuid/update";
        public const string addGuid = "/api/v2/updateGuid/add";
    }
}
