package com.odinsoftware.lottolion.setting;

import android.graphics.PorterDuff;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.MotionEvent;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ImageView;
import android.widget.ProgressBar;

import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;
import com.odinsoftware.lottolion.R;
import com.odinsoftware.lottolion.service.ApiAsnycTask;
import com.odinsoftware.lottolion.service.LLAlert;
import com.odinsoftware.lottolion.service.LLCommon;
import com.odinsoftware.lottolion.service.LLSharedPreferences;
import com.odinsoftware.lottolion.type.ApiResult;

import java.util.HashMap;

public class SettingEmailActivity extends AppCompatActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_setting_email);

        InitSize();
        InitEvent();
    }

    private void InitEvent() {
        ImageView iv_back = (ImageView)findViewById(R.id.iv_back);

        iv_back.setOnTouchListener(listener_touch_back);
        iv_back.setOnClickListener(listener_back);

        Button btn_email = (Button)findViewById(R.id.btn_email);
        Button btn_certify = (Button)findViewById(R.id.btn_certify);

        btn_email.setOnClickListener(listener_click);
        btn_certify.setOnClickListener(listener_click);
    }

    private void InitSize() {
        ImageView iv_back = (ImageView)findViewById(R.id.iv_back);
        ImageView iv_back1 = (ImageView)findViewById(R.id.iv_back1);
        ImageView iv_line1 = (ImageView)findViewById(R.id.iv_line1);
        ImageView iv_label_email = (ImageView)findViewById(R.id.iv_label_email);
        ImageView iv_label_certify = (ImageView)findViewById(R.id.iv_label_certify);

        iv_back.setLayoutParams(LLCommon.Get().ResizeDP(this,iv_back.getLayoutParams()));
        iv_back1.setLayoutParams(LLCommon.Get().ResizeDP(this,iv_back1.getLayoutParams()));
        iv_line1.setLayoutParams(LLCommon.Get().ResizeDP(this,iv_line1.getLayoutParams()));
        iv_label_email.setLayoutParams(LLCommon.Get().ResizeDP(this,iv_label_email.getLayoutParams()));
        iv_label_certify.setLayoutParams(LLCommon.Get().ResizeDP(this,iv_label_certify.getLayoutParams()));
    }
    View.OnClickListener listener_back = new View.OnClickListener() {
        @Override
        public void onClick(View v) {
            finish();
        }
    };
    View.OnTouchListener listener_touch_back = new View.OnTouchListener() {
        @Override
        public boolean onTouch(View v, MotionEvent event) {
            // TODO Auto-generated method stub
            ImageView iv_back1 = (ImageView)findViewById(R.id.iv_back1);
            if(event.getAction() == MotionEvent.ACTION_DOWN){
                iv_back1.setColorFilter(0xaa111111, PorterDuff.Mode.SRC_OVER);
            }
            else if(event.getAction() == MotionEvent.ACTION_UP){
                iv_back1.setColorFilter(0x00000000, PorterDuff.Mode.SRC_OVER);
            }

            return false;
        }
    };
    View.OnClickListener listener_click = new View.OnClickListener() {
        @Override
        public void onClick(View v) {
            Loading(true);
            EditText et_email = (EditText) findViewById(R.id.et_email);
            EditText et_certify = (EditText) findViewById(R.id.et_certify);
            switch (v.getId()) {
                case R.id.btn_email:
                    SendMailToCheckMailAddress(LLCommon.Get().GetGuestToken(getApplicationContext()), et_email.getText().toString());
                    break;
                case R.id.btn_certify:
                    CheckMailAddress(LLCommon.Get().GetGuestToken(getApplicationContext()), et_email.getText().toString(), et_certify.getText().toString());
                    break;
            }
        }
    };
    private void SendMailToCheckMailAddress(String key, String email){
        try{
            HashMap<String, String> parameter = new HashMap<>();
            parameter.put("mail_address",String.valueOf(email));
            ApiAsnycTask api = new ApiAsnycTask(getString(R.string.api_url)+"user/SendMailToCheckMailAddress",key);
            api.execute(parameter);
            api.finish = new ApiAsnycTask.Finish() {
                @Override
                public void Finish(String json) {
                    Gson g = new Gson();
                    ApiResult<String> result = g.fromJson(json, new TypeToken<ApiResult<String>>(){}.getType());
                    if(result != null){
                        if(result.success){
                            EditText et_email = (EditText)findViewById(R.id.et_email);
                            LLAlert.Get(SettingEmailActivity.this).Alert(et_email.getText().toString()+"\n인증코드가 발송되었습니다.");
                        }
                        else{
                            LLAlert.Get(SettingEmailActivity.this).Alert(result.message);
                        }
                    }
                    Loading(false);
                }
            };
        }
        catch (Exception ex){

        }
    }
    private void CheckMailAddress(String key, String email, String check){
        try{
            HashMap<String, String> parameter = new HashMap<>();
            parameter.put("mail_address",String.valueOf(email));
            parameter.put("check_number",String.valueOf(check));
            ApiAsnycTask api = new ApiAsnycTask(getString(R.string.api_url)+"user/CheckMailAddress",key);
            api.execute(parameter);
            api.finish = new ApiAsnycTask.Finish() {
                @Override
                public void Finish(String json) {
                    Gson g = new Gson();
                    ApiResult<String> result = g.fromJson(json, new TypeToken<ApiResult<String>>(){}.getType());
                    if(result != null){
                        if(result.success){
                            String token = LLSharedPreferences.LL(getApplicationContext()).getSaveSession();
                            if(LLCommon.Get().CheckExpired(token) == false){
                                EditText et_email = (EditText) findViewById(R.id.et_email);
                                EditText et_certify = (EditText) findViewById(R.id.et_certify);
                                UpdateMailAddress(token,et_email.getText().toString(), et_certify.getText().toString());
                            }
                            else{
                                finish();
                            }
                        }
                        else{
                            LLAlert.Get(SettingEmailActivity.this).Alert(result.message);
                        }
                    }
                    Loading(false);
                }
            };
        }
        catch (Exception ex){

        }
    }
    private void UpdateMailAddress(String key, String email, String check){
        try{
            HashMap<String, String> parameter = new HashMap<>();
            parameter.put("mail_address",email);
            parameter.put("check_number",check);
            ApiAsnycTask api = new ApiAsnycTask(getString(R.string.api_url)+"user/UpdateMailAddress",key);
            api.execute(parameter);
            api.finish = new ApiAsnycTask.Finish() {
                @Override
                public void Finish(String json) {
                    Gson g = new Gson();
                    ApiResult<String> result = g.fromJson(json, new TypeToken<ApiResult<String>>(){}.getType());
                    if(result != null){
                        if(result.success){
                            EditText et_email = (EditText)findViewById(R.id.et_email);
                            LLAlert.Get(SettingEmailActivity.this).Alert("메일이 변경되었습니다.", new LLAlert.DialogOkCancelListener() {
                                @Override
                                public void DialogOkCancelListener() {
                                    finish();
                                }
                            });
                        }
                        else{
                            LLAlert.Get(SettingEmailActivity.this).Alert(result.message);
                        }
                    }
                    Loading(false);
                }
            };
        }
        catch (Exception ex){

        }
    }
    private void Loading(boolean view){
        Button btn_loading = (Button)findViewById(R.id.btn_loading);
        ProgressBar pb_loading = (ProgressBar)findViewById(R.id.pb_loading);

        if(view){
            btn_loading.setVisibility(View.VISIBLE);
            pb_loading.setVisibility(View.VISIBLE);
        }
        else{
            btn_loading.setVisibility(View.INVISIBLE);
            pb_loading.setVisibility(View.INVISIBLE);
        }
    }
}
