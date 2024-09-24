using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OKR.Infrastructure
{
    public static class SignalRMessage
    {
        public static string WeightUpdate { get; set; } = "ReceiveMessageWeightUpdate";
    }
}
