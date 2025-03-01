package com.odinsoftware.lottolion.service;

import android.content.Context;
import android.content.SharedPreferences;
import android.preference.PreferenceManager;
import android.provider.Settings;
import android.telephony.TelephonyManager;

import java.util.UUID;

/**
 * Created by limyg on 2017-04-25.
 */
public class LLSharedPreferences {

    private final String CACHE_DEVICE_ID = "LOTTO_LION_UUID";
    private final String CACHE_CHECK_ID = "CACHE_CHECK_ID";
    private final String CACHE_CHECK_PW = "CACHE_CHECK_PW";
    private final String CACHE_SAVE_ID = "CACHE_SAVE_ID";
    private final String CACHE_SAVE_PW = "CACHE_SAVE_PW";
    private final String CACHE_SAVE_SESSION = "CACHE_SAVE_SESSION";
    private final String CACHE_SAVE_GUEST_SESSION = "CACHE_SAVE_GUEST_SESSION";
    private final String CACHE_SAVE_SEQUENCE = "CACHE_SAVE_SEQUENCE";


    private static Context context;
    private static LLSharedPreferences sigleton = null;

    public static LLSharedPreferences LL(Context p_context){
        if(sigleton == null){
            context = p_context;
            sigleton = new LLSharedPreferences();
        }
        return sigleton;
    }

    public boolean putCheckId(boolean check){
        SharedPreferences sharedPreferences = PreferenceManager.getDefaultSharedPreferences(context);
        return sharedPreferences.edit().putBoolean(CACHE_CHECK_ID, check).commit();
    }
    public boolean getCheckId(){
        SharedPreferences sharedPreferences = PreferenceManager.getDefaultSharedPreferences(context);
        return sharedPreferences.getBoolean(CACHE_CHECK_ID, false);
    }
    public boolean putCheckPw(boolean check){
        SharedPreferences sharedPreferences = PreferenceManager.getDefaultSharedPreferences(context);
        return sharedPreferences.edit().putBoolean(CACHE_CHECK_PW, check).commit();
    }
    public boolean getCheckPw(){
        SharedPreferences sharedPreferences = PreferenceManager.getDefaultSharedPreferences(context);
        return sharedPreferences.getBoolean(CACHE_CHECK_PW, false);
    }

    public boolean putSaveSession(String save){
        SharedPreferences sharedPreferences = PreferenceManager.getDefaultSharedPreferences(context);
        return sharedPreferences.edit().putString(CACHE_SAVE_SESSION, save).commit();
    }
    public String getSaveSession(){
        SharedPreferences sharedPreferences = PreferenceManager.getDefaultSharedPreferences(context);
        return sharedPreferences.getString(CACHE_SAVE_SESSION, "");
    }

    public boolean putSaveGuestSession(String save){
        SharedPreferences sharedPreferences = PreferenceManager.getDefaultSharedPreferences(context);
        return sharedPreferences.edit().putString(CACHE_SAVE_GUEST_SESSION, save).commit();
    }
    public String getSaveGuestSession(){
        SharedPreferences sharedPreferences = PreferenceManager.getDefaultSharedPreferences(context);
        return sharedPreferences.getString(CACHE_SAVE_GUEST_SESSION, "");
    }

    public boolean putSaveId(String save){
        SharedPreferences sharedPreferences = PreferenceManager.getDefaultSharedPreferences(context);
        return sharedPreferences.edit().putString(CACHE_SAVE_ID, save).commit();
    }
    public String getSaveId(){
        SharedPreferences sharedPreferences = PreferenceManager.getDefaultSharedPreferences(context);
        return sharedPreferences.getString(CACHE_SAVE_ID, "");
    }

    public boolean putSavePw(String save){
        SharedPreferences sharedPreferences = PreferenceManager.getDefaultSharedPreferences(context);
        return sharedPreferences.edit().putString(CACHE_SAVE_PW, save).commit();
    }
    public String getSavePw(){
        SharedPreferences sharedPreferences = PreferenceManager.getDefaultSharedPreferences(context);
        return sharedPreferences.getString(CACHE_SAVE_PW, "");
    }

    public boolean putSequence(int sequence){
        SharedPreferences sharedPreferences = PreferenceManager.getDefaultSharedPreferences(context);
        return sharedPreferences.edit().putInt(CACHE_SAVE_SEQUENCE, sequence).commit();
    }
    public int getSequence(){
        SharedPreferences sharedPreferences = PreferenceManager.getDefaultSharedPreferences(context);
        return sharedPreferences.getInt(CACHE_SAVE_SEQUENCE, 1);
    }

    public String GetDeviceUUID() {
        UUID deviceUUID = null;
        SharedPreferences sharedPreferences = PreferenceManager.getDefaultSharedPreferences(context);
        String cachedDeviceID = sharedPreferences.getString(CACHE_DEVICE_ID, "");
        if(cachedDeviceID != ""){
            deviceUUID = UUID.fromString(cachedDeviceID);
        }
        else{
            final String androidUniqueID = Settings.Secure.getString(context.getContentResolver(), Settings.Secure.ANDROID_ID);
            try{

                if(androidUniqueID != null){
                    deviceUUID = UUID.nameUUIDFromBytes(androidUniqueID.getBytes("utf8"));
                }
                else{
                    final String anotherUniqueID = ((TelephonyManager)context.getSystemService(context.TELEPHONY_SERVICE)).getDeviceId();
                    if(anotherUniqueID != null){
                        deviceUUID = UUID.nameUUIDFromBytes(anotherUniqueID.getBytes("utf8"));
                    }
                    else{
                        deviceUUID = UUID.randomUUID();
                    }
                }
            }
            catch(Exception ex){
                throw new RuntimeException(ex);
            }
        }
        sharedPreferences.edit().putString(CACHE_DEVICE_ID, deviceUUID.toString()).apply();
        return deviceUUID.toString();
    }
}
