using GDotnet.Reader.Api.DAL;
using GDotnet.Reader.Api.Protocol.Gx;
using System;
using System.Collections.Generic;
using System.Text;


// ==============================================================
//   Copyright (C) 2019 SZGxwl Inc. All rights reserved.
// 
//   Create by xiao.liu at 2019/7/13 16:55:42.
//
//   xiao.liu [mailto:fanqie0127@gmail.com]
// ============================================================== 
namespace GDotnet.Reader.Api
{
    /// <summary>
    /// 
    /// Dual channel GPI trigger, in/out count
    /// 
    /// This sample code in and out of the count only for reference, count accuracy cannot be guaranteed!
    /// 
    /// </summary>
    static class ExampleGpiInOut
    {
        static void Main()
        {
            // The corresponding GPI port trigger parameters need to be configured
            GClient clientConn = new GClient();
            eConnectionAttemptEventStatusType status;
            if (clientConn.OpenSerial("COM16:115200", 3000, out status))
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

        private static int gpiInIndex = 0;              // in GPI index
        private static int gpiOutIndex = 1;             // out GPI index

        private static int inCount = 0;                 

        // Plan 1：Use the timer to determine the time of receiving GPI trigger information      
        // Plan 2：Device time using GPI trigger information (code abbreviated) (also see JAVA sample code)
        // Plan 1 code example 
        private static System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        private static int intervalTime = 1000;          // The interval between triggers
        private static int firstTrigger = -1;            // Trigger the GPI index first
        public static void OnEncapedGpiStart(EncapedLogBaseGpiStart msg)
        {
            if (null == msg) { return; }
            if (msg.logBaseGpiStart.GpiPort != gpiInIndex && msg.logBaseGpiStart.GpiPort != gpiOutIndex) { return; }
            if (firstTrigger == -1)
            {
                firstTrigger = msg.logBaseGpiStart.GpiPort;
                stopwatch.Reset();
                stopwatch.Start();
            }
            else
            {
                if (firstTrigger != msg.logBaseGpiStart.GpiPort)
                {
                    stopwatch.Stop();
                    TimeSpan timespan = stopwatch.Elapsed;
                    stopwatch.Reset();
                    stopwatch.Start();
                    if (timespan.TotalMilliseconds < intervalTime)
                    {
                        if (firstTrigger == gpiInIndex)
                        {
                            inCount++;
                            Console.WriteLine("=================== in[" + inCount + "] ===================");
                        }
                        else if (firstTrigger == gpiOutIndex)
                        {
                            outCount++;
                            Console.WriteLine("=================== out[" + outCount + "] ===================");
                        }
                        firstTrigger = -1;
                    }
                    else
                    {
                        firstTrigger = msg.logBaseGpiStart.GpiPort;
                    }
                }
                else
                {
                    stopwatch.Reset();
                    stopwatch.Start();
                }
            }
        }

        public static void OnEncapedGpiOver(EncapedLogBaseGpiOver msg)
        {
            
        }
    }
}
