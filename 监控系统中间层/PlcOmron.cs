using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 监控系统中间层
{
    class PlcOmron:IPlcOmron
    {
        bool IPlcOmron.Connect()
        {
            return true;
        }
        string IPlcOmron.Get(int address)
        {
            return "GetOmron";
        }

        string IPlcOmron.Set(int address)
        {
            return "SetOmron";
        }
    }
}
