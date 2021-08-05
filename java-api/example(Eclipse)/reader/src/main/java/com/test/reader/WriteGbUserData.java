package com.test.reader;

import com.gg.reader.api.dal.GClient;
import com.gg.reader.api.protocol.gx.EnumG;
import com.gg.reader.api.protocol.gx.MsgBaseWriteGb;

import java.util.Scanner;

public class WriteGbUserData {
    public static void main(String[] args) {
        GClient client = new GClient();
        Scanner scanner = new Scanner(System.in);
        if (client.openSerial(ConnectedConstant.Serial_RS232, 2000)) {
            System.out.println("Enter any character to start write.");
            scanner.nextLine();

            MsgBaseWriteGb msg = new MsgBaseWriteGb();
            msg.setAntennaEnable(EnumG.AntennaNo_1);
            //0x10:标签编码区 | 0x20:标签安全区 | 0x30~0x3F:用户子区0~15
            msg.setArea(0x30);
            msg.setStart(4);//word
            String data = "AAAA";
            int len = PcUtils.getValueLen(data);
            String s = PcUtils.padLeft(data, len * 4, '0');
            msg.setHexWriteData(s);
            client.sendSynMsg(msg);
            if (0x00 == msg.getRtCode()) {
                System.out.println("Write successful.");
            } else {
                System.out.println(msg.getRtMsg());
            }
        }
    }
}
