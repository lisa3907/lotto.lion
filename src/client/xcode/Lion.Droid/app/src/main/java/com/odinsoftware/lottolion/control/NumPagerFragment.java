package com.odinsoftware.lottolion.control;

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
import android.widget.ListView;
import android.widget.ProgressBar;
import android.widget.TextView;

import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;
import com.odinsoftware.lottolion.LoginActivity;
import com.odinsoftware.lottolion.R;
import com.odinsoftware.lottolion.service.ApiAsnycTask;
import com.odinsoftware.lottolion.service.LLAlert;
import com.odinsoftware.lottolion.service.LLCommon;
import com.odinsoftware.lottolion.service.LLSharedPreferences;
import com.odinsoftware.lottolion.type.ApiResult;
import com.odinsoftware.lottolion.type.UserChoices;

import java.util.ArrayList;
import java.util.HashMap;

/**
 * Created by limyg on 2017-04-25.
 */
public class NumPagerFragment extends Fragment {
    View rootView;
    ArrayList<UserChoices> list = new ArrayList<>();
    ListViewNumAdapter adapter;
    ImageView iv_pre, iv_post;

    int sequence;
    int maxSequence;
    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        rootView = inflater.inflate(R.layout.fragment_num, container, false);
        InitData(rootView);
        InitSize(rootView);
        InitEvent(rootView);
        return rootView;
    }
    public void Init(){
        InitData(rootView);
    }

    private void InitEvent(View view) {
        ImageView iv_mail = (ImageView)view.findViewById(R.id.iv_mail);

        iv_pre.setOnTouchListener(listener_touch);
        iv_pre.setOnClickListener(listener_click);

        iv_post.setOnTouchListener(listener_touch);
        iv_post.setOnClickListener(listener_click);

        iv_mail.setOnTouchListener(listener_touch);
        iv_mail.setOnClickListener(listener_click);
    }

    private void InitSize(View view) {
        iv_pre.setLayoutParams(LLCommon.Get().ResizeDP(getContext(),iv_pre.getLayoutParams()));
        iv_post.setLayoutParams(LLCommon.Get().ResizeDP(getContext(),iv_post.getLayoutParams()));
    }

    private void InitData(View view) {
        if(view == null)
            return;
        String token = LLSharedPreferences.LL(getContext()).getSaveSession();
        iv_pre = (ImageView)view.findViewById(R.id.iv_pre);
        iv_post = (ImageView)view.findViewById(R.id.iv_post);

        if(LLCommon.Get().CheckExpired(token) == false){
            sequence = LLSharedPreferences.LL(getContext()).getSequence();
            maxSequence = sequence;
            PrePost(sequence);

            ImageView iv_cover = (ImageView)view.findViewById(R.id.iv_cover);
            ListView lv_lotto = (ListView)view.findViewById(R.id.lv_lotto);
            TextView tv_seqnece = (TextView)view.findViewById(R.id.tv_sequence);
            tv_seqnece.setText(sequence+"회차 (예상)");

            adapter = new ListViewNumAdapter(getContext(),R.layout.listview_num_custom,list);
            lv_lotto.setAdapter(adapter);
            iv_cover.setVisibility(View.INVISIBLE);

            GetUserChoices(token,sequence);
        }
    }

    private void GetUserChoices(String key, int sequence){
        try{
            Loading(true);
            HashMap<String, String> parameter = new HashMap<>();
            parameter.put("sequence_no",String.valueOf(sequence));
            ApiAsnycTask api = new ApiAsnycTask(getString(R.string.api_url)+"lotto/GetUserChoices",key);
            api.execute(parameter);
            api.finish = new ApiAsnycTask.Finish() {
                @Override
                public void Finish(String json) {
                    Gson g = new Gson();
                    ApiResult<ArrayList<UserChoices>> result = g.fromJson(json, new TypeToken<ApiResult<ArrayList<UserChoices>>>(){}.getType());
                    if(result != null){
                        if(result.success){
                            list.clear();
                            list.addAll(result.result);
                            adapter.notifyDataSetChanged();
                            TextView tv_noting = (TextView)rootView.findViewById(R.id.tv_noting);
                            if(result.result.size() == 0)
                                tv_noting.setVisibility(View.VISIBLE);
                            else
                                tv_noting.setVisibility(View.INVISIBLE);

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
    private void SendChoicedNumbers(String key, int sequence){
        try{
            Loading(true);
            HashMap<String, String> parameter = new HashMap<>();
            parameter.put("sequence_no",String.valueOf(sequence));
            ApiAsnycTask api = new ApiAsnycTask(getString(R.string.api_url)+"lotto/SendChoicedNumbers",key);
            api.execute(parameter);
            api.finish = new ApiAsnycTask.Finish() {
                @Override
                public void Finish(String json) {
                    Gson g = new Gson();
                    ApiResult<String> result = g.fromJson(json, new TypeToken<ApiResult<String>>(){}.getType());
                    if(result != null){
                        if(result.success){
                            LLAlert.Get(getContext()).Alert("메일이 발송되었습니다.");
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
    View.OnClickListener listener_click = new View.OnClickListener() {
        @Override
        public void onClick(View v) {
            String token = LLSharedPreferences.LL(getContext()).getSaveSession();
            if(LLCommon.Get().CheckExpired(token) == false){
                switch(v.getId()){
                    case R.id.iv_pre:
                        sequence -=1;
                        if(sequence < 1)
                            sequence = 1;
                        GetUserChoices(token,sequence);
                        break;
                    case R.id.iv_post:
                        sequence +=1;
                        if(sequence > maxSequence)
                            sequence = maxSequence;
                        GetUserChoices(token,sequence);
                        break;
                    case R.id.iv_mail:
                        SendChoicedNumbers(token,sequence);
                        break;
                }
                TextView tv_seqnece = (TextView)rootView.findViewById(R.id.tv_sequence);
                tv_seqnece.setText(sequence+"회차 "+(sequence == maxSequence? "(예상)":"(결과)"));
                PrePost(sequence);
            }
            else{
                Intent intent = new Intent(getContext(), LoginActivity.class);
                startActivity(intent);
            }
        }
    };
    private void PrePost(int sequence){
        if(sequence == 1){
           iv_pre.setVisibility(View.INVISIBLE);
        }
        else if (sequence == maxSequence){
            iv_post.setVisibility(View.INVISIBLE);
        }
        else{
            iv_pre.setVisibility(View.VISIBLE);
            iv_post.setVisibility(View.VISIBLE);
        }
    }
    View.OnTouchListener listener_touch = new View.OnTouchListener() {
        @Override
        public boolean onTouch(View v, MotionEvent event) {
            // TODO Auto-generated method stub
            ImageView iv = (ImageView)v;
            if(event.getAction() == MotionEvent.ACTION_DOWN){
                iv.setColorFilter(0xaa111111, PorterDuff.Mode.SRC_OVER);
            }
            else if(event.getAction() == MotionEvent.ACTION_UP){
                iv.setColorFilter(0x00000000, PorterDuff.Mode.SRC_OVER);
            }

            return false;
        }
    };
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
