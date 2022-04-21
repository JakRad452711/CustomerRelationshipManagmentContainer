using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerRelationshipManagment.PredefinedValues
{
    public class ViewPaths
    {
        public static readonly String HOME_PATH = "/Main";
        public static readonly String SUCCESS_PATH = "~/Views/Shared/Success.cshtml";
        public static readonly String FAILURE_PATH = "~/Views/Shared/Failure.cshtml";

        public static String ControllerRootPath(String ControllerName)
        {
            return "~/" + ControllerName + "/Main";
        }
    }
}
