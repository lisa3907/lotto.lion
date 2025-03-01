package com.odinsoftware.lottolion.service;

import com.google.firebase.iid.FirebaseInstanceId;
import com.google.firebase.iid.FirebaseInstanceIdService;
import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;
import com.odinsoftware.lottolion.R;
import com.odinsoftware.lottolion.type.ApiResult;

import java.util.HashMap;

/**
 * Created by limyg on 2017-04-27.
 */
public class RefreshTokenService extends FirebaseInstanceIdService {
    @Override
    public void onTokenRefresh() {
        String refreshedId = FirebaseInstanceId.getInstance().getToken();
        String token = LLSharedPreferences.LL(getApplicationContext()).getSaveSession();
        if(LLCommon.Get().CheckExpired(token) == false){
            UpdateDeviceId(token, refreshedId);
        }
    }
    private void UpdateDeviceId(String key, String id){
        try{
            HashMap<String, String> parameter = new HashMap<>();
            parameter.put("device_type","A");
            parameter.put("device_id",id);
            ApiAsnycTask api = new ApiAsnycTask(getString(R.string.api_url)+"user/UpdateDeviceId",key);
            api.execute(parameter);
        }
        catch (Exception ex){

        }
    }
}
