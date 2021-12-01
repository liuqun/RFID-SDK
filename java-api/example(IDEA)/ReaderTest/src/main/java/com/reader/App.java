package com.reader;


import com.gg.reader.api.dal.GClient;
import com.gg.reader.api.dal.HandlerTagEpcLog;
import com.gg.reader.api.dal.HandlerTagEpcOver;
import com.gg.reader.api.protocol.gx.*;
import javax.usb.UsbConfiguration;
import javax.usb.UsbDevice;
import javax.usb.UsbDeviceDescriptor;
import javax.usb.UsbEndpoint;
import javax.usb.UsbException;
import javax.usb.UsbHostManager;
import javax.usb.UsbHub;
import javax.usb.UsbInterface;
import javax.usb.UsbInterfacePolicy;
import javax.usb.UsbPipe;
import javax.usb.UsbServices;
import java.io.UnsupportedEncodingException;
import java.util.Hashtable;
import java.util.List;
import java.util.Optional;
import java.util.Scanner;

public class App {
    public static Optional<UsbDevice> findUsbDeviceByID(int idVendor, int idProduct) {
        UsbServices srv;
        UsbHub hub;

        try {
            srv = UsbHostManager.getUsbServices();
            hub = srv.getRootUsbHub();
            for (Object it: hub.getAttachedUsbDevices()) {
                UsbDevice item = (UsbDevice) it;
                UsbDeviceDescriptor desc = item.getUsbDeviceDescriptor();
                if (desc.idVendor() == idVendor && desc.idProduct() == idProduct) {
                    System.out.println("调试信息: 已经找到RFID发卡器设备！");
                    return Optional.of(item);
                }
                System.out.printf("调试信息: USB厂商代码VID = 0x%04x\n", desc.idVendor());
                System.out.printf("调试信息: 产品类型代码PID = 0x%04x\n\n", desc.idProduct());
                //System.out.println(item.getProductString());
            }
        } catch (UsbException ex1) {
            String reason = "";
            System.out.println(reason);
        }
        return Optional.empty();
    }
    public static void main(String[] args) {
        Optional<UsbDevice> usbDevice =
                findUsbDeviceByID(0x03EB, 0x2421);
        if (!usbDevice.isPresent()) {
            System.out.println("请检查读卡器USB数据线，重新插拔一次");
        }
        GClient client = new GClient();
        Scanner sc = new Scanner(System.in);
        if (!usbDevice.isPresent() || !client.openUsbHid(usbDevice.get())) {
            System.out.println("Connect failure.");
        }
        else {
            // 订阅标签上报事件
            client.onTagEpcLog = new HandlerTagEpcLog() {
                @Override
                public void log(String readName, LogBaseEpcInfo logBaseEpcInfo) {
                    // 回调内部如有阻塞，会影响API正常使用
                    // 标签回调数量较多，请将标签数据先缓存起来再作业务处理
                    if (null != logBaseEpcInfo && 0 == logBaseEpcInfo.getResult()) {
                        System.out.println(logBaseEpcInfo);
                    }
                }
            };
            client.onTagEpcOver = new HandlerTagEpcOver() {
                @Override
                public void log(String readName, LogBaseEpcOver logBaseEpcOver) {
                    if (null != logBaseEpcOver) {
                        System.out.println("Epc log over.");
                    }
                }
            };
            // 停止指令，空闲态
            MsgBaseStop msgBaseStop = new MsgBaseStop();
            client.sendSynMsg(msgBaseStop);
            if (0 == msgBaseStop.getRtCode()) {
                System.out.println("Stop successful.");
            } else {
                System.out.println("Stop error.");
            }

            // 功率配置, 将所有天线功率都设置为0dBm.（注意:USB桌面发卡器只有一路天线）
            MsgBaseSetPower msgBaseSetPower = new MsgBaseSetPower();
            Hashtable<Integer, Integer> hashtable = new Hashtable<>();
            hashtable.put(1, 0);
            //hashtable.put(2, 0);
            //hashtable.put(3, 0);
            //hashtable.put(4, 0);
            msgBaseSetPower.setDicPower(hashtable);
            client.sendSynMsg(msgBaseSetPower);
            if (0 == msgBaseSetPower.getRtCode()) {
                System.out.println("Power configuration successful.");
            } else {
                System.out.println("Power configuration error.");
            }

            //按任意键开始读卡
            System.out.println("Enter any character to start reading the tag.");
            sc.nextLine();

            // 4个天线读卡, 读取EPC数据区以及TID数据区
            MsgBaseInventoryEpc msgBaseInventoryEpc = new MsgBaseInventoryEpc();
            msgBaseInventoryEpc.setAntennaEnable(EnumG.AntennaNo_1);//msgBaseInventoryEpc.setAntennaEnable(EnumG.AntennaNo_1 | EnumG.AntennaNo_2 | EnumG.AntennaNo_3 | EnumG.AntennaNo_4);
            msgBaseInventoryEpc.setInventoryMode(EnumG.InventoryMode_Inventory);

            ParamEpcReadTid tid = new ParamEpcReadTid();
            tid.setMode(EnumG.ParamTidMode_Auto);
            tid.setLen(6);
            msgBaseInventoryEpc.setReadTid(tid);

            client.sendSynMsg(msgBaseInventoryEpc);
            if (0 == msgBaseInventoryEpc.getRtCode()) {
                System.out.println("Inventory epc successful.");
            } else {
                System.out.println("Inventory epc error.");
            }

            //按任意键停止读卡(停止前请拿开标签)
            sc.nextLine();
            // 停止读卡，空闲态
            client.sendSynMsg(msgBaseStop);
            if (0 == msgBaseStop.getRtCode()) {
                System.out.println("Stop successful.");
            } else {
                System.out.println("Stop error.");
            }

			System.out.println("Close the connection");
            client.close();

        }
    }

}
