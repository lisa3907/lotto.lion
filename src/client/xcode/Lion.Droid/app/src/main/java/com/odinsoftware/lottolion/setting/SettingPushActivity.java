package com.odinsoftware.lottolion.setting;

import android.graphics.PorterDuff;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.MotionEvent;
import android.view.View;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.ListView;
import android.widget.ProgressBar;
import android.widget.TextView;

import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;
import com.odinsoftware.lottolion.R;
import com.odinsoftware.lottolion.control.ListViewPushAdapter;
import com.odinsoftware.lottolion.service.ApiAsnycTask;
import com.odinsoftware.lottolion.service.LLAlert;
import com.odinsoftware.lottolion.service.LLCommon;
import com.odinsoftware.lottolion.service.LLSharedPreferences;
import com.odinsoftware.lottolion.type.ApiResult;
import com.odinsoftware.lottolion.type.PushMessage;

import java.util.ArrayList;
import java.util.Calendar;
import java.util.HashMap;

public class SettingPushActivity extends AppCompatActivity {
    ArrayList<PushMessage> list = new ArrayList<>();
    ListViewPushAdapter adapter;
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_setting_push);

        InitSize();
        InitData();
        InitEvent();
    }

    private void InitEvent() {
        ImageView iv_back = (ImageView)findViewById(R.id.iv_back);

        iv_back.setOnTouchListener(listener_touch_back);
        iv_back.setOnClickListener(listener_back);
    }

    private void InitData() {
        String token = LLSharedPreferences.LL(getApplicationContext()).getSaveSession();

        if(LLCommon.Get().CheckExpired(token) == false){
            GetMessages(token);
            PushClear(token);
        }
        ListView lv_push = (ListView)findViewById(R.id.lv_push);
        adapter = new ListViewPushAdapter(this,R.layout.listview_push_custom, list);
        lv_push.setAdapter(adapter);
    }

    private void InitSize() {
        ImageView iv_back = (ImageView)findViewById(R.id.iv_back);
        ImageView iv_back1 = (ImageView)findViewById(R.id.iv_back1);
        ImageView iv_line1 = (ImageView)findViewById(R.id.iv_line1);

        iv_back.setLayoutParams(LLCommon.Get().ResizeDP(this,iv_back.getLayoutParams()));
        iv_back1.setLayoutParams(LLCommon.Get().ResizeDP(this,iv_back1.getLayoutParams()));
        iv_line1.setLayoutParams(LLCommon.Get().ResizeDP(this,iv_line1.getLayoutParams()));
    }
    private void GetMessages(String key){
        try{
            Loading(true);
            HashMap<String, String> parameter = new HashMap<>();
            parameter.put("notify_time",LLCommon.Get().GetDate(Calendar.getInstance()));
            ApiAsnycTask api = new ApiAsnycTask(getString(R.string.api_url)+"notify/GetMessages",key);
            api.execute(parameter);
            api.finish = new ApiAsnycTask.Finish() {
                @Override
                public void Finish(String json) {
                    Gson g = new Gson();
                    ApiResult<ArrayList<PushMessage>> result = g.fromJson(json, new TypeToken<ApiResult<ArrayList<PushMessage>>>(){}.getType());
                    if(result != null){
                        if(result.success){
                            list.clear();
                            list.addAll(result.result);
                            adapter.notifyDataSetChanged();
                            TextView tv_noting = (TextView)findViewById(R.id.tv_noting);
                            if(result.result.size() == 0)
                                tv_noting.setVisibility(View.VISIBLE);
                            else
                                tv_noting.setVisibility(View.INVISIBLE);

                        }
                        else{
                            LLAlert.Get(SettingPushActivity.this).Alert(result.message);
                        }
                    }
                    Loading(false);
                }
            };
        }
        catch (Exception ex){

        }
    }
    private void PushClear(String key){
        try{
            HashMap<String, String> parameter = new HashMap<>();
            ApiAsnycTask api = new ApiAsnycTask(getString(R.string.api_url)+"notify/Clear",key);
            api.execute(parameter);
        }
        catch (Exception ex){

        }
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
