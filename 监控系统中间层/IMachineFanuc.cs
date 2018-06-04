using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 监控系统中间层
{
    interface IMachineFanuc
    {
        //获取句柄
        short GetMachineHandle(string IPAdress, ushort portNumber, short delayTime);

        //释放句柄
        short FreeMachineHandle(ushort Flibhndl);    

        //获取状态信息
        short GetMachineStateInfo(short[] stateInfo);

        //获取机械坐标X、Y、Z、B轴
        void GetMachineCoor(double[] MacCoor);

        //获取绝对位置坐标值
        void GetAbsoluteCoor(double[] AbsCoor);

        //读取相对位置坐标
        void GetRelativeCoor(double[] RelCoor);

        //读待走移动量
        void GetDistanceToGo(double[] DisCoor);

        //读主轴负载 
        short Getspload(out int slm);

        //读CNC伺服轴的实际进给倍率
        short GetActFeedrate(out long actFeedrate);

        //读主轴转速 actual rotational speed
        short GetSpindleSpeed(out long SpindleSpeed);

        //Reads the servo load meter data (X、Y、Z、B轴) 
        void GetServoLoadMeter(double[] servoLoad);

        //get path
        short GetPath(out short pathNum);

        //set path
        short SetPath(short pathNum);

        //Reads the parameter 
        short GetParameter(short para, out int res);
    }
}
