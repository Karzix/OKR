using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKR.Infrastructure.Enum
{
    public enum Status
    {
        noStatus,
        onTrack,
        atRisk,
        offTrack,
        closed
    }

    public class FormatStatusObjectives
    {
        public static string getStatusObjectivesName(Status type)
        {
            switch (type)
            {
                case Status.noStatus:
                    return "No Sratus";
                case Status.onTrack:
                    return "On Track";
                case Status.atRisk:
                    return "At Risk";
                case Status.offTrack:
                    return "Off Track";
                case Status.closed:
                    return "Closed";
            }
            return type.ToString();
        }
    }
}
