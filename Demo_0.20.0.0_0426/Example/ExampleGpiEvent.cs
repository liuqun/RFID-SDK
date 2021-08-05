using GDotnet.Reader.Api.DAL;
using GDotnet.Reader.Api.Protocol.Gx;
using System;
using System.Collections.Generic;
using System.Text;


// ==============================================================
//   Copyright (C) 2019 SZGxwl Inc. All rights reserved.
// 
//   Create by xiao.liu at 2019/5/10 13:57:51.
//
//   xiao.liu [mailto:fanqie0127@gmail.com]
// ============================================================== 
namespace GDotnet.Reader.Api
{
    /// <summary>
    /// GPI trigger
    /// </summary>
    static class ExampleGpiEvent
    {
        static void Main()
        {
            // The corresponding GPI port trigger parameters need to be configured 
            // and the GPI escalation message can only be received after the trigger condition is reached
            GClient clientConn = new GClient();
            eConnectionAttemptEventStatusType status;
            if (clientConn.OpenTcp("192.168.1.168:8160", 3000, out status))
            {
                Console.WriteLine("Connect successful.");
                clientConn.OnEncapedGpiStart += new delegateEncapedGpiStart(OnEncapedGpiStart);
                clientConn.OnEncapedGpiOver += new delegateEncapedGpiOver(OnEncapedGpiOver);
                
            }
            else 
            {
                Console.WriteLine("Connect failure.");
            }

            Console.ReadKey();
        }

        public static void OnEncapedGpiStart(EncapedLogBaseGpiStart msg)
        {
            // Any blocking inside the callback will affect the normal use of the API !
            // 回调里面的任何阻塞或者效率过低，都会影响API的正常使用 !
            Console.WriteLine("[" + msg.reader + "][Gpi " + msg.logBaseGpiStart.GpiPort + " start][" + (msg.logBaseGpiStart.Level == 0 ? "low" : "high")
                     + "][" + msg.logBaseGpiStart.TriggerTime.ToString("yyyy-MM-dd HH:mm:ss.fff") + "]");
        }

        public static void OnEncapedGpiOver(EncapedLogBaseGpiOver msg)
        {
            // Any blocking inside the callback will affect the normal use of the API !
            // 回调里面的任何阻塞或者效率过低，都会影响API的正常使用 !
            Console.WriteLine("[" + msg.reader + "][Gpi " + msg.logBaseGpiOver.GpiPort + " over][" + (msg.logBaseGpiOver.Level == 0 ? "low" : "high")
                    + "][" + msg.logBaseGpiOver.TriggerTime.ToString("yyyy-MM-dd HH:mm:ss.fff") + "]");
        }

    }
}
