using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 监控系统中间层
{
    class MachineFanuc : IMachineFanuc
    {
        private ushort mFlibhndl;

        //获取加工中心句柄(输入IP地址、端口号、延时时间)
        #region cnc_allclibhndl3     
        short IMachineFanuc.GetMachineHandle(string IPAdress, ushort portNumber, short delayTime)
        {
            short ret = Focas1.cnc_allclibhndl3(IPAdress, portNumber, delayTime, out mFlibhndl);
            return ret;
        }
        #endregion

        //释放加工中心句柄
        #region cnc_freelibhndl
        short IMachineFanuc.FreeMachineHandle(ushort Flibhndl)
        {
            short ret = Focas1.cnc_freelibhndl(Flibhndl);
            return ret;
        }
        #endregion

        //获取状态信息
        #region cnc_statinfo
        short IMachineFanuc.GetMachineStateInfo(short[] stateInfo)
        {
            Focas1.ODBST odbst_StateInfo = new Focas1.ODBST();             //状态信息
            short ret = Focas1.cnc_statinfo(mFlibhndl, odbst_StateInfo);
            stateInfo[0] = odbst_StateInfo.tmmode;     //* T/M mode 
            stateInfo[1] = odbst_StateInfo.aut;        //selected automatic mode
            stateInfo[2] = odbst_StateInfo.run;        //running status
            stateInfo[3] = odbst_StateInfo.motion;     //axis, dwell status
            stateInfo[4] = odbst_StateInfo.mstb;       //m, s, t, b status
            stateInfo[5] = odbst_StateInfo.emergency;  //emergency stop status
            stateInfo[6] = odbst_StateInfo.alarm;      //alarm status
            stateInfo[7] = odbst_StateInfo.edit;       //editting status
            return ret;
        }
        #endregion

        //获取机械坐标
        #region cnc_machine   
        void IMachineFanuc.GetMachineCoor(double[] machCoor)
        {
            Focas1.ODBAXIS odbaxis_MachCoor = new Focas1.ODBAXIS();
            for (short i = 0; i <= 3; i++)
            {
                Focas1.cnc_machine(mFlibhndl, (short)(i + 1), 8, odbaxis_MachCoor);
                machCoor[i] = odbaxis_MachCoor.data[0] * Math.Pow(10, -3);
            }
        }
        #endregion

        //获取绝对位置坐标
        #region cnc_absolute2
        void IMachineFanuc.GetAbsoluteCoor(double[] absoluteCoor)
        {
            Focas1.ODBAXIS odbaxis_AbsoCoor = new Focas1.ODBAXIS();
            for (short i = 1; i <= 4; i++)
            {
                Focas1.cnc_absolute2(mFlibhndl, i, 8, odbaxis_AbsoCoor);
                absoluteCoor[i - 1] = odbaxis_AbsoCoor.data[0] * Math.Pow(10, -3);
            }
        }
        #endregion

        //读取相对位置坐标
        #region cnc_relative2
        void IMachineFanuc.GetRelativeCoor(double[] relativeCoor)
        {
            Focas1.ODBAXIS odbaxis_RelativeCoor = new Focas1.ODBAXIS();
            for (short i = 1; i <= 4; i++)
            {
                Focas1.cnc_relative2(mFlibhndl, i, 8, odbaxis_RelativeCoor);
                relativeCoor[i - 1] = odbaxis_RelativeCoor.data[0] * Math.Pow(10, -3);
            }
        }
        #endregion

        //读待走移动量
        #region cnc_distance
        void IMachineFanuc.GetDistanceToGo(double[] distanceCoor)
        {
            Focas1.ODBAXIS odbaxis_Distance = new Focas1.ODBAXIS();
            for (short i = 1; i <= 4; i++)
            {
                Focas1.cnc_distance(mFlibhndl, i, 8, odbaxis_Distance);
                distanceCoor[i - 1] = odbaxis_Distance.data[0] * Math.Pow(10, -3);
            }
        }
        #endregion

        //读CNC伺服轴的实际进给倍率
        #region cnc_actf
        short IMachineFanuc.GetActFeedrate(out long actFeedrate)
        {
            Focas1.ODBACT odbact_ActF = new Focas1.ODBACT();
            short ret = Focas1.cnc_actf(mFlibhndl, odbact_ActF);
            actFeedrate = odbact_ActF.data;
            return ret;
        }
        #endregion

        //读主轴转速 actual rotational speed (可一次读所有主轴) (使用这一个)
        #region cnc_acts2
        short IMachineFanuc.GetSpindleSpeed(out long SpindleSpeed)
        {
            Focas1.ODBACT2 odbact_RotationSpeed = new Focas1.ODBACT2();
            short ret = Focas1.cnc_acts2(mFlibhndl, 1, odbact_RotationSpeed);
            SpindleSpeed = odbact_RotationSpeed.data[0];
            return ret;
        }
        #endregion

        //(X、Y、Z、B轴) Reads the servo load meter data 
        #region cnc_rdsvmeter
        void IMachineFanuc.GetServoLoadMeter(double[] servoLoad)
        {
            Focas1.ODBSVLOAD odbsvload_ServoLoad = new Focas1.ODBSVLOAD();
            short numOfAxis = 4;
            Focas1.cnc_rdsvmeter(mFlibhndl, ref numOfAxis, odbsvload_ServoLoad);
            servoLoad[0] = odbsvload_ServoLoad.svload1.data;
            servoLoad[1] = odbsvload_ServoLoad.svload2.data;
            servoLoad[2] = odbsvload_ServoLoad.svload3.data;
            servoLoad[3] = odbsvload_ServoLoad.svload4.data;
        }
        #endregion

        //读取主轴负载 Reads the spindle load meter data and the spindle motor speed data 
        #region cnc_rdspmeter
        short IMachineFanuc.Getspload(out int slm)
        {
            Focas1.ODBSPLOAD odb_spLoadMeter = new Focas1.ODBSPLOAD();
            short ret;
            short numOfSpindle = 1;
            ret = Focas1.cnc_rdspmeter(mFlibhndl, 0, ref numOfSpindle, odb_spLoadMeter);
            slm = odb_spLoadMeter.spload1.spload.data;
            return ret;
        }
        #endregion

        //Reads the current selected path number which is the target path of the Data window functions.
        #region cnc_getpath
        short IMachineFanuc.GetPath(out short pathNum)
        {
            short maxPathNum;
            short ret = Focas1.cnc_getpath(mFlibhndl, out pathNum, out maxPathNum);
            return ret;
        }
        #endregion

        //Selects the path number which is the target path in the multi-path system or system with loader control. 
        #region cnc_setpath
        short IMachineFanuc.SetPath(short pathNum)
        {
            short ret = Focas1.cnc_setpath(mFlibhndl, pathNum);
            return ret;
        }
        #endregion

        //查询参数
        #region cnc_rdparam
        short IMachineFanuc.GetParameter(short para, out int res)
        {
            short ret;
            Focas1.IODBPSD iod = new Focas1.IODBPSD();
            ret = Focas1.cnc_rdparam(mFlibhndl, para, 0, 8, iod);
            res = iod.u.idata;
            return ret;
        }
        #endregion
    }
}
