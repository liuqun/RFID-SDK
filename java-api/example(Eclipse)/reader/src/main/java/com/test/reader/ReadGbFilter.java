package com.test.reader;

import com.gg.reader.api.dal.*;
import com.gg.reader.api.protocol.gx.*;

import java.util.Scanner;

public class ReadGbFilter {
    public static void main(String[] args) {
        GClient client = new GClient();
        Scanner scanner = new Scanner(System.in);
        if (client.openSerial(ConnectedConstant.Serial_RS232, 2000)) {
            subscribeHandler(client);

            System.out.println("Press any key to start reading the card!");
            scanner.nextLine();

            MsgBaseInventoryGb msg = new MsgBaseInventoryGb();
            msg.setAntennaEnable(EnumG.AntennaNo_1 | EnumG.AntennaNo_2);
            msg.setInventoryMode(EnumG.InventoryMode_Inventory);

            //匹配TID读 E280110520007B05A8C208A8  可选参数
//            ParamEpcFilter filter = new ParamEpcFilter();
//            String tid = "E280110520007B05A8C208A8";
//            filter.setArea(EnumG.ParamFilterArea_TID);
//            filter.setBitStart(0);
//            filter.setHexData(tid);
//            filter.setBitLength(tid.length() * 4);
//            msg.setFilter(filter);

            //读TID 默认只读EPC 可选参数
            ParamEpcReadTid readTid = new ParamEpcReadTid();
            readTid.setMode(EnumG.ParamTidMode_Auto);
            readTid.setLen(6);//word
            msg.setReadTid(readTid);

            //读UserData 可选参数
            ParamGbReadUserdata readUserdata = new ParamGbReadUserdata();
            readUserdata.setChildArea(0x30);
            readUserdata.setStart(4);
            readUserdata.setLen(6);//word
            msg.setReadUserdata(readUserdata);

            client.sendSynMsg(msg);
            if (0x00 == msg.getRtCode()) {
                System.out.println("MsgBaseInventoryGb[OK].");
            } else {
                System.out.println(msg.getRtMsg());
            }

            System.out.println("Press any key to Stop reading card");
            scanner.nextLine();

            MsgBaseStop stopMsg = new MsgBaseStop();
            client.sendSynMsg(stopMsg);
            if (0x00 == stopMsg.getRtCode()) {
                System.out.println("MsgBaseStop Success");
            } else {
                System.out.println("MsgBaseStop Fail");
            }

            System.out.println("Close the connection");
            client.close();

        }
    }

    //订阅GB标签信息上报
    private static void subscribeHandler(GClient client) {
        client.onTagGbLog = new HandlerTagGbLog() {
            @Override
            public void log(String s, LogBaseGbInfo logBaseGbInfo) {
                if (null != logBaseGbInfo && logBaseGbInfo.getResult() == 0) {
                    System.out.println(logBaseGbInfo);
                }
            }
        };

        client.onTagGbOver = new HandlerTagGbOver() {
            @Override
            public void log(String s, LogBaseGbOver logBaseGbOver) {
                System.out.println("HandlerTagGbOver");
            }
        };

    }
}
