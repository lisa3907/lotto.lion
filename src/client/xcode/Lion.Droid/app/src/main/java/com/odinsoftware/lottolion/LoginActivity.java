package com.odinsoftware.lottolion;

import android.content.Intent;
import android.graphics.Color;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.MotionEvent;
import android.view.View;
import android.widget.Button;
import android.widget.CheckBox;
import android.widget.CompoundButton;
import android.widget.EditText;
import android.widget.ImageView;
import android.widget.ProgressBar;
import android.widget.TextView;

import com.google.firebase.iid.FirebaseInstanceId;
import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;
import com.odinsoftware.lottolion.service.ApiAsnycTask;
import com.odinsoftware.lottolion.service.LLAlert;
import com.odinsoftware.lottolion.service.LLCommon;
import com.odinsoftware.lottolion.service.LLSharedPreferences;
import com.odinsoftware.lottolion.type.ApiResult;

import java.util.HashMap;

public class LoginActivity extends AppCompatActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_login);

        InitData();
        InitEvent();
        InitSize();
    }

    private void InitSize() {
        ImageView iv_line1 = (ImageView)findViewById(R.id.iv_line1);
        ImageView iv_line2 = (ImageView)findViewById(R.id.iv_line2);

        iv_line1.setLayoutParams(LLCommon.Get().ResizeDP(this,iv_line1.getLayoutParams()));
        iv_line2.setLayoutParams(LLCommon.Get().ResizeDP(this,iv_line2.getLayoutParams()));
    }

    private void InitEvent() {

        Button btn_login = (Button)findViewById(R.id.btn_login);
        TextView tv_join = (TextView)findViewById(R.id.tv_join);
        TextView tv_findPw = (TextView)findViewById(R.id.tv_findPw);
        CheckBox cb_id = (CheckBox)findViewById(R.id.cb_id);
        CheckBox cb_pw = (CheckBox)findViewById(R.id.cb_pw);

        btn_login.setOnClickListener(listener_click);
        tv_join.setOnClickListener(listener_click);
        tv_findPw.setOnClickListener(listener_click);
        tv_join.setOnTouchListener(listener_touch_click);
        tv_findPw.setOnTouchListener(listener_touch_click);

        cb_id.setOnCheckedChangeListener(listener_check);
        cb_pw.setOnCheckedChangeListener(listener_check);
    }

    private void InitData() {

        EditText et_id = (EditText)findViewById(R.id.et_id);
        EditText et_pw = (EditText)findViewById(R.id.et_pw);
        CheckBox cb_id = (CheckBox)findViewById(R.id.cb_id);
        CheckBox cb_pw = (CheckBox)findViewById(R.id.cb_pw);

        cb_id.setChecked(LLSharedPreferences.LL(getApplicationContext()).getCheckId());
        cb_pw.setChecked(LLSharedPreferences.LL(getApplicationContext()).getCheckPw());

        if(cb_id.isChecked())
            et_id.setText(LLSharedPreferences.LL(getApplicationContext()).getSaveId());
        if(cb_pw.isChecked())
            et_pw.setText(LLSharedPreferences.LL(getApplicationContext()).getSavePw());
    }
    View.OnClickListener listener_click = new View.OnClickListener() {
        @Override
        public void onClick(View v) {
            Intent intent = null;
            switch (v.getId()){
                case R.id.btn_login:
                    Loading(true);
                    EditText et_id = (EditText)findViewById(R.id.et_id);
                    EditText et_pw = (EditText)findViewById(R.id.et_pw);
                    CheckBox cb_id = (CheckBox)findViewById(R.id.cb_id);
                    CheckBox cb_pw = (CheckBox)findViewById(R.id.cb_pw);
                    if(cb_id.isChecked())
                        LLSharedPreferences.LL(getApplicationContext()).putSaveId(et_id.getText().toString());
                    if(cb_pw.isChecked())
                        LLSharedPreferences.LL(getApplicationContext()).putSavePw(et_pw.getText().toString());
                    GetTokenByLoginId(LLCommon.Get().GetGuestToken(getApplicationContext()),et_id.getText().toString(),et_pw.getText().toString());
                    break;
                case R.id.tv_join:
                    intent = new Intent(LoginActivity.this, JoinActivity.class);
                    startActivity(intent);
                    break;
                case R.id.tv_findPw:
                    intent = new Intent(LoginActivity.this, FindPwActivity.class);
                    startActivity(intent);
                    break;
            }
        }
    };
    private void GetTokenByLoginId(String key, String id, String pw){
        try{
            HashMap<String, String> parameter = new HashMap<>();
            parameter.put("login_id",id);
            parameter.put("password",pw);
            parameter.put("device_type","A");
            parameter.put("device_id", FirebaseInstanceId.getInstance().getToken());
            ApiAsnycTask api = new ApiAsnycTask(getString(R.string.api_url)+"user/GetTokenByLoginId",key);
            api.execute(parameter);
            api.finish = new ApiAsnycTask.Finish() {
                @Override
                public void Finish(String json) {
                    Gson g = new Gson();
                    ApiResult<String> result = g.fromJson(json, new TypeToken<ApiResult<String>>(){}.getType());
                    if(result != null){
                        if(result.success){
                            LLSharedPreferences.LL(getApplicationContext()).putSaveSession(result.result);
                            finish();
                        }
                        else{
                            LLAlert.Get(LoginActivity.this).Alert(result.message);
                        }
                    }
                    Loading(false);
                }
            };
        }
        catch (Exception ex){

        }
    }
    View.OnTouchListener listener_touch_click = new View.OnTouchListener() {
        @Override
        public boolean onTouch(View v, MotionEvent event) {
            // TODO Auto-generated method stub
            TextView view = (TextView)v;
            if(event.getAction() == MotionEvent.ACTION_DOWN){
                view.setTextColor(Color.parseColor("#A0000000"));
            }
            else if(event.getAction() == MotionEvent.ACTION_UP){
                view.setTextColor(Color.parseColor("#000000"));
            }

            return false;
        }
    };
    CompoundButton.OnCheckedChangeListener listener_check = new CompoundButton.OnCheckedChangeListener() {
        @Override
        public void onCheckedChanged(CompoundButton buttonView, boolean isChecked) {
            switch (buttonView.getId()){
                case R.id.cb_id:
                    LLSharedPreferences.LL(getApplicationContext()).putCheckId(isChecked);
                    break;
                case R.id.cb_pw:
                    LLSharedPreferences.LL(getApplicationContext()).putCheckPw(isChecked);
                    break;
            }
        }
    };
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
