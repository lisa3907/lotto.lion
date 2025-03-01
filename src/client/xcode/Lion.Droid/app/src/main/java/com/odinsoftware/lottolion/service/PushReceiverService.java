package com.odinsoftware.lottolion.service;

import android.app.Notification;
import android.app.NotificationManager;
import android.graphics.Color;
import android.os.Build;

import com.google.firebase.messaging.FirebaseMessagingService;
import com.google.firebase.messaging.RemoteMessage;
import com.odinsoftware.lottolion.R;

import java.util.Map;

/**
 * Created by limyg on 2017-04-27.
 */
public class PushReceiverService extends FirebaseMessagingService {
    @Override
    public void onMessageReceived(RemoteMessage remoteMessage) {
        Map<String, String> data = remoteMessage.getData();
        Notification.Builder notifyBuilder = new Notification.Builder(this)
                .setContentTitle(data.get("title"))
                .setContentText(data.get("message"))
                .setSmallIcon(R.drawable.alarm)
                .setWhen(System.currentTimeMillis());

        NotificationManager notificationManager =
                (NotificationManager) getSystemService(this.NOTIFICATION_SERVICE);
        notificationManager.notify(0, notifyBuilder.build());

        LLCommon.Get().ResetBadge(this);
    }
}
