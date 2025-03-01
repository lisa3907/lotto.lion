package com.odinsoftware.lottolion.control;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentManager;
import android.support.v4.app.FragmentPagerAdapter;
import android.view.LayoutInflater;
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
import com.odinsoftware.lottolion.type.ApiResult;
import com.odinsoftware.lottolion.type.PrizeBySeqNo;
import com.odinsoftware.lottolion.type.ThisWeekPrize;

import java.util.ArrayList;
import java.util.Calendar;
import java.util.HashMap;

/**
 * Created by limyg on 2017-04-24.
 */
public class ResultPagerFragment extends Fragment {

    View rootView;

//    String jwt = "";
    int maxSequence = 0;

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        rootView = inflater.inflate(R.layout.fragment_result, container, false);
        InitSize(rootView);
        InitEvent(rootView);
        InitData(rootView);

        return rootView;
    }

    private void InitData(View view) {
        String token = LLCommon.Get().GetGuestToken(getContext());
        GetThisWeekPrize(token);
    }

    private void InitEvent(View view) {
        Button btn_sequence = (Button)view.findViewById(R.id.btn_sequence);
        btn_sequence.setOnClickListener(listener_click);
    }

    private void InitSize(View view) {
        ImageView iv_label1 = (ImageView)view.findViewById(R.id.iv_label1);
        ImageView iv_lotto1 = (ImageView)view.findViewById(R.id.iv_lotto1);
        ImageView iv_lotto2 = (ImageView)view.findViewById(R.id.iv_lotto2);
        ImageView iv_lotto3 = (ImageView)view.findViewById(R.id.iv_lotto3);
        ImageView iv_lotto4 = (ImageView)view.findViewById(R.id.iv_lotto4);
        ImageView iv_lotto5 = (ImageView)view.findViewById(R.id.iv_lotto5);
        ImageView iv_lotto6 = (ImageView)view.findViewById(R.id.iv_lotto6);
        ImageView iv_lotto7 = (ImageView)view.findViewById(R.id.iv_lotto7);
        ImageView iv_result1 = (ImageView)view.findViewById(R.id.iv_result1);
        ImageView iv_result2 = (ImageView)view.findViewById(R.id.iv_result2);
        ImageView iv_result3 = (ImageView)view.findViewById(R.id.iv_result3);
        ImageView iv_result4 = (ImageView)view.findViewById(R.id.iv_result4);
        ImageView iv_result5 = (ImageView)view.findViewById(R.id.iv_result5);
        ImageView iv_line1 = (ImageView)view.findViewById(R.id.iv_line1);
        ImageView iv_line2 = (ImageView)view.findViewById(R.id.iv_line2);
        ImageView iv_line3 = (ImageView)view.findViewById(R.id.iv_line3);
        ImageView iv_line4 = (ImageView)view.findViewById(R.id.iv_line4);
        ImageView iv_line5 = (ImageView)view.findViewById(R.id.iv_line5);
        ImageView iv_line6 = (ImageView)view.findViewById(R.id.iv_line6);
        ImageView iv_line7 = (ImageView)view.findViewById(R.id.iv_line7);
        ImageView iv_line8 = (ImageView)view.findViewById(R.id.iv_line8);

        iv_label1.setLayoutParams(LLCommon.Get().ResizeDP(getContext(),iv_label1.getLayoutParams()));
        iv_lotto1.setLayoutParams(LLCommon.Get().ResizeDP(getContext(),iv_lotto1.getLayoutParams()));
        iv_lotto2.setLayoutParams(LLCommon.Get().ResizeDP(getContext(),iv_lotto2.getLayoutParams()));
        iv_lotto3.setLayoutParams(LLCommon.Get().ResizeDP(getContext(),iv_lotto3.getLayoutParams()));
        iv_lotto4.setLayoutParams(LLCommon.Get().ResizeDP(getContext(),iv_lotto4.getLayoutParams()));
        iv_lotto5.setLayoutParams(LLCommon.Get().ResizeDP(getContext(),iv_lotto5.getLayoutParams()));
        iv_lotto6.setLayoutParams(LLCommon.Get().ResizeDP(getContext(),iv_lotto6.getLayoutParams()));
        iv_lotto7.setLayoutParams(LLCommon.Get().ResizeDP(getContext(),iv_lotto7.getLayoutParams()));

        iv_result1.setLayoutParams(LLCommon.Get().ResizeDP(getContext(),iv_result1.getLayoutParams()));
        iv_result2.setLayoutParams(LLCommon.Get().ResizeDP(getContext(),iv_result2.getLayoutParams()));
        iv_result3.setLayoutParams(LLCommon.Get().ResizeDP(getContext(),iv_result3.getLayoutParams()));
        iv_result4.setLayoutParams(LLCommon.Get().ResizeDP(getContext(),iv_result4.getLayoutParams()));
        iv_result5.setLayoutParams(LLCommon.Get().ResizeDP(getContext(),iv_result5.getLayoutParams()));
        iv_line1.setLayoutParams(LLCommon.Get().ResizeDP(getContext(),iv_line1.getLayoutParams()));
        iv_line2.setLayoutParams(LLCommon.Get().ResizeDP(getContext(),iv_line2.getLayoutParams()));
        iv_line3.setLayoutParams(LLCommon.Get().ResizeDP(getContext(),iv_line3.getLayoutParams()));
        iv_line4.setLayoutParams(LLCommon.Get().ResizeDP(getContext(),iv_line4.getLayoutParams()));
        iv_line5.setLayoutParams(LLCommon.Get().ResizeDP(getContext(),iv_line5.getLayoutParams()));
        iv_line6.setLayoutParams(LLCommon.Get().ResizeDP(getContext(),iv_line6.getLayoutParams()));
        iv_line7.setLayoutParams(LLCommon.Get().ResizeDP(getContext(),iv_line7.getLayoutParams()));
        iv_line8.setLayoutParams(LLCommon.Get().ResizeDP(getContext(),iv_line8.getLayoutParams()));
    }

    View.OnClickListener listener_click = new View.OnClickListener() {
        @Override
        public void onClick(View v) {
            switch (v.getId()){
                case R.id.btn_sequence:
                    Intent intent = new Intent(getContext(), NumberPopupActivity.class);
                    intent.putExtra("min",1);
                    intent.putExtra("max",maxSequence);
                    startActivityForResult(intent,1);
                    break;
            }
        }
    };

    @Override
    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        Loading(true);
        switch (requestCode) {
            case 1:
                if (resultCode == Activity.RESULT_OK && data != null) {
                    TextView tv_sequence = (TextView) rootView.findViewById(R.id.tv_sequence);
                    int sequence = data.getIntExtra("number", 1);
                    tv_sequence.setText(sequence + "회차");
                    GetPrizeBySeqNo(LLCommon.Get().GetGuestToken(getContext()), sequence);
                }
                Loading(false);
                break;
        }
    }

    private void InitThisWeekPrize(View view, ThisWeekPrize weekPrize){
        TextView tv_predictDate = (TextView)view.findViewById(R.id.tv_predictDate);
        TextView tv_predictSequence = (TextView)view.findViewById(R.id.tv_predictSequence);
        TextView tv_predict = (TextView)view.findViewById(R.id.tv_predict);
        TextView tv_sales = (TextView)view.findViewById(R.id.tv_sales);
        Calendar cd_predictDate = LLCommon.Get().GetDate(weekPrize.issueDate);
        String date = "%04d"+"-%02d"+"-%02d";

        tv_predictDate.setText(String.format(date.toString()
                ,cd_predictDate.get(Calendar.YEAR)
                ,cd_predictDate.get(Calendar.MONTH)+1
                ,cd_predictDate.get(Calendar.DAY_OF_MONTH)));
        tv_predictSequence.setText(weekPrize.sequenceNo+"회차");
        tv_predict.setText(LLCommon.Get().GetStringComma(weekPrize.predictAmount)+"원");
        tv_sales.setText(LLCommon.Get().GetStringComma(weekPrize.salesAmount)+"원");
    }
    private void InitPrizeBySeqNo(View view, PrizeBySeqNo prize){
        TextView tv_date = (TextView)view.findViewById(R.id.tv_date);
        TextView tv_sequence = (TextView)view.findViewById(R.id.tv_sequence);
        ImageView iv_lotto1 = (ImageView)view.findViewById(R.id.iv_lotto1);
        TextView tv_lotto1 = (TextView)view.findViewById(R.id.tv_lotto1);
        ImageView iv_lotto2 = (ImageView)view.findViewById(R.id.iv_lotto2);
        TextView tv_lotto2 = (TextView)view.findViewById(R.id.tv_lotto2);
        ImageView iv_lotto3 = (ImageView)view.findViewById(R.id.iv_lotto3);
        TextView tv_lotto3 = (TextView)view.findViewById(R.id.tv_lotto3);
        ImageView iv_lotto4 = (ImageView)view.findViewById(R.id.iv_lotto4);
        TextView tv_lotto4 = (TextView)view.findViewById(R.id.tv_lotto4);
        ImageView iv_lotto5 = (ImageView)view.findViewById(R.id.iv_lotto5);
        TextView tv_lotto5 = (TextView)view.findViewById(R.id.tv_lotto5);
        ImageView iv_lotto6 = (ImageView)view.findViewById(R.id.iv_lotto6);
        TextView tv_lotto6 = (TextView)view.findViewById(R.id.tv_lotto6);
        ImageView iv_lotto7 = (ImageView)view.findViewById(R.id.iv_lotto7);
        TextView tv_lotto7 = (TextView)view.findViewById(R.id.tv_lotto7);
        TextView tv_count_result1 = (TextView)view.findViewById(R.id.tv_count_result1);
        TextView tv_amount_result1 = (TextView)view.findViewById(R.id.tv_amount_result1);
        TextView tv_count_result2 = (TextView)view.findViewById(R.id.tv_count_result2);
        TextView tv_amount_result2 = (TextView)view.findViewById(R.id.tv_amount_result2);
        TextView tv_count_result3 = (TextView)view.findViewById(R.id.tv_count_result3);
        TextView tv_amount_result3 = (TextView)view.findViewById(R.id.tv_amount_result3);
        TextView tv_count_result4 = (TextView)view.findViewById(R.id.tv_count_result4);
        TextView tv_amount_result4 = (TextView)view.findViewById(R.id.tv_amount_result4);
        TextView tv_count_result5 = (TextView)view.findViewById(R.id.tv_count_result5);
        TextView tv_amount_result5 = (TextView)view.findViewById(R.id.tv_amount_result5);

        Calendar cd_predictDate = LLCommon.Get().GetDate(prize.issueDate);
        String date = "%04d"+"-%02d"+"-%02d";

        tv_date.setText(String.format(date.toString()
                ,cd_predictDate.get(Calendar.YEAR)
                ,cd_predictDate.get(Calendar.MONTH)+1
                ,cd_predictDate.get(Calendar.DAY_OF_MONTH)));
        tv_sequence.setText(prize.sequenceNo+"회차");

        tv_lotto1.setText(prize.digit1);
        tv_lotto2.setText(prize.digit2);
        tv_lotto3.setText(prize.digit3);
        tv_lotto4.setText(prize.digit4);
        tv_lotto5.setText(prize.digit5);
        tv_lotto6.setText(prize.digit6);
        tv_lotto7.setText(prize.digit7);
        CheckLottoNum(Integer.parseInt(prize.digit1),iv_lotto1);
        CheckLottoNum(Integer.parseInt(prize.digit2),iv_lotto2);
        CheckLottoNum(Integer.parseInt(prize.digit3),iv_lotto3);
        CheckLottoNum(Integer.parseInt(prize.digit4),iv_lotto4);
        CheckLottoNum(Integer.parseInt(prize.digit5),iv_lotto5);
        CheckLottoNum(Integer.parseInt(prize.digit6),iv_lotto6);
        CheckLottoNum(Integer.parseInt(prize.digit7),iv_lotto7);

        tv_amount_result1.setText(LLCommon.Get().GetStringComma(prize.amount1)+"원");
        tv_amount_result2.setText(LLCommon.Get().GetStringComma(prize.amount2)+"원");
        tv_amount_result3.setText(LLCommon.Get().GetStringComma(prize.amount3)+"원");
        tv_amount_result4.setText(LLCommon.Get().GetStringComma(prize.amount4)+"원");
        tv_amount_result5.setText(LLCommon.Get().GetStringComma(prize.amount5)+"원");
        tv_count_result1.setText(LLCommon.Get().GetStringComma(prize.count1)+"명");
        tv_count_result2.setText(LLCommon.Get().GetStringComma(prize.count2)+"명");
        tv_count_result3.setText(LLCommon.Get().GetStringComma(prize.count3)+"명");
        tv_count_result4.setText(LLCommon.Get().GetStringComma(prize.count4)+"명");
        tv_count_result5.setText(LLCommon.Get().GetStringComma(prize.count5)+"명");
    }
    private void CheckLottoNum(int num, ImageView iv){
        if(num/40 == 1){
            iv.setImageResource(R.drawable.circled_g);
        }
        else if(num/30 == 1){
            iv.setImageResource(R.drawable.circled_k);
        }
        else if(num/20 == 1){
            iv.setImageResource(R.drawable.circled_r);
        }
        else if(num/10 == 1){
            iv.setImageResource(R.drawable.circled_b);
        }
        else{
            iv.setImageResource(R.drawable.circled_y);
        }
    }

    private void GetThisWeekPrize(String key){
        Loading(true);

        try{
            final String _key = key;
            ApiAsnycTask api = new ApiAsnycTask(getString(R.string.api_url)+"lotto/GetThisWeekPrize",key);
            api.execute();
            api.finish = new ApiAsnycTask.Finish() {
                @Override
                public void Finish(String json) {
                    Gson g = new Gson();
                    ApiResult<ThisWeekPrize> result = g.fromJson(json, new TypeToken<ApiResult<ThisWeekPrize>>(){}.getType());
                    if(result!=null){
                        LLSharedPreferences.LL(getContext()).putSequence(result.result.sequenceNo);
                        maxSequence =result.result.sequenceNo -1;
                        if(result.success){
                            InitThisWeekPrize(rootView,result.result);
                            GetPrizeBySeqNo(_key, maxSequence);
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
    private void GetPrizeBySeqNo(String key, int sequence_no){
        Loading(true);
        try{
            HashMap<String, String> parameter = new HashMap<>();
            parameter.put("sequence_no",String.valueOf(sequence_no));
            ApiAsnycTask api = new ApiAsnycTask(getString(R.string.api_url)+"lotto/GetPrizeBySeqNo",key);
            api.execute(parameter);
            api.finish = new ApiAsnycTask.Finish() {
                @Override
                public void Finish(String json) {
                    Gson g = new Gson();
                    ApiResult<PrizeBySeqNo> result = g.fromJson(json, new TypeToken<ApiResult<PrizeBySeqNo>>(){}.getType());
                    if(result != null){
                        if(result.success){
                            InitPrizeBySeqNo(rootView,result.result);
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
