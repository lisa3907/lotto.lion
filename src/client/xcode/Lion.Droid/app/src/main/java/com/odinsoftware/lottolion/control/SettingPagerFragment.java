package com.odinsoftware.lottolion.control;

import android.app.Activity;
import android.content.Intent;
import android.graphics.PorterDuff;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.view.LayoutInflater;
import android.view.MotionEvent;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.ProgressBar;
import android.widget.TextView;

import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;
import com.odinsoftware.lottolion.R;
import com.odinsoftware.lottolion.popup.NumberPopupActivity;
import com.odinsoftware.lottolion.service.ApiAsnycTask;
import com.odinsoftware.lottolion.service.LLAlert;
import com.odinsoftware.lottolion.service.LLCommon;
import com.odinsoftware.lottolion.service.LLSharedPreferences;
import com.odinsoftware.lottolion.setting.SettingEmailActivity;
import com.odinsoftware.lottolion.setting.SettingPushActivity;
import com.odinsoftware.lottolion.setting.SettingPwActivity;
import com.odinsoftware.lottolion.type.ApiResult;
import com.odinsoftware.lottolion.type.UserInfo;

import java.util.ArrayList;
import java.util.Collections;
import java.util.HashMap;

/**
 * Created by limyg on 2017-04-26.
 */
public class SettingPagerFragment extends Fragment {

    private final int MAX = 1;
    private final int FIX = 2;
    View rootView;
    UserInfo userInfo;

    public interface Refresh{
        void onRefresh();
    }
    public Refresh refresh;
    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        rootView = inflater.inflate(R.layout.fragment_setting, container, false);
        InitSize(rootView);
        InitEvent(rootView);
        InitData();

