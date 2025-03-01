package com.odinsoftware.lottolion.control;

import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.ImageView;
import android.widget.TextView;

import com.odinsoftware.lottolion.R;
import com.odinsoftware.lottolion.service.LLCommon;
import com.odinsoftware.lottolion.type.UserChoices;

import java.util.ArrayList;

/**
 * Created by limyg on 2017-04-26.
 */
public class ListViewNumAdapter extends ArrayAdapter {
    int resourceId ;
    public ListViewNumAdapter(Context context, int resource, ArrayList<UserChoices> list) {
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
            InitSize(convertView);
        }
        // 화면에 표시될 View(Layout이 inflate된)로부터 위젯에 대한 참조 획득
        ImageView iv_lotto1 = (ImageView)convertView.findViewById(R.id.iv_lotto1);
        TextView tv_lotto1 = (TextView)convertView.findViewById(R.id.tv_lotto1);
        ImageView iv_lotto2 = (ImageView)convertView.findViewById(R.id.iv_lotto2);
        TextView tv_lotto2 = (TextView)convertView.findViewById(R.id.tv_lotto2);
        ImageView iv_lotto3 = (ImageView)convertView.findViewById(R.id.iv_lotto3);
        TextView tv_lotto3 = (TextView)convertView.findViewById(R.id.tv_lotto3);
        ImageView iv_lotto4 = (ImageView)convertView.findViewById(R.id.iv_lotto4);
        TextView tv_lotto4 = (TextView)convertView.findViewById(R.id.tv_lotto4);
        ImageView iv_lotto5 = (ImageView)convertView.findViewById(R.id.iv_lotto5);
        TextView tv_lotto5 = (TextView)convertView.findViewById(R.id.tv_lotto5);
        ImageView iv_lotto6 = (ImageView)convertView.findViewById(R.id.iv_lotto6);
        TextView tv_lotto6 = (TextView)convertView.findViewById(R.id.tv_lotto6);

        TextView tv_result = (TextView)convertView.findViewById(R.id.tv_result);
        TextView tv_amount = (TextView)convertView.findViewById(R.id.tv_amount);

        ImageView iv_cover = (ImageView)convertView.findViewById(R.id.iv_cover);
        final UserChoices lotto = (UserChoices) getItem(position);

        tv_lotto1.setText(lotto.digit1);
        tv_lotto2.setText(lotto.digit2);
        tv_lotto3.setText(lotto.digit3);
        tv_lotto4.setText(lotto.digit4);
        tv_lotto5.setText(lotto.digit5);
        tv_lotto6.setText(lotto.digit6);

        CheckLottoNum(Integer.parseInt(lotto.digit1),iv_lotto1);
        CheckLottoNum(Integer.parseInt(lotto.digit2),iv_lotto2);
        CheckLottoNum(Integer.parseInt(lotto.digit3),iv_lotto3);
        CheckLottoNum(Integer.parseInt(lotto.digit4),iv_lotto4);
        CheckLottoNum(Integer.parseInt(lotto.digit5),iv_lotto5);
        CheckLottoNum(Integer.parseInt(lotto.digit6),iv_lotto6);

        if(lotto.ranking.equals("0")){
            iv_cover.setVisibility(View.INVISIBLE);
            tv_result.setText("당첨결과: -");
            tv_amount.setText("당첨금액: -");
        }
        else if(lotto.ranking.equals("6")){
            iv_cover.setVisibility(View.VISIBLE);
            tv_result.setText("당첨결과: X");
            tv_amount.setText("당첨금액: 0원");
        }
        else {
            iv_cover.setVisibility(View.INVISIBLE);
            tv_result.setText("당첨결과: "+lotto.ranking+"등");
            tv_amount.setText("당첨금액: "+ LLCommon.Get().GetStringComma(lotto.amount)+"원");
        }

        
        return convertView;
    }

    private void InitSize(View view) {
        ImageView iv_lotto1 = (ImageView)view.findViewById(R.id.iv_lotto1);
        ImageView iv_lotto2 = (ImageView)view.findViewById(R.id.iv_lotto2);
        ImageView iv_lotto3 = (ImageView)view.findViewById(R.id.iv_lotto3);
        ImageView iv_lotto4 = (ImageView)view.findViewById(R.id.iv_lotto4);
        ImageView iv_lotto5 = (ImageView)view.findViewById(R.id.iv_lotto5);
        ImageView iv_lotto6 = (ImageView)view.findViewById(R.id.iv_lotto6);

        iv_lotto1.setLayoutParams(LLCommon.Get().ResizeDP(getContext(),iv_lotto1.getLayoutParams()));
        iv_lotto2.setLayoutParams(LLCommon.Get().ResizeDP(getContext(),iv_lotto2.getLayoutParams()));
        iv_lotto3.setLayoutParams(LLCommon.Get().ResizeDP(getContext(),iv_lotto3.getLayoutParams()));
        iv_lotto4.setLayoutParams(LLCommon.Get().ResizeDP(getContext(),iv_lotto4.getLayoutParams()));
        iv_lotto5.setLayoutParams(LLCommon.Get().ResizeDP(getContext(),iv_lotto5.getLayoutParams()));
        iv_lotto6.setLayoutParams(LLCommon.Get().ResizeDP(getContext(),iv_lotto6.getLayoutParams()));
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
}
