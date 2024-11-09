using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKR.Infrastructure.Enum
{
    public enum Quarter
    {
        Quarter1,
        Quarter2,
        Quarter3,
        Quarter4
    }
    public class helperQuarter
    {
        public static Quarter GetCurrentQuarter()
        {
            // Lấy tháng hiện tại
            int month = DateTime.Now.Month;

            // Xác định quý dựa trên tháng
            return month switch
            {
                <= 3 => Quarter.Quarter1,
                <= 6 => Quarter.Quarter2,
                <= 9 => Quarter.Quarter3,
                _ => Quarter.Quarter4
            };
        }
    } 
}
