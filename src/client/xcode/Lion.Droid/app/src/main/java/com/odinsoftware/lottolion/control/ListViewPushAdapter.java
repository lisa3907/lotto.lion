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
import com.odinsoftware.lottolion.service.LLCommon;
import com.odinsoftware.lottolion.type.PushMessage;

import java.util.ArrayList;
import java.util.Calendar;

/**
 * Created by limyg on 2017-04-27.
 */
public class ListViewPushAdapter extends ArrayAdapter {
    int resourceId ;

    public ListViewPushAdapter(Context context, int resource, ArrayList<PushMessage> list) {
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
        TextView tv_date = (TextView) convertView.findViewById(R.id.tv_date);
        TextView tv_message = (TextView) convertView.findViewById(R.id.tv_message);
        Button btn_center = (Button) convertView.findViewById(R.id.btn_center);
        PushMessage message = (PushMessage) getItem(position);

        Calendar cd_date = LLCommon.Get().GetDate(message.notifyTime);
        String date = "%04d"+".%02d"+".%02d"+" (%s)";

        tv_date.setText(String.format(date.toString()
                ,cd_date.get(Calendar.YEAR)
                ,cd_date.get(Calendar.MONTH)+1
                ,cd_date.get(Calendar.DAY_OF_MONTH)
                ,LLCommon.Get().doDayOfWeek(cd_date.get(Calendar.DAY_OF_WEEK))));
        tv_message.setText(message.message);

        if(message.isRead){
            btn_center.getBackground().setColorFilter(0x10000000, PorterDuff.Mode.SRC_OVER);
        }
        else{
            btn_center.getBackground().setColorFilter(0x00000000, PorterDuff.Mode.SRC_OVER);
        }
        return convertView;
    }
}
