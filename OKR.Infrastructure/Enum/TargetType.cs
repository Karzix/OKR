using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKR.Infrastructure.Enum
{
    public enum TargetType
    {
        individual,
        company,
        team
    }
    public class FormatTargetType
    {
        public static string getTargetTypeName(TargetType type)
        {
            switch (type)
            {
                case TargetType.individual:
                    return "Individual";
                case TargetType.company:
                    return "Branch";
                case TargetType.team:
                    return "Team";
            }
            return type.ToString();
        }
    }
}
