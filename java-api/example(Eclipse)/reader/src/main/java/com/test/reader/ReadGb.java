package com.test.reader;

import com.gg.reader.api.dal.*;
import com.gg.reader.api.protocol.gx.*;

import java.util.Scanner;

public class ReadGb {
    public static void main(String[] args) {
        GClient client = new GClient();
        Scanner scanner = new Scanner(System.in);
        if (client.openSerial(ConnectedConstant.Serial_RS232, 2000)) {
            subscribeHandler(client);

            System.out.println("Press any key to start reading the card!");
            scanner.nextLine();

            MsgBaseInventoryGb msgBaseInventoryGb = new MsgBaseInventoryGb();
            msgBaseInventoryGb.setAntennaEnable(EnumG.AntennaNo_1 | EnumG.AntennaNo_2);//1号天线与2号天线读
            msgBaseInventoryGb.setInventoryMode(EnumG.InventoryMode_Inventory);
            client.sendSynMsg(msgBaseInventoryGb);
            if (0x00 == msgBaseInventoryGb.getRtCode()) {
                System.out.println("MsgBaseInventoryGb[OK].");
            } else {
                System.out.println(msgBaseInventoryGb.getRtMsg());
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