        return rootView;
    }

    private void InitData() {
        String token = LLSharedPreferences.LL(getContext()).getSaveSession();
        if(LLCommon.Get().CheckExpired(token) == false) {
            GetUserInfo(token);
            GetCount(token);
        }
    }

    public void Init(){
        InitData();
    }

    private void InitEvent(View view) {
        ImageView iv_label_email = (ImageView)view.findViewById(R.id.iv_label_email);
        ImageView iv_label_max = (ImageView)view.findViewById(R.id.iv_label_max);
        ImageView iv_label_fix = (ImageView)view.findViewById(R.id.iv_label_fix);
        ImageView iv_label_pw = (ImageView)view.findViewById(R.id.iv_label_pw);
        ImageView iv_label_push = (ImageView)view.findViewById(R.id.iv_label_push);
        Button btn_logout = (Button)view.findViewById(R.id.btn_logout);
        Button btn_leave = (Button)view.findViewById(R.id.btn_leave);

        iv_label_email.setOnTouchListener(listener_touch_iv);
        iv_label_email.setOnClickListener(listener_click_iv);
        iv_label_max.setOnTouchListener(listener_touch_iv);
        iv_label_max.setOnClickListener(listener_click_iv);
        iv_label_fix.setOnTouchListener(listener_touch_iv);
        iv_label_fix.setOnClickListener(listener_click_iv);
        iv_label_pw.setOnTouchListener(listener_touch_iv);
        iv_label_pw.setOnClickListener(listener_click_iv);
        iv_label_push.setOnTouchListener(listener_touch_iv);
        iv_label_push.setOnClickListener(listener_click_iv);

        btn_logout.setOnTouchListener(listener_touch_btn);
        btn_logout.setOnClickListener(listener_click_btn);
        btn_leave.setOnTouchListener(listener_touch_btn);
        btn_leave.setOnClickListener(listener_click_btn);
    }

    private void InitSize(View view) {
        ImageView iv_label_email = (ImageView)view.findViewById(R.id.iv_label_email);
        ImageView iv_label_max = (ImageView)view.findViewById(R.id.iv_label_max);
        ImageView iv_label_fix = (ImageView)view.findViewById(R.id.iv_label_fix);
        ImageView iv_label_pw = (ImageView)view.findViewById(R.id.iv_label_pw);
        ImageView iv_label_push = (ImageView)view.findViewById(R.id.iv_label_push);
        Button btn_logout = (Button)view.findViewById(R.id.btn_logout);
        Button btn_leave = (Button)view.findViewById(R.id.btn_leave);

        iv_label_email.setLayoutParams(LLCommon.Get().ResizeDP(getContext(),iv_label_email.getLayoutParams()));
        iv_label_max.setLayoutParams(LLCommon.Get().ResizeDP(getContext(),iv_label_max.getLayoutParams()));
        iv_label_fix.setLayoutParams(LLCommon.Get().ResizeDP(getContext(),iv_label_fix.getLayoutParams()));
        iv_label_pw.setLayoutParams(LLCommon.Get().ResizeDP(getContext(),iv_label_pw.getLayoutParams()));
        iv_label_push.setLayoutParams(LLCommon.Get().ResizeDP(getContext(),iv_label_push.getLayoutParams()));

        btn_logout.setLayoutParams(LLCommon.Get().ResizeDP(getContext(),btn_logout.getLayoutParams()));
        btn_leave.setLayoutParams(LLCommon.Get().ResizeDP(getContext(),btn_leave.getLayoutParams()));
    }

    View.OnClickListener listener_click_iv = new View.OnClickListener() {
        @Override
        public void onClick(View v) {
            Intent intent = null;
            switch (v.getId()){
                case R.id.iv_label_email:
                    intent = new Intent(getContext(), SettingEmailActivity.class);
                    startActivity(intent);
                    break;
                case R.id.iv_label_max:
                    intent = new Intent(getContext(), NumberPopupActivity.class);
                    intent.putExtra("min",5);
                    intent.putExtra("max",100);
                    intent.putExtra("reverse",false);
                    startActivityForResult(intent,MAX);
                    break;
                case R.id.iv_label_fix:
                    intent = new Intent(getContext(), NumberPopupActivity.class);
                    intent.putExtra("min",0);
                    intent.putExtra("max",45);
                    intent.putExtra("reverse",false);
                    startActivityForResult(intent,FIX);
                    break;
                case R.id.iv_label_pw:
                    intent = new Intent(getContext(), SettingPwActivity.class);
                    startActivity(intent);
                    break;
                case R.id.iv_label_push:
                    intent = new Intent(getContext(), SettingPushActivity.class);
                    startActivity(intent);
                    break;
            }
        }
    };
    View.OnTouchListener listener_touch_iv = new View.OnTouchListener() {
        @Override
        public boolean onTouch(View v, MotionEvent event) {
            // TODO Auto-generated method stub
            ImageView view = (ImageView)v;
            if(event.getAction() == MotionEvent.ACTION_DOWN){
                view.getBackground().setColorFilter(0x30000000, PorterDuff.Mode.SRC_OVER);
            }
            else if(event.getAction() == MotionEvent.ACTION_UP){
                view.getBackground().setColorFilter(0x00000000, PorterDuff.Mode.SRC_OVER);
            }
            else if(event.getAction() == MotionEvent.ACTION_CANCEL){
                view.getBackground().setColorFilter(0x00000000, PorterDuff.Mode.SRC_OVER);
                view.invalidate();
            }
            return false;
        }
    };
    View.OnClickListener listener_click_btn = new View.OnClickListener() {
        @Override
        public void onClick(View v) {
            switch (v.getId()){
                case R.id.btn_logout:
                    LLSharedPreferences.LL(getContext()).putSaveSession("");
                    if(refresh!=null)
                        refresh.onRefresh();
                    break;
                case R.id.btn_leave:
                    LLAlert.Get(getContext()).CancelAlert("모든 정보가 삭제됩니다.\n정말로 탈퇴하시겠습니까?", new LLAlert.DialogOkCancelListener() {
                        @Override
                        public void DialogOkCancelListener() {
                            String token = LLSharedPreferences.LL(getContext()).getSaveSession();
                            if(LLCommon.Get().CheckExpired(token) == false) {
                                LeaveMember(token);
                            }
                        }
                    });
                    break;
            }
        }
    };
    View.OnTouchListener listener_touch_btn =  new View.OnTouchListener() {
        @Override
        public boolean onTouch(View v, MotionEvent event) {
            Button view = (Button)v;
            if(event.getAction() == MotionEvent.ACTION_DOWN){
                view.getBackground().setColorFilter(0x50000000, PorterDuff.Mode.SRC_OVER);
                view.invalidate();
            }
            else if(event.getAction() == MotionEvent.ACTION_UP){
                view.getBackground().setColorFilter(0x00000000, PorterDuff.Mode.SRC_OVER);
                view.invalidate();
            }
            else if(event.getAction() == MotionEvent.ACTION_CANCEL){
                view.getBackground().setColorFilter(0x00000000, PorterDuff.Mode.SRC_OVER);
                view.invalidate();
            }
            return false;
        }
    };
    private void GetUserInfo(String key){
        try{
            Loading(true);
            ApiAsnycTask api = new ApiAsnycTask(getString(R.string.api_url)+"user/GetUserInfor",key);
            api.execute();
            api.finish = new ApiAsnycTask.Finish() {
                @Override
                public void Finish(String json) {
                    Gson g = new Gson();
                    ApiResult<UserInfo> result = g.fromJson(json, new TypeToken<ApiResult<UserInfo>>(){}.getType());
                    if(result != null){
                        if(result.success){
                            userInfo = result.result;
                            TextView tv_email = (TextView)rootView.findViewById(R.id.tv_email);
                            TextView tv_max = (TextView)rootView.findViewById(R.id.tv_max);
                            TextView tv_fix = (TextView)rootView.findViewById(R.id.tv_fix);
                            tv_email.setText(userInfo.emailAddress);
                            tv_max.setText(userInfo.maxSelectNumber);
                            ArrayList<Integer> list_digit = new ArrayList<>();
                            list_digit.add(Integer.parseInt(userInfo.digit1));
                            list_digit.add(Integer.parseInt(userInfo.digit2));
                            list_digit.add(Integer.parseInt(userInfo.digit3));
                            Collections.sort(list_digit);
                            String fix="-";
                            for(Integer digit : list_digit){
                                if(digit !=0){
                                  if(fix.equals("-"))
                                      fix =digit.toString();
                                  else
                                      fix +=", "+digit.toString();
                                }
                            }
                            tv_fix.setText(fix);
                        }
                        else{
                            LLAlert.Get(getContext()).Alert(result.message);
                        }
                    }
                    Loading(false);
                }
            };
        }
        catch (Exception ex){

        }
    }
    private void UpdateUserInfo(String key, String name, String max, String digit1, String digit2, String digit3){
        try{
            Loading(true);
            HashMap<String, String> parameter = new HashMap<>();
            parameter.put("login_name",name);
            parameter.put("max_select_number",max);
            parameter.put("digit1", digit1);
            parameter.put("digit2", digit2);
            parameter.put("digit3", digit3);
            ApiAsnycTask api = new ApiAsnycTask(getString(R.string.api_url)+"user/UpdateUserInfor",key);
            api.execute(parameter);
            api.finish = new ApiAsnycTask.Finish() {
                @Override
                public void Finish(String json) {
                    Gson g = new Gson();
                    ApiResult<String> result = g.fromJson(json, new TypeToken<ApiResult<String>>(){}.getType());
                    if(result != null){
                        if(result.success){
                            LLAlert.Get(getContext()).Alert("변경되었습니다.", new LLAlert.DialogOkCancelListener() {
                                @Override
                                public void DialogOkCancelListener() {
                                    Init();
                                }
                            });
                        }
                        else{
                            LLAlert.Get(getContext()).Alert(result.message);
                        }
                    }
                    Loading(false);
                }
            };
        }
        catch (Exception ex){

        }
    }
    private void GetCount(String key){
        try{
            ApiAsnycTask api = new ApiAsnycTask(getString(R.string.api_url)+"notify/GetCount",key);
            api.execute();
            api.finish = new ApiAsnycTask.Finish() {
                @Override
                public void Finish(String json) {
                    Gson g = new Gson();
                    ApiResult<String> result = g.fromJson(json, new TypeToken<ApiResult<String>>(){}.getType());
                    if(result != null){
                        if(result.success){
                            TextView tv_push = (TextView)rootView.findViewById(R.id.tv_push);
                            tv_push.setText(result.result);
                        }
                        else{
                            LLAlert.Get(getContext()).Alert(result.message);
                        }
                    }
                }
            };
        }
        catch (Exception ex){

        }
    }
    private void LeaveMember(String key){
        try{
            Loading(true);
            ApiAsnycTask api = new ApiAsnycTask(getString(R.string.api_url)+"user/LeaveMember",key);
            api.execute();
            api.finish = new ApiAsnycTask.Finish() {
                @Override
                public void Finish(String json) {
                    Gson g = new Gson();
                    ApiResult<String> result = g.fromJson(json, new TypeToken<ApiResult<String>>(){}.getType());
                    if(result != null){
                        if(result.success){
                            LLSharedPreferences.LL(getContext()).putSaveSession("");
                            if(refresh!=null)
                                refresh.onRefresh();
                        }
                        else{
                            LLAlert.Get(getContext()).Alert(result.message);
                        }
                    }
                    Loading(false);
                }
            };
        }
        catch (Exception ex){

        }
    }
    @Override
    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        Loading(true);
        switch (requestCode) {
            case MAX:
                if (resultCode == Activity.RESULT_OK && data != null) {
                    userInfo.maxSelectNumber = String.valueOf(data.getIntExtra("number",0));
                    UpdateUserInfo(LLSharedPreferences.LL(getContext()).getSaveSession(),userInfo.loginName,userInfo.maxSelectNumber,userInfo.digit1,userInfo.digit2,userInfo.digit3);
                }
                break;
            case FIX:
                if (resultCode == Activity.RESULT_OK && data != null) {
                    String sequence = String.valueOf(data.getIntExtra("number",0));
                    if(sequence.equals("0")){
                            userInfo.digit3 = sequence;
                            userInfo.digit2 = sequence;
                            userInfo.digit1 = sequence;
                    }else {
                        if(!(userInfo.digit1.equals(sequence) || userInfo.digit2.equals(sequence) || userInfo.digit3.equals(sequence))){
                            if(userInfo.digit1.equals("0") == true){
                                userInfo.digit1 = String.valueOf(sequence);
                            }
                            else if (userInfo.digit2.equals("0") == true){
                                userInfo.digit2 = String.valueOf(sequence);
                            }
                            else{
                                userInfo.digit3 = String.valueOf(sequence);
                            }
                        }
                    }
                    UpdateUserInfo(LLSharedPreferences.LL(getContext()).getSaveSession(),userInfo.loginName,userInfo.maxSelectNumber,userInfo.digit1,userInfo.digit2,userInfo.digit3);
                }
                break;
        }
    }

    private void Loading(boolean view){
        Button btn_loading = (Button)rootView.findViewById(R.id.btn_loading);
        ProgressBar pb_loading = (ProgressBar)rootView.findViewById(R.id.pb_loading);

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
