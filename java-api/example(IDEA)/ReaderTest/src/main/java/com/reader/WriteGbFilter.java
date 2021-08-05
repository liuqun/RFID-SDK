package com.reader;

import com.gg.reader.api.dal.GClient;
import com.gg.reader.api.protocol.gx.EnumG;
import com.gg.reader.api.protocol.gx.MsgBaseWriteGb;
import com.gg.reader.api.protocol.gx.ParamEpcFilter;

import java.util.Scanner;

public class WriteGbFilter {
    public static void main(String[] args) {
        GClient client = new GClient();
        Scanner scanner = new Scanner(System.in);
        if (client.openSerial(ConnectedConstant.Serial_RS232, 2000)) {

            System.out.println("Enter any character to start write.");
            scanner.nextLine();

            MsgBaseWriteGb msg = new MsgBaseWriteGb();
            msg.setAntennaEnable(EnumG.AntennaNo_1 | EnumG.AntennaNo_2);
            msg.setStart(0);
            msg.setArea(0x10);
            String data = "AAAA";
            int len = PcUtils.getValueLen(data);
            msg.setHexWriteData(PcUtils.padLeft(data, len * 4, '0'));

            //匹配写
            String tid = "E280110520007B05A8C208A8";
            ParamEpcFilter filter = new ParamEpcFilter();
            filter.setArea(0x10);
            filter.setHexData(tid);
            filter.setBitStart(0);
            filter.setBitLength(tid.length() * 4);
            msg.setFilter(filter);

            client.sendSynMsg(msg);
            if (0x00 == msg.getRtCode()) {
                System.out.println("Write successful.");
            } else {
                System.out.println(msg.getRtMsg());
            }
        }
    }
}
