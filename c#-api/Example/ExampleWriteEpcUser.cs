using GDotnet.Reader.Api.DAL;
using GDotnet.Reader.Api.Protocol.Gx;
using GDotnet.Reader.Api.Utils;
using System;
using System.Collections.Generic;
using System.Text;


// ==============================================================
//   Copyright (C) 2019 SZGxwl Inc. All rights reserved.
// 
//   Create by xiao.liu at 2019/4/13 16:02:54.
//
//   xiao.liu [mailto:fanqie0127@gmail.com]
// ============================================================== 
namespace GDotnet.Reader.Api
{
    public class ExampleWriteEpcUser
    {
        static void Main()
        {
            GClient clientConn = new GClient();
            eConnectionAttemptEventStatusType status;
            // clientConn.OpenTcp("192.168.1.168:6180", 3000, out status)
            if (clientConn.OpenSerial("COM16:115200", 3000, out status))
            {
                // stop
                MsgBaseStop msgBaseStop = new MsgBaseStop();
                clientConn.SendSynMsg(msgBaseStop);
                if (0 == msgBaseStop.RtCode)
                {
                    Console.WriteLine("Stop successful.");
                }
                else { Console.WriteLine("Stop error."); }
                // write Userdata， hex 4321
                String sWriteHexData = "4321";                                     
                Console.WriteLine("Write hex " + sWriteHexData);
                Console.WriteLine("Enter any character to start wirte.");
                Console.ReadKey();
                MsgBaseWriteEpc msgBaseWriteEpc = new MsgBaseWriteEpc();
                msgBaseWriteEpc.AntennaEnable = (ushort)eAntennaNo._1;
                msgBaseWriteEpc.Area = (byte)eEpcWriteArea.Userdata;
                msgBaseWriteEpc.Start = 0;
                msgBaseWriteEpc.HexWriteData = sWriteHexData;
                // /* If you need to write data with a special encoding, encode it yourself and use the "BwriteData" property */
                // msgBaseWriteEpc.BwriteData = Util.ConvertHexStringToByteArray(msgBaseWriteEpc.HexWriteData);

                // /* Matching the TID write tag example for writing a single tag in a multi-tag environment */
                //String filterTid = "E2801160200061CB22BC0916";
                //msgBaseWriteEpc.Filter = new ParamEpcFilter();
                //msgBaseWriteEpc.Filter.Area = (byte)eParamFilterArea.TID;
                //msgBaseWriteEpc.Filter.BitStart = 0;
                //msgBaseWriteEpc.Filter.HexData = filterTid;
                //msgBaseWriteEpc.Filter.BData = Util.ConvertHexStringToByteArray(msgBaseWriteEpc.Filter.HexData);
                //msgBaseWriteEpc.Filter.BitLength = (byte)(msgBaseWriteEpc.Filter.BData.Length * 8);

                ///* match epc */
                //String filterEpc = "4321";
                //Console.WriteLine("Filter hex " + filterEpc);
                //msgBaseWriteEpc.Filter = new ParamEpcFilter();
                //msgBaseWriteEpc.Filter.Area = (byte)eParamFilterArea.EPC;
                //msgBaseWriteEpc.Filter.BitStart = 32;
                //msgBaseWriteEpc.Filter.HexData = filterEpc;
                //// msgBaseWriteEpc.Filter.BData = Util.ConvertHexStringToByteArray(msgBaseWriteEpc.Filter.HexData);
                //msgBaseWriteEpc.Filter.BitLength = (byte)(msgBaseWriteEpc.Filter.BData.Length * 8);

                clientConn.SendSynMsg(msgBaseWriteEpc);
                if (0 == msgBaseWriteEpc.RtCode)
                {
                    Console.WriteLine("Write successful.");
                }
                else
                {
                    Console.WriteLine(msgBaseWriteEpc.RtMsg);
                }
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("Connect failure.");
            }
            Console.ReadKey();
        }
    }
}
