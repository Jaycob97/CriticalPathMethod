using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public class Action
    {
        public string name;
        public int previous;
        public int next;
        public double time;
        public bool isCritical;
        public double startTime;
        public List<int> nextActions;
    }
}
