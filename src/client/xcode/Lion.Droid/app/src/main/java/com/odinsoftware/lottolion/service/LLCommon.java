package com.odinsoftware.lottolion.service;

import android.content.Context;
import android.content.Intent;
import android.util.DisplayMetrics;
import android.view.ViewGroup;
import android.view.WindowManager;

import com.auth0.android.jwt.JWT;
import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;
import com.odinsoftware.lottolion.type.ApiResult;

import java.text.DateFormat;
import java.text.DecimalFormat;
import java.text.SimpleDateFormat;
import java.util.Calendar;

/**
 * Created by limyg on 2017-04-24.
 */
public class LLCommon {
    private static LLCommon sigleton = null;
    public static LLCommon Get(){
        if(sigleton == null){
            sigleton = new LLCommon();
        }
        return sigleton;
    }

    public String GetDate(Calendar p_date){
        DateFormat sdFormat = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss");
        return sdFormat.format(p_date.getTime());
    }
    public Calendar GetDate(String p_date){
        Calendar cal = Calendar.getInstance();
        try{
            DateFormat sdFormat = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss");
            cal.setTime(sdFormat.parse(p_date));
        }
        catch (Exception ex){

        }
        return cal;
    }
    public boolean CheckExpired(String token){
        boolean result = true;
        try{
            JWT jwt = new JWT(token);
            result = jwt.getExpiresAt().getTime() < Calendar.getInstance().getTimeInMillis();
        }
        catch (Exception ex){

        }
        return result;
    }
    public String GetStringComma(String str) {
        if (str.length() == 0)
            return "";
        double value = Double.parseDouble(str);
        DecimalFormat format = new DecimalFormat("###,###");
        return format.format(value);
    }
    public ViewGroup.LayoutParams ResizeDP(Context context, ViewGroup.LayoutParams layout){
        DisplayMetrics metrics = new DisplayMetrics();
        ((WindowManager)context.getSystemService(context.WINDOW_SERVICE)).getDefaultDisplay().getMetrics(metrics);

        layout.width = (int)((layout.width * Math.round(metrics.density))/metrics.density);
        layout.height = (int)((layout.height * Math.round(metrics.density))/metrics.density);

        if(isTablet(context)){
            layout.width = (int)(layout.width * metrics.widthPixels/720);
            layout.height = (int)(layout.height * metrics.heightPixels/1280);
        }

        return layout;
    }
    public boolean isTablet(Context context) {
//        boolean xlarge = ((context.getResources().getConfiguration().screenLayout & Configuration.SCREENLAYOUT_SIZE_MASK) == Configuration.SCREENLAYOUT_SIZE_XLARGE);
//        boolean large = ((context.getResources().getConfiguration().screenLayout & Configuration.SCREENLAYOUT_SIZE_MASK) == Configuration.SCREENLAYOUT_SIZE_LARGE);
//        return (xlarge || large);
        boolean tablet = false;
        DisplayMetrics metrics = new DisplayMetrics();
        ((WindowManager)context.getSystemService(context.WINDOW_SERVICE)).getDefaultDisplay().getMetrics(metrics);

        float yInches= metrics.heightPixels/metrics.ydpi;
        float xInches= metrics.widthPixels/metrics.xdpi;
        double diagonalInches = Math.sqrt(xInches*xInches + yInches*yInches);
        if (diagonalInches>=6.5){
            tablet = true;
            // 6.5inch device or bigger
        }else{
            tablet = false;
            // smaller device
        }

        return tablet;
    }
    public String GetGuestToken(Context context){
        String token=LLSharedPreferences.LL(context).getSaveGuestSession();
        if(CheckExpired(token)){
            ApiResult<String> newToken = GetGuestToken();
            if(newToken.success) {
                token = newToken.result;
                LLSharedPreferences.LL(context).putSaveGuestSession(token);
            }
            else{
                LLAlert.Get(context).Alert(newToken.message);
            }
        }

        return token;
    }
    public String doDayOfWeek(int dow) {
        String result = "";
        switch (dow){
            case 1:
                result = "일";
                break;
            case 2:
                result = "월";
                break;
            case 3:
                result = "화";
                break;
            case 4:
                result = "수";
                break;
            case 5:
                result = "목";
                break;
            case 6:
                result = "금";
                break;
            case 7:
                result = "토";
                break;
        }
        return result;
    };

    public void ResetBadge(Context context){
        Intent intent = new Intent("android.intent.action.BADGE_COUNT_UPDATE");
// 패키지 네임과 클래스 네임 설정
        intent.putExtra("badge_count_package_name", "com.odinsoftware.lottolion");
        intent.putExtra("badge_count_class_name", "com.odinsoftware.lottolion.MainActivity");
// 업데이트 카운트
        intent.putExtra("badge_count", 0); context.sendBroadcast(intent);
    }

    private ApiResult<String> GetGuestToken(){
        ApiResult<String> result = null;
        try{
            ApiAsnycTask api = new ApiAsnycTask("https://lottoapi.odinsoftware.co.kr/api/user/getguesttoken","");
            String json = api.execute().get();
            Gson g = new Gson();
            result = g.fromJson(json, new TypeToken<ApiResult<String>>(){}.getType());
        }
        catch (Exception ex){

        }
        return result;
    }
}
