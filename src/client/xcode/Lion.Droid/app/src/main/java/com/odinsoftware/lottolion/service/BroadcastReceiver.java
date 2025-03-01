package com.odinsoftware.lottolion.service;

import android.content.Context;
import android.content.Intent;
import android.os.Bundle;
import android.os.Vibrator;

/**
 * Created by limyg on 2017-04-27.
 */
public class BroadcastReceiver extends android.content.BroadcastReceiver {
    @Override
    public void onReceive(Context context, Intent intent) {
        if (intent.getAction().equals(Intent.ACTION_BOOT_COMPLETED)) {

            Intent i = new Intent(context, MobileService.class);
            context.startService(i);
        }
        if (intent.getAction().equals("com.google.android.c2dm.intent.RECEIVE")) {
            Bundle data = intent.getExtras();
            Intent alarm = new Intent("android.intent.action.BADGE_COUNT_UPDATE");
// 패키지 네임과 클래스 네임 설정
            alarm.putExtra("badge_count_package_name", "com.odinsoftware.lottolion");
            alarm.putExtra("badge_count_class_name", "com.odinsoftware.lottolion.LoginActivity");
// 업데이트 카운트
            alarm.putExtra("badge_count", data.getString("badge"));
            context.sendBroadcast(alarm);
            Vibrator vibrator = (Vibrator) context.getSystemService(Context.VIBRATOR_SERVICE);// (Context.VIBRATE_SERVICE)
            long millisecond = 500;
            vibrator.vibrate(millisecond);


        }
    }
}