using GDotnet.Reader.Api.DAL;
using GDotnet.Reader.Api.DAL.Communication;
using GDotnet.Reader.Api.Protocol.Gx;
using System;
using System.Collections.Generic;
using System.Text;


// ==============================================================
//   Copyright (C) 2020 SZGxwl Inc. All rights reserved.
// 
//   Create by xiao.liu at 2020/2/17 12:03:22.
//
//   xiao.liu [mailto:fanqie0127@gmail.com]
// ============================================================== 
namespace GDotnet.Reader.Api
{
    class Example_USB_HID
    {

        static void Main() 
        {
            List<string> devList = new List<string>();
            devList = GClient.GetUsbHidList();
            if (null != devList && devList.Count > 0) 
            {
                foreach (var item in devList)
                {
                    Console.WriteLine(item);
                }
                GClient clientConn = new GClient();
                eConnectionAttemptEventStatusType status;
                if (clientConn.OpenUsbHid(devList[0], IntPtr.Zero, 3000, out status))
                {
                    // subscribe to event
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
                }
            }
            Console.ReadKey();
        }

        #region API事件

        public static void OnEncapedTagEpcLog(EncapedLogBaseEpcInfo msg)
        {
            // Any blocking inside the callback will affect the normal use of the API !
            // 回调里面的任何阻塞或者效率过低，都会影响API的正常使用 !
            if (null != msg && 0 == msg.logBaseEpcInfo.Result)
            {
                Console.WriteLine(":ant[" + msg.logBaseEpcInfo.AntId + "]" + msg.logBaseEpcInfo.Epc + "|" + msg.logBaseEpcInfo.Tid);
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
