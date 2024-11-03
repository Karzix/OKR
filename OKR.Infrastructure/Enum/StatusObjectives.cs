using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKR.Infrastructure.Enum
{
    public enum StatusObjectives
    {
        noStatus,
        onTrack,
        atRisk,
        offTrack,
        closed
    }

    public class FormatStatusObjectives
    {
        public static string getStatusObjectivesName(StatusObjectives type)
        {
            switch (type)
            {
                case StatusObjectives.noStatus:
                    return "No Sratus";
                case StatusObjectives.onTrack:
                    return "On Track";
                case StatusObjectives.atRisk:
                    return "At Risk";
                case StatusObjectives.offTrack:
                    return "Off Track";
                case StatusObjectives.closed:
                    return "Closed";
            }
            return type.ToString();
        }
    }
}
