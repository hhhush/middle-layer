using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 监控系统中间层
{
    class DataItem
    {
        public string machineID;   //设备编号
        public string value;       //值
        public string content;     //语义
        public string timestamp;   //时间戳

        /// <summary>
        /// 把DataItem转化成字符串返回
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "*" + machineID + "#" + value + "#" + content + "#" + timestamp + "$";
        }
    }
}