using GDotnet.Reader.Api.DAL;
using GDotnet.Reader.Api.Protocol.Gx;
using System;
using System.Collections.Generic;
using System.Text;


// ==============================================================
//   Copyright (C) 2021 SZGxwl Inc. All rights reserved.
// 
//   Create by xiao.liu at 2021-04-06 08:21:24.
//
//   xiao.liu [mailto:fanqie0127@gmail.com]
// ============================================================== 
namespace GDotnet.Reader.Api.Example
{
    static class ExampleAntennaDetect
    {
        static void Main()
        {
            GClient clientConn = new GClient();
            eConnectionAttemptEventStatusType status;
            // clientConn.OpenSerial("COM18:115200", 3000, out status)
            if (clientConn.OpenTcp("192.168.1.168:8160", 3000, out status))
            {

                // stop 
                MsgBaseStop msgBaseStop = new MsgBaseStop();
                clientConn.SendSynMsg(msgBaseStop);
                if (0 == msgBaseStop.RtCode)
                {
                    Console.WriteLine("Stop successful.");
                }
                else { Console.WriteLine("Stop error."); }
                Console.WriteLine(AntennaDetect(clientConn, 1));
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("Connect failure.");
            }
            Console.ReadKey();
        }

        /// <summary>
        /// 通过前后向助波差值来判断天线是否正常连接，不同规格的天线差值不同，默认为大于40，可根据实际情况动态调整
        /// 读写器必须处理空闲状态才能检测天线端口，读卡时无法检测
        /// </summary>
        /// <param name="clientConn">读写器连接对象</param>
        /// <param name="antIndex">天线端口号</param>
        /// <returns>是否正常连接</returns>
        static bool AntennaDetect(GClient clientConn, uint antIndex)
        {
            if (null == clientConn) { return false; }
            uint antNo = antIndex;
            MsgTestCarrierWave msg = new MsgTestCarrierWave();
            msg.AntennaNum = antNo;
            msg.FreqCursor = 1;
            clientConn.SendSynMsg(msg);
            if (0 == msg.RtCode)
            {
                MsgTestVSWRcheck msgTestVSWRcheck = new MsgTestVSWRcheck();
                clientConn.SendSynMsg(msgTestVSWRcheck);
                clientConn.SendUnsynMsg(new MsgBaseStop());
                if (0 == msg.RtCode)
                {
                    byte diffValue = (byte)(msgTestVSWRcheck.PreValue - msgTestVSWRcheck.SufValue);
                    if (diffValue > 40)             // 默认为 大于40
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
