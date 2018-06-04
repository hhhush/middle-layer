using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Xml;

namespace 监控系统中间层
{
    class Program
    {
        /*Socket通信*/
        private SocketCommunication socketCommunication = new SocketCommunication("177.177.1.2", 8080);  //输入IP地址和端口号

        /*fanuc数控系统*/
        private IMachineFanuc iMachineFanuc = new MachineFanuc();
        private string fanucMachineID = "1";      //设备编号
        private Timer timerGetFanucMachineInfo;   //获取数据
        private Timer timerSendFanucMachineInfo;  //发送数据
        private readonly ConcurrentQueue<DataItem> queueFanucMachineData = new ConcurrentQueue<DataItem>();       //Fanuc运行数据队列

        /*Omron PLC*/
        private IPlcOmron iPlcOmron = new PlcOmron();
        private string PlcOmronID = "2";
        private Timer timerGetOmronPLCInfo;   //获取数据
        private Timer timerSendOmronPLCInfo;  //发送数据
        XmlNodeList xmlNodeListOmron;
        private readonly ConcurrentQueue<DataItem> queueOmronPLCData = new ConcurrentQueue<DataItem>();       //Omron运行数据队列

        /*Robot Scara*/
        private IRobotScara iRobotScara = new RobotScara();
        private string RobotScaraID = "3";
        private Timer timerGetRobotScaraInfo;   //获取数据
        private Timer timerSendRobotScaraInfo;  //发送数据
        private readonly ConcurrentQueue<DataItem> queueRobotScaraData = new ConcurrentQueue<DataItem>();       //Robot Scara运行数据队列

        /*Siemens数控系统*/
        private IMachineSiemens iMachineSiemens = new MachineSiemens();
        private string SiemensMachineID = "4";
        private Timer timerGetSiemensMachineInfo;   //获取数据
        private Timer timerSendSiemensMachineInfo;  //发送数据
        private readonly ConcurrentQueue<DataItem> queueSiemensMachineData = new ConcurrentQueue<DataItem>(); //Siemens运行数据队列


        static void Main(string[] args)
        {
            Program program = new Program();
            program.Init();

        }


        /// <summary>
        /// 初始化,和服务器及设备建立连接，启动定时器，定时收发数据
        /// </summary>
        private void Init()
        {
            socketCommunication.Connect();  //建立与OPC UA服务器连接

            short isFanucConnect = iMachineFanuc.GetMachineHandle("192.168.1.1", 8193, 10);  //获取Fanuc系统的通信句柄
            if (isFanucConnect == 0)    //连接成功
            {
                timerGetFanucMachineInfo = new Timer(2000);
                timerGetFanucMachineInfo.Enabled = true;
                timerGetFanucMachineInfo.Elapsed += GetFanucMachineData;

                timerSendFanucMachineInfo = new Timer(2000);
                timerSendFanucMachineInfo.Enabled = true;
                timerSendFanucMachineInfo.Elapsed += SendFanucMachineData;
            }

            bool isOmronConnect = iPlcOmron.Connect();
            if (isOmronConnect)
            {
                GetConfig("PLCOmron.xml", "DataItem", out xmlNodeListOmron);
                timerGetOmronPLCInfo = new Timer(2000);
                timerGetOmronPLCInfo.Enabled = true;
                timerGetOmronPLCInfo.Elapsed += GetOmronPLCData;

                timerSendOmronPLCInfo = new Timer(2000);
                timerSendOmronPLCInfo.Enabled = true;
                timerSendOmronPLCInfo.Elapsed += SendOmronPLCData;
            }
            bool isRobotConnect = iRobotScara.Connect();
            if (isRobotConnect)
            {

                timerGetOmronPLCInfo = new Timer(2000);
                timerGetOmronPLCInfo.Enabled = true;
                timerGetOmronPLCInfo.Elapsed += GetRobotScara;

                timerSendOmronPLCInfo = new Timer(2000);
                timerSendOmronPLCInfo.Enabled = true;
                timerSendOmronPLCInfo.Elapsed += SendRobotScara;
            }

        }

