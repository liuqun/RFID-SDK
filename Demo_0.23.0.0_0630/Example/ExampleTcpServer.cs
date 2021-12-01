using GDotnet.Reader.Api.DAL;
using GDotnet.Reader.Api.Protocol.Gx;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

// ==============================================================
//   Copyright (C) 2020 SZGxwl Inc. All rights reserved.
// 
//   Create by xiao.liu at 2020-07-17 15:47:31.
//
//   xiao.liu [mailto:fanqie0127@gmail.com]
// ============================================================== 
namespace GDotnet.Reader.Api
{
    static class ExampleTcpServer
    {
        private static GClient clientConn = null;
        private static object waitClient = new object();
        static void Main()
        {
            GServer server = new GServer();
            server.OnGClientConnected += new delegateGClientConnected(OnGClientConnected);
            server.Open(8160);
            Console.WriteLine("请将设备设置成客户端模式!(Please set the device to client mode!)");
            Console.WriteLine("accepted 8160, waiting for client...");
            lock (waitClient) 
            {
                Monitor.Wait(waitClient);
            }

            clientConn.OnEncapedTagEpcLog += new delegateEncapedTagEpcLog(OnEncapedTagEpcLog);
            clientConn.OnEncapedTagEpcOver += new delegateEncapedTagEpcOver(OnEncapedTagEpcOver);

            // stop 
            MsgBaseStop msgBaseStop = new MsgBaseStop();
            clientConn.SendSynMsg(msgBaseStop);
            if (0 == msgBaseStop.RtCode)
            {
                Console.WriteLine("Stop successful.");
            }
            else { Console.WriteLine("Stop error."); }

            // 4 antenna read Inventory, EPC & TID
            MsgBaseInventoryEpc msgBaseInventoryEpc = new MsgBaseInventoryEpc();
            msgBaseInventoryEpc.AntennaEnable = (uint)(eAntennaNo._1 | eAntennaNo._2 | eAntennaNo._3 | eAntennaNo._4);
            msgBaseInventoryEpc.InventoryMode = (byte)eInventoryMode.Inventory;
            msgBaseInventoryEpc.ReadTid = new ParamEpcReadTid();                // tid Param
            msgBaseInventoryEpc.ReadTid.Mode = (byte)eParamTidMode.Auto;
            msgBaseInventoryEpc.ReadTid.Len = 6;
            clientConn.SendSynMsg(msgBaseInventoryEpc);
            if (0 == msgBaseInventoryEpc.RtCode)
            {
                Console.WriteLine("Inventory epc successful.");
            }
            else { Console.WriteLine("Inventory epc error."); }
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
            Console.WriteLine("按任意键关闭(Enter any character to close).");
            Console.ReadKey();
            clientConn.Close();
            server.Close();
        }


        #region API事件

        public static void OnGClientConnected(GClient client) 
        {
            if (clientConn != null) 
            {
                clientConn.Close();
            }
            clientConn = client;
            Console.WriteLine("发现新设备连接(find device).");
            lock (waitClient)
            {
                Monitor.Pulse(waitClient);
            }
        }

        public static void OnEncapedTagEpcLog(EncapedLogBaseEpcInfo msg)
        {
            // Any blocking inside the callback will affect the normal use of the API !
            // 回调里面的任何阻塞或者效率过低，都会影响API的正常使用 !
            if (null != msg && 0 == msg.logBaseEpcInfo.Result)
            {
                Console.WriteLine(msg.serialNo + ":ant[" + msg.logBaseEpcInfo.AntId + "]" + msg.logBaseEpcInfo.Epc + "|" + msg.logBaseEpcInfo.Tid);
            }
        }

        public static void OnEncapedTagEpcOver(EncapedLogBaseEpcOver msg)
        {
            if (null != msg)
            {
                Console.WriteLine("Epc log over.");
            }
        }

        #endregion

    }
}
