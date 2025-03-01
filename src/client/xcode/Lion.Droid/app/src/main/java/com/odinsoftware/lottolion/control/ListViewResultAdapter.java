package com.odinsoftware.lottolion.control;

import android.content.Context;
import android.graphics.PorterDuff;
import android.view.LayoutInflater;
import android.view.MotionEvent;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.TextView;

import com.odinsoftware.lottolion.R;

import java.util.ArrayList;

/**
 * Created by limyg on 2017-04-25.
 */
public class ListViewResultAdapter extends ArrayAdapter {
    int resourceId ;
    int selectedSequence = -1;
    Button preBtn;
    public interface ResultOnClickListener{
        void onClickListener(int position);
    }
    public ResultOnClickListener resultClickListener;
    // ListViewBtnAdapter 생성자. 마지막에 ListBtnClickListener 추가.
    public ListViewResultAdapter(Context context, int resource, ArrayList<Integer> list) {
        super(context, resource, list) ;

        // resource id 값 복사. (super로 전달된 resource를 참조할 방법이 없음.)
        this.resourceId = resource ;
    }
    @Override
    public View getView(int position, View convertView, ViewGroup parent) {
        int pos = position ;
        final Context context = parent.getContext();

        // 생성자로부터 저장된 resourceId(listview_btn_item)에 해당하는 Layout을 inflate하여 convertView 참조 획득.
        if (convertView == null) {
            LayoutInflater inflater = (LayoutInflater) context.getSystemService(Context.LAYOUT_INFLATER_SERVICE);
            convertView = inflater.inflate(this.resourceId, parent, false);
        }
        // 화면에 표시될 View(Layout이 inflate된)로부터 위젯에 대한 참조 획득
        final TextView tv_result = (TextView) convertView.findViewById(R.id.tv_result);
        Button btn_center = (Button) convertView.findViewById(R.id.btn_center);
        final Integer sequence = (Integer) getItem(position);
        btn_center.setTag(sequence);

        // Data Set(listViewItemList)에서 position에 위치한 데이터 참조 획득
        String value = sequence.toString();

        // 아이템 내 각 위젯에 데이터 반영
        if(value.equals("0"))
            value = "Clear";
        tv_result.setText(value);

        if(selectedSequence == sequence){
            btn_center.getBackground().setColorFilter(0x10000000, PorterDuff.Mode.SRC_OVER);
        }
        else{
            btn_center.getBackground().setColorFilter(0x00000000, PorterDuff.Mode.SRC_OVER);
        }
        btn_center.invalidate();
        // button1 클릭 시 TextView(textView1)의 내용 변경.
        btn_center.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                Button view = (Button)v;
                if(preBtn!=null){
                    preBtn.getBackground().setColorFilter(0x00000000, PorterDuff.Mode.SRC_OVER);
                    preBtn.invalidate();
                }
                view.getBackground().setColorFilter(0x10000000, PorterDuff.Mode.SRC_OVER);
                view.invalidate();
                selectedSequence = (int)view.getTag();
                preBtn = view;

                if(resultClickListener != null)
                    resultClickListener.onClickListener((int)v.getTag());
            }
        });
        btn_center.setOnTouchListener(new View.OnTouchListener() {
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
        });

        return convertView;
    }
}
