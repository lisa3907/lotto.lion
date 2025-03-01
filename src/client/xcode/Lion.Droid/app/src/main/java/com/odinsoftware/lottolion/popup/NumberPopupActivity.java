package com.odinsoftware.lottolion.popup;

import android.app.Activity;
import android.content.Intent;
import android.graphics.PorterDuff;
import android.media.Image;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.MotionEvent;
import android.view.View;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.ListView;
import android.widget.NumberPicker;

import com.odinsoftware.lottolion.R;
import com.odinsoftware.lottolion.control.ListViewResultAdapter;
import com.odinsoftware.lottolion.service.LLCommon;

import java.util.ArrayList;
import java.util.List;

public class NumberPopupActivity extends Activity {

    //NumberPicker np_popup;
    Intent intent;
    int result;
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_number_popup);

        InitData();
        InitEvent();
        InitSize();
    }

    private void InitSize() {
        Button btn_ok = (Button)findViewById(R.id.btn_ok);
        Button btn_cancel = (Button)findViewById(R.id.btn_cancel);

        btn_ok.setLayoutParams(LLCommon.Get().ResizeDP(this,btn_ok.getLayoutParams()));
        btn_cancel.setLayoutParams(LLCommon.Get().ResizeDP(this,btn_cancel.getLayoutParams()));
    }

    private void InitEvent() {
        Button btn_ok = (Button)findViewById(R.id.btn_ok);
        Button btn_cancel = (Button)findViewById(R.id.btn_cancel);

        btn_ok.setOnClickListener(listener_click);
        btn_cancel.setOnClickListener(listener_click);
        btn_ok.setOnTouchListener(listener_touch);
        btn_cancel.setOnTouchListener(listener_touch);

    }

    private void InitData() {
        intent = getIntent();
        ListView lv_popup = (ListView)findViewById(R.id.lv_popup);
        int min =intent.getIntExtra("min",1);
        int max= intent.getIntExtra("max",10);
        boolean reverse = intent.getBooleanExtra("reverse",true);

        ArrayList<Integer> list = new ArrayList<>();
        if(reverse){
            result= max;
            for(int i= max; i>= min ; i--){
                list.add(i);
            }
        }
        else{
            result= min;
            for(int i= min; i<= max ; i++){
                list.add(i);
            }
        }
        ListViewResultAdapter adapter = new ListViewResultAdapter(this,R.layout.listview_result_custom,list);
        adapter.resultClickListener = new ListViewResultAdapter.ResultOnClickListener() {
            @Override
            public void onClickListener(int position) {
                result = position;
            }
        };
        lv_popup.setAdapter(adapter);

    }
    View.OnClickListener listener_click = new View.OnClickListener() {
        @Override
        public void onClick(View v) {
            switch (v.getId()){
                case R.id.btn_ok:
                    intent.putExtra("number",result);
                    setResult(RESULT_OK, intent);
                    finish();
                    break;
                case R.id.btn_cancel:
                    setResult(RESULT_CANCELED, intent);
                    finish();
                    break;
            }
        }
    };
    View.OnTouchListener listener_touch =  new View.OnTouchListener() {
        @Override
        public boolean onTouch(View v, MotionEvent event) {
            Button view = (Button)v;
            if(event.getAction() == MotionEvent.ACTION_DOWN){
                view.getBackground().setColorFilter(0x10000000, PorterDuff.Mode.SRC_OVER);
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
}
