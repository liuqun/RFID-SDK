using GDotnet.Reader.Api.DAL;
using GDotnet.Reader.Api.Protocol.Gx;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;


// ==============================================================
//   Copyright (C) 2019 SZGxwl Inc. All rights reserved.
// 
//   Create by xiao.liu at 2019/6/13 19:01:52.
//
//   xiao.liu [mailto:fanqie0127@gmail.com]
// ============================================================== 
namespace GDotnet.Reader.Api
{
    static class ExampleReadSingleTag
    {
        static void Main()
        {
            GClient clientConn = new GClient();
            eConnectionAttemptEventStatusType status;
            // clientConn.OpenTcp("192.168.1.168:8160", 3000, out status)
            if (clientConn.OpenSerial("COM16:115200", 3000, out status))
            {
                // subscribe to event
                clientConn.OnEncapedTagEpcLog += new delegateEncapedTagEpcLog(OnEncapedTagEpcLog);
                clientConn.OnEncapedTagEpcOver += new delegateEncapedTagEpcOver(OnEncapedTagEpcOver);


                Console.WriteLine("Enter any character to start reading the tag.");
                Console.ReadKey();
                Console.WriteLine("Reading....");
                // 2 antenna read Inventory, EPC & TID
                EncapedLogBaseEpcInfo tag = ReadSingleTag(clientConn, (uint)(eAntennaNo._1 | eAntennaNo._2), 5000);
                if (null != tag) 
                {
                    Console.WriteLine(tag.logBaseEpcInfo.ToString());
                }
                // do sth.

                Console.ReadKey();

            }
            else
            {
                Console.WriteLine("Connect failure.");
            }
            Console.ReadKey();
        }

        private static object waitReadSingle = new object();
        private static EncapedLogBaseEpcInfo waitTag = null;
        public static EncapedLogBaseEpcInfo ReadSingleTag(GClient clientConn, uint antEnable, int timeout) 
        {
            waitTag = null;
            MsgBaseStop msgBaseStop = new MsgBaseStop();
            clientConn.SendUnsynMsg(msgBaseStop);
            MsgBaseInventoryEpc msgBaseInventoryEpc = new MsgBaseInventoryEpc();
            msgBaseInventoryEpc.AntennaEnable = antEnable;
            msgBaseInventoryEpc.InventoryMode = (byte)eInventoryMode.Inventory;
            msgBaseInventoryEpc.ReadTid = new ParamEpcReadTid();                // tid参数
            msgBaseInventoryEpc.ReadTid.Mode = (byte)eParamTidMode.Auto;
            msgBaseInventoryEpc.ReadTid.Len = 6;
            clientConn.SendUnsynMsg(msgBaseInventoryEpc);
            try
            {
                lock (waitReadSingle) 
                {
                    if (null == waitTag) 
                    {
                        Monitor.Wait(waitReadSingle, timeout);
                    }
                }
            }
            catch { }
            msgBaseStop = new MsgBaseStop();
            clientConn.SendUnsynMsg(msgBaseStop);

            return waitTag;
        }

        #region API事件

        public static void OnEncapedTagEpcLog(EncapedLogBaseEpcInfo msg)
        {
            // Any blocking inside the callback will affect the normal use of the API !
            // 回调里面的任何阻塞或者效率过低，都会影响API的正常使用 !
            if (null != msg && 0 == msg.logBaseEpcInfo.Result)
            {
                waitTag = msg;
                try
                {
                    lock (waitReadSingle)
                    {
                        Monitor.Pulse(waitReadSingle);
                    }
                }
                catch { }
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
