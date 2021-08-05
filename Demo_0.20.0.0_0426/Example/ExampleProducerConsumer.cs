using GDotnet.Reader.Api.DAL;
using GDotnet.Reader.Api.Protocol.Gx;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;


// ==============================================================
//   Copyright (C) 2019 SZGxwl Inc. All rights reserved.
// 
//   Create by xiao.liu at 2019/6/17 19:13:53.
//
//   xiao.liu [mailto:fanqie0127@gmail.com]
// ============================================================== 
namespace GDotnet.Reader.Api.Example
{
    static class ExampleProducerConsumer
    {
        static void Main()
        {
            GClient clientConn = new GClient();
            eConnectionAttemptEventStatusType status;
            // clientConn.OpenTcp("192.168.1.168:8160", 3000, out status)
            if (clientConn.OpenSerial("COM16:115200", 3000, out status))
            {
                // subscribe 
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

                Console.WriteLine("Enter any character to start reading the tag.");
                Console.ReadKey();
                RunConsumer();
                // 4 antenna read Inventory, EPC & TID
                MsgBaseInventoryEpc msgBaseInventoryEpc = new MsgBaseInventoryEpc();
                msgBaseInventoryEpc.AntennaEnable = (uint)(eAntennaNo._1);
                msgBaseInventoryEpc.InventoryMode = (byte)eInventoryMode.Inventory;
                msgBaseInventoryEpc.ReadTid = new ParamEpcReadTid();                // tid parameter
                msgBaseInventoryEpc.ReadTid.Mode = (byte)eParamTidMode.Auto;
                msgBaseInventoryEpc.ReadTid.Len = 6;
                clientConn.SendSynMsg(msgBaseInventoryEpc);
                if (0 == msgBaseInventoryEpc.RtCode)
                {
                    Console.WriteLine("Inventory epc successful.");
                }
                else { Console.WriteLine("Inventory epc error."); }
                Console.WriteLine("Reading....");
                Console.WriteLine("Enter any character to stop.");
                Console.ReadKey();


                // stop
                clientConn.SendSynMsg(msgBaseStop);
                if (0 == msgBaseStop.RtCode)
                {
                    Console.WriteLine("Stop successful.");
                }
                else { Console.WriteLine("Stop error."); }
                DisposeConsumer();
                Console.ReadKey();

            }
            else
            {
                Console.WriteLine("Connect failure.");
            }
            Console.ReadKey();
        }

        private static object waitRead = new object();
        private static Queue<EncapedLogBaseEpcInfo> buffTag = new Queue<EncapedLogBaseEpcInfo>();
        private static int buffMax = 100000;

        private static Boolean keepConsumer = false;
        private static void RunConsumer() 
        {
            if (keepConsumer) { return; }
            keepConsumer = true;
            long printIndex = 0;
            ThreadPool.QueueUserWorkItem(new WaitCallback(delegate(Object o){
                while (keepConsumer)
                {
                    try
                    {
                        EncapedLogBaseEpcInfo tag = null;
                        lock (waitRead)
                        {
                            if (buffTag.Count <= 0)
                            {
                                Monitor.Wait(waitRead);
                            }
                            else 
                            {
                                tag = buffTag.Dequeue();
                            }
                        }
                        if (null != tag)
                        {
                            Console.WriteLine(printIndex + " -- " + tag.logBaseEpcInfo.ToString());
                            printIndex++;
                        }
                    }
                    catch { }
                }
            }));
        }

        private static void DisposeConsumer() 
        {
            keepConsumer = false;
            try
            {
                lock (waitRead)
                {
                    Monitor.Pulse(waitRead);
                }
            }
            catch { }
        }

        #region API事件

        public static void OnEncapedTagEpcLog(EncapedLogBaseEpcInfo msg)
        {
            // Any blocking inside the callback will affect the normal use of the API !
            // 回调里面的任何阻塞或者效率过低，都会影响API的正常使用 !
            if (null != msg && 0 == msg.logBaseEpcInfo.Result)
            {
                try
                {
                    lock (waitRead)
                    {
                        if (buffTag.Count < buffMax)
                        {
                            buffTag.Enqueue(msg);
                        }
                        Monitor.Pulse(waitRead);
                    }
                }
                catch { }
            }
            else 
            {
                Console.WriteLine("-1");
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
