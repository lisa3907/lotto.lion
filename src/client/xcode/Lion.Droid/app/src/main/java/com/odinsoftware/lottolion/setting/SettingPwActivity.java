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

public class SettingPwActivity extends AppCompatActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_setting_pw);

        InitSize();
        InitEvent();
    }
    private void InitEvent() {
        ImageView iv_back = (ImageView)findViewById(R.id.iv_back);

        iv_back.setOnTouchListener(listener_touch_back);
        iv_back.setOnClickListener(listener_back);

        Button btn_pw = (Button)findViewById(R.id.btn_pw);

        btn_pw.setOnClickListener(listener_click);
    }

    private void InitSize() {
        ImageView iv_back = (ImageView)findViewById(R.id.iv_back);
        ImageView iv_back1 = (ImageView)findViewById(R.id.iv_back1);
        ImageView iv_line1 = (ImageView)findViewById(R.id.iv_line1);
        ImageView iv_label_pw = (ImageView)findViewById(R.id.iv_label_pw);
        ImageView iv_label_checkPw = (ImageView)findViewById(R.id.iv_label_checkPw);

        iv_back.setLayoutParams(LLCommon.Get().ResizeDP(this,iv_back.getLayoutParams()));
        iv_back1.setLayoutParams(LLCommon.Get().ResizeDP(this,iv_back1.getLayoutParams()));
        iv_line1.setLayoutParams(LLCommon.Get().ResizeDP(this,iv_line1.getLayoutParams()));
        iv_label_pw.setLayoutParams(LLCommon.Get().ResizeDP(this,iv_label_pw.getLayoutParams()));
        iv_label_checkPw.setLayoutParams(LLCommon.Get().ResizeDP(this,iv_label_checkPw.getLayoutParams()));
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
            EditText et_pw = (EditText) findViewById(R.id.et_pw);
            EditText et_checkPw = (EditText) findViewById(R.id.et_checkPw);
            if(et_pw.getText().toString().length()<6){
                LLAlert.Get(SettingPwActivity.this).Alert("비밀번호는 6자리이상 가능합니다.");
                Loading(false);
            }
            else if (et_pw.getText().toString().equals(et_checkPw.getText().toString()) == false){
                LLAlert.Get(SettingPwActivity.this).Alert("비밀번호가 일치하지 않습니다.");
                Loading(false);
            }
            else{
                String token = LLSharedPreferences.LL(getApplicationContext()).getSaveSession();
                if(LLCommon.Get().CheckExpired(token) == false) {
                    ChangePassword(token, et_pw.getText().toString());
                }
                else{
                    finish();
                }
            }
        }
    };
    private void ChangePassword(String key, String pw){
        try{
            HashMap<String, String> parameter = new HashMap<>();
            parameter.put("password",pw);
            ApiAsnycTask api = new ApiAsnycTask(getString(R.string.api_url)+"user/ChangePassword",key);
            api.execute(parameter);
            api.finish = new ApiAsnycTask.Finish() {
                @Override
                public void Finish(String json) {
                    Gson g = new Gson();
                    ApiResult<String> result = g.fromJson(json, new TypeToken<ApiResult<String>>(){}.getType());
                    if(result != null){
                        if(result.success){
                            LLAlert.Get(SettingPwActivity.this).Alert("비밀번호가 변경되었습니다.", new LLAlert.DialogOkCancelListener() {
                                @Override
                                public void DialogOkCancelListener() {
                                    finish();
                                }
                            });
                        }
                        else{
                            LLAlert.Get(SettingPwActivity.this).Alert(result.message);
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
