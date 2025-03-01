package com.odinsoftware.lottolion.type;

import java.io.Serializable;

/**
 * Created by limyg on 2017-04-27.
 */
public class PushMessage implements Serializable {
    public String notifyTime;
    public String message;
    public boolean isRead;
}
