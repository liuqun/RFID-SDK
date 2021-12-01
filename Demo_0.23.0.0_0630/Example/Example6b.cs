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
    static class Example6b
    {
        static void Main()
        {
            GClient clientConn = new GClient();
            eConnectionAttemptEventStatusType status;
            // clientConn.OpenSerial("COM18:115200", 3000, out status)
            if (clientConn.OpenTcp("192.168.1.168:8160", 3000, out status))
            {
                // subscribe to event
                clientConn.OnEncapedTag6bLog += new delegateEncapedTag6bLog(OnEncapedTag6bLog);
                clientConn.OnEncapedTag6bOver += new delegateEncapedTag6bOver(OnEncapedTag6bOver);

                // stop 
                MsgBaseStop msgBaseStop = new MsgBaseStop();
                clientConn.SendSynMsg(msgBaseStop);
                if (0 == msgBaseStop.RtCode)
                {
                    Console.WriteLine("Stop successful.");
                }
                else { Console.WriteLine("Stop error."); }

                // 4 antenna read Inventory, TID & Userdata
                MsgBaseInventory6b msgBaseInventory6b = new MsgBaseInventory6b();
                msgBaseInventory6b.AntennaEnable = (uint)(eAntennaNo._1 | eAntennaNo._2 | eAntennaNo._3 | eAntennaNo._4);
                msgBaseInventory6b.InventoryMode = (byte)eInventoryMode.Inventory;
                msgBaseInventory6b.Area = (byte)e6bReadMode.TidAndUserdata;                // tid Param

                msgBaseInventory6b.ReadUserdata = new Param6bReadUserdata();
                msgBaseInventory6b.ReadUserdata.Start = 0;
                msgBaseInventory6b.ReadUserdata.Len = 4;

                clientConn.SendSynMsg(msgBaseInventory6b);
                if (0 == msgBaseInventory6b.RtCode)
                {
                    Console.WriteLine("Inventory 6b successful.");
                }
                else { Console.WriteLine("Inventory 6b error."); }
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

        public static void OnEncapedTag6bLog(EncapedLogBase6bInfo msg)
        {
            // Any blocking inside the callback will affect the normal use of the API !
            // 回调里面的任何阻塞或者效率过低，都会影响API的正常使用 !
            if (null != msg && 0 == msg.logBase6bInfo.Result) 
            {
                Console.WriteLine(msg.reader + ":ant[" + msg.logBase6bInfo.AntId + "]" + msg.logBase6bInfo.Tid + "|" + msg.logBase6bInfo.Userdata);
            }
        }

        public static void OnEncapedTag6bOver(EncapedLogBase6bOver msg)
        {
            if (null != msg)
            {
                Console.WriteLine("6b log over.");
            }
        }

        #endregion
    }
}