        /// <summary>
        /// 将采集到的数据添加进队列
        /// </summary>
        /// <param name="machineID">设备号</param>
        /// <param name="value">参数值</param>
        /// <param name="content">参数名</param>
        /// <param name="timeStamp">时间戳</param>
        /// <param name="queue">队列</param>
        private void PushToSequence(string machineID, string value, string content, string timeStamp, ConcurrentQueue<DataItem> queue)
        {
            DataItem dataItem = new DataItem();
            dataItem.machineID = machineID;
            dataItem.value = value;
            dataItem.content = content;
            dataItem.timestamp = timeStamp;
            queue.Enqueue(dataItem);
        }

        /****************************************fanuc数控系统*************************************************************************/

        /// <summary>
        /// 采集Fanuc数据，定时调用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GetFanucMachineData(object sender, ElapsedEventArgs e)
        {
            GetFanucFeedrate();
        }


        /// <summary>
        /// 采集fanuc进给率，并存入队列
        /// </summary>
        private void GetFanucFeedrate()
        {
            long actFeedrate;
            iMachineFanuc.GetActFeedrate(out actFeedrate);
            PushToSequence(fanucMachineID, actFeedrate.ToString(), "进给率", DateTime.Now.ToString(), queueFanucMachineData);
        }


        /// <summary>
        ///  从Fanuc队列中取出数据，发送到服务器，定时调用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendFanucMachineData(object sender, ElapsedEventArgs e)
        {
            if (!queueFanucMachineData.IsEmpty)
            {
                DataItem dataItem = new DataItem();
                if (queueFanucMachineData.TryDequeue(out dataItem))
                {
                    socketCommunication.SendMessage(dataItem.ToString());
                }
            }
        }



        /// <summary>
        /// 从xml文件中读取PLC配置信息
        /// </summary>
        /// <param name="fileName">xml文件名</param>
        /// <param name="rootNode">根节点</param>
        /// <param name="xmlNodeList">节点列表</param>
        private void GetConfig(string fileName, string rootNode, out XmlNodeList xmlNodeList)
        {
            XmlDocument doc = new XmlDocument();
            //加载根目录下XML文件
            doc.Load(fileName);
            //获取根节点
            XmlElement root = doc.DocumentElement;
            //获取子节点
            xmlNodeList = root.GetElementsByTagName(rootNode);
        }

        /****************************************Omron PLC*************************************************************************/

        /// <summary>
        /// 根据 XmlNodeList 采集PLC数据，并存入队列
        /// </summary>
        /// <param name="xmlNodeList">节点列表</param>
        private void GetPlcData(XmlNodeList xmlNodeList)
        {
            foreach (XmlNode node in xmlNodeList)
            {
                string address = ((XmlElement)node).GetAttribute("address");
                string content = ((XmlElement)node).GetElementsByTagName("content")[0].InnerText;
                string value = iPlcOmron.Get(Convert.ToInt32(address));
                PushToSequence(PlcOmronID, value, content, DateTime.Now.ToString(), queueOmronPLCData);//队列名
            }
        }

        //采集OmronPLC数据
        /// <summary>
        /// 根据 xmlNodeListOmron 采集OmronPLC数据，并存入队列，定时调用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GetOmronPLCData(object sender, ElapsedEventArgs e)
        {
            GetPlcData(xmlNodeListOmron);
        }


        /// <summary>
        /// 从OmronPLC队列中取出数据，发送到服务器，定时调用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendOmronPLCData(object sender, ElapsedEventArgs e)
        {
            if (!queueOmronPLCData.IsEmpty)
            {
                DataItem dataItem = new DataItem();
                if (queueOmronPLCData.TryDequeue(out dataItem))
                {
                    socketCommunication.SendMessage(dataItem.ToString());
                }
            }
        }
        /****************************************Robot Scara*************************************************************************/
        /// <summary>
        /// 发送robotscara数据到服务器，定时调用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendRobotScara(object sender, ElapsedEventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 采集Robot Scara数据，并存入队列，定时调用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GetRobotScara(object sender, ElapsedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
