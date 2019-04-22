using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
     public class Event
    {
        public double earliestTime = 0;
        public double latestTime = 0;
        public double timeGap;
        public List<int> previousEvent = new List<int>();
        public List<int> previousAction = new List<int>();
        public List<int> nextEvent = new List<int>();
        public List<int> nextAction = new List<int>();
    }
}
