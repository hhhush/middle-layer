using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 监控系统中间层
{
    interface IPlcOmron
    {
        //连接
        bool Connect();     

        //采集PLC数据
        string Get(int address);

        //设置PLC数据
        string Set(int address);
    }
}
