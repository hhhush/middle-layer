using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 监控系统中间层
{
    interface IRobotScara
    {
        bool Connect();
        double Get();
        double Set();

    }
}
