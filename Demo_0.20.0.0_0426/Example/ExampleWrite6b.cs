using GDotnet.Reader.Api.DAL;
using GDotnet.Reader.Api.Protocol.Gx;
using GDotnet.Reader.Api.Utils;
using System;
using System.Collections.Generic;
using System.Text;


// ==============================================================
//   Copyright (C) 2019 SZGxwl Inc. All rights reserved.
// 
//   Create by xiao.liu at 2019/10/16 8:43:26.
//
//   xiao.liu [mailto:fanqie0127@gmail.com]
// ============================================================== 
namespace GDotnet.Reader.Api
{
    public class ExampleWrite6b
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
                // write EPC， hex 1234
                String sWriteHexData = "4321";
                Console.WriteLine("Write hex " + sWriteHexData);
                Console.WriteLine("Enter any character to start wirte.");
                Console.ReadKey();
                MsgBaseWrite6b msgBaseWrite6b = new MsgBaseWrite6b();
                msgBaseWrite6b.AntennaEnable = (ushort)eAntennaNo._1;
                msgBaseWrite6b.HexMatchTid = "E0040000F8B3E808";
                // msgBaseWrite6b.BMatchTid = Util.ConvertHexStringToByteArray(msgBaseWrite6b.HexMatchTid);
                msgBaseWrite6b.Start = 8;
                msgBaseWrite6b.HexWriteData = sWriteHexData;
                // msgBaseWrite6b.BMatchTid = Util.ConvertHexStringToByteArray(msgBaseWrite6b.HexMatchTid);

                clientConn.SendSynMsg(msgBaseWrite6b);
                if (0 == msgBaseWrite6b.RtCode)
                {
                    Console.WriteLine("Write successful.");
                }
                else
                {
                    Console.WriteLine(msgBaseWrite6b.RtMsg);
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
