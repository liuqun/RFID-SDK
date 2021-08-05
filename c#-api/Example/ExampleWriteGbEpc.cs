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
    public class ExampleWriteGbEpc
    {
        static void Main()
        {
            GClient clientConn = new GClient();
            eConnectionAttemptEventStatusType status;
            // clientConn.OpenTcp("192.168.1.168:6180", 3000, out status)
            if (clientConn.OpenSerial("COM6:115200", 3000, out status))
            {
                // stop
                MsgBaseStop msgBaseStop = new MsgBaseStop();
                clientConn.SendSynMsg(msgBaseStop);
                if (0 == msgBaseStop.RtCode)
                {
                    Console.WriteLine("Stop successful.");
                }
                else { Console.WriteLine("Stop error."); }
                // write EPC， hex 1234
                String sWriteHexData = "4321";                                     
                Console.WriteLine("Write hex " + sWriteHexData);
                Console.WriteLine("Enter any character to start wirte.");
                Console.ReadKey();
                MsgBaseWriteGb msgBaseWriteGb = new MsgBaseWriteGb();
                msgBaseWriteGb.AntennaEnable = (ushort)eAntennaNo._1;
                msgBaseWriteGb.Area = (byte)eGbWriteArea.Epc;
                msgBaseWriteGb.Start = 0;                                          
                int iWordLen = sWriteHexData.Length / 4;                            // 1 word = 2 byte     
                ushort iPc = (ushort)(iWordLen << 8);                              
                String sPc = Convert.ToString(iPc, 16).PadLeft(4, '0');
                sWriteHexData = sPc + sWriteHexData;                                // 
                msgBaseWriteGb.HexWriteData = sWriteHexData.Trim().PadRight(iWordLen * 4, '0');
                // /* If you need to write data with a special encoding, encode it yourself and use the "BwriteData" property */
                // msgBaseWriteEpc.BwriteData = Util.ConvertHexStringToByteArray(msgBaseWriteEpc.HexWriteData);

                // /* Matching the TID write tag example for writing a single tag in a multi-tag environment */
                //String filterTid = "E2801160200061CB22BC0916";
                //msgBaseWriteGb.Filter = new ParamEpcFilter();
                //msgBaseWriteGb.Filter.Area = (byte)eParamGbFilterArea.TID;
                //msgBaseWriteGb.Filter.BitStart = 0;
                //msgBaseWriteGb.Filter.HexData = filterTid;
                //msgBaseWriteGb.Filter.BData = Util.ConvertHexStringToByteArray(msgBaseWriteGb.Filter.HexData);
                //msgBaseWriteGb.Filter.BitLength = (byte)(msgBaseWriteGb.Filter.BData.Length * 8);

                ///* match epc */
                //String filterEpc = "4321";
                //Console.WriteLine("Filter hex " + filterEpc);
                //msgBaseWriteGb.Filter = new ParamEpcFilter();
                //msgBaseWriteGb.Filter.Area = (byte)eParamGbFilterArea.EPC;
                //msgBaseWriteGb.Filter.BitStart = 32;
                //msgBaseWriteGb.Filter.HexData = filterEpc;
                //// msgBaseWriteGb.Filter.BData = Util.ConvertHexStringToByteArray(msgBaseWriteGb.Filter.HexData);
                //msgBaseWriteGb.Filter.BitLength = (byte)(msgBaseWriteGb.Filter.BData.Length * 8);

                clientConn.SendSynMsg(msgBaseWriteGb);
                if (0 == msgBaseWriteGb.RtCode)
                {
                    Console.WriteLine("Write successful.");
                }
                else
                {
                    Console.WriteLine(msgBaseWriteGb.RtMsg);
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
