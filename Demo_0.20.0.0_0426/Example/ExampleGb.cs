using GDotnet.Reader.Api.DAL;
using GDotnet.Reader.Api.Protocol.Gx;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;


// ==============================================================
//   Copyright (C) 2019 SZGxwl Inc. All rights reserved.
// 
//   Create by xiao.liu at 2019/1/11 11:22:51.
//
//   xiao.liu [mailto:fanqie0127@gmail.com]
// ============================================================== 
namespace GDotnet.Reader.Api
{
    static class ExampleGb
    {
        static void Main()
        {
            GClient clientConn = new GClient();
            eConnectionAttemptEventStatusType status;
            // clientConn.OpenTcp("192.168.1.168:8160", 3000, out status)
            if (clientConn.OpenSerial("COM6:115200", 3000, out status))
            {
                // subscribe to event
                clientConn.OnEncapedTagGbLog += new delegateEncapedTagGbLog(OnEncapedTagGbLog);
                clientConn.OnEncapedTagGbOver += new delegateEncapedTagGbOver(OnEncapedTagGbOver);

                // stop 
                MsgBaseStop msgBaseStop = new MsgBaseStop();
                clientConn.SendSynMsg(msgBaseStop);
                if (0 == msgBaseStop.RtCode)
                {
                    Console.WriteLine("Stop successful.");
                }
                else { Console.WriteLine("Stop error."); }

                // MsgAppSetGpo msgAppSetGpo = new MsgAppSetGpo();
                // msgAppSetGpo.Gpo1 = 1;
                // clientConn.SendUnsynMsg(msgAppSetGpo);
                // Thread.Sleep(500);
                // msgAppSetGpo.Gpo1 = 0;
                // clientConn.SendUnsynMsg(msgAppSetGpo);

                // power of the four antennas is set at 30dBm
                //MsgBaseSetPower msgBaseSetPower = new MsgBaseSetPower();
                //msgBaseSetPower.DicPower = new Dictionary<byte, byte>()
                //{
                //    {1, 30},
                //    {2, 30},
                //    {3, 30},
                //    {4, 30}
                //};
                //clientConn.SendSynMsg(msgBaseSetPower);
                //if (0 == msgBaseSetPower.RtCode)
                //{
                //    Console.WriteLine("Power configuration successful.");
                //}
                //else { Console.WriteLine("Power configuration error."); }
                //Console.WriteLine("Enter any character to start reading the tag.");
                //Console.ReadKey();

                // 4 antenna read Inventory, EPC & TID
                MsgBaseInventoryGb msgBaseInventoryGb = new MsgBaseInventoryGb();
                msgBaseInventoryGb.AntennaEnable = (uint)(eAntennaNo._1 | eAntennaNo._2 | eAntennaNo._3 | eAntennaNo._4);
                msgBaseInventoryGb.InventoryMode = (byte)eInventoryMode.Inventory;
                msgBaseInventoryGb.ReadTid = new ParamEpcReadTid();                // tid Param
                msgBaseInventoryGb.ReadTid.Mode = (byte)eParamTidMode.Auto;
                msgBaseInventoryGb.ReadTid.Len = 6;
                clientConn.SendSynMsg(msgBaseInventoryGb);
                if (0 == msgBaseInventoryGb.RtCode)
                {
                    Console.WriteLine("Inventory GB tag successful.");
                }
                else { Console.WriteLine("Inventory GB tag error."); }
                Console.WriteLine("Reading....");
                Console.WriteLine("按任意键停止循环读卡(Enter any character to stop).");
                Console.ReadKey();

                // stop
                clientConn.SendSynMsg(msgBaseStop);
                if (0 == msgBaseStop.RtCode)
                {
                    Console.WriteLine("Stop successful.");
                }
                else { Console.WriteLine("Stop error."); }
            }
            else 
            {
                Console.WriteLine("Connect failure.");
            }
            Console.ReadKey();
        }

        #region API事件

        public static void OnEncapedTagGbLog(EncapedLogBaseGbInfo msg)
        {
            // Any blocking inside the callback will affect the normal use of the API !
            // 回调里面的任何阻塞或者效率过低，都会影响API的正常使用 !
            if (null != msg && 0 == msg.logBaseGbInfo.Result) 
            {
                Console.WriteLine(msg.reader + ":ant[" + msg.logBaseGbInfo.AntId + "]" + msg.logBaseGbInfo.Epc + "|" + msg.logBaseGbInfo.Tid);
            }
        }

        public static void OnEncapedTagGbOver(EncapedLogBaseGbOver msg)
        {
            if (null != msg)
            {
                Console.WriteLine("Epc log over.");
            }
        }

        #endregion
    }
}
