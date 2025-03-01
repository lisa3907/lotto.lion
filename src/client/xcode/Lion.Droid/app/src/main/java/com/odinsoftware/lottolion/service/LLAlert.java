package com.odinsoftware.lottolion.service;

import android.content.Context;
import android.content.DialogInterface;
import android.support.v7.app.AlertDialog;

import com.odinsoftware.lottolion.R;

/**
 * Created by limyg on 2017-04-25.
 */
public class LLAlert {
    private static Context context;
    private static LLAlert sigleton = null;
    public interface DialogOkCancelListener{
        void DialogOkCancelListener();
    }
    DialogOkCancelListener dialogOkListener;
    DialogOkCancelListener dialogCancelListener;

    public static LLAlert Get(Context p_context){
        context = p_context;
        if(sigleton == null){
            sigleton = new LLAlert();
        }
        return sigleton;
    }
    public void Alert(String text){
        AlertDialog.Builder builder = new AlertDialog.Builder(context, android.R.style.Theme_Holo_Light_Dialog_NoActionBar);
        builder.setTitle("알림")
                .setMessage(text)
                .setCancelable(false)
                .setPositiveButton("확인", new DialogInterface.OnClickListener(){
                    public void onClick(DialogInterface dialog, int whichButton){
                        dialog.cancel();;
                    }
                });
        builder.create().show();
    }
    public void Alert(String text, DialogOkCancelListener listener){
        this.dialogOkListener = listener;
        AlertDialog.Builder builder = new AlertDialog.Builder(context, android.R.style.Theme_Holo_Light_Dialog_NoActionBar);
        builder.setTitle("알림")
                .setMessage(text)
                .setCancelable(false)
                .setPositiveButton("확인", new DialogInterface.OnClickListener(){
                    public void onClick(DialogInterface dialog, int whichButton){
                        if(dialogOkListener != null)
                            dialogOkListener.DialogOkCancelListener();
                        dialog.cancel();;
                    }
                });
        builder.create().show();
    }
    public void CancelAlert(String text, DialogOkCancelListener listener){
        this.dialogOkListener = listener;
        AlertDialog.Builder builder = new AlertDialog.Builder(context, android.R.style.Theme_Holo_Light_Dialog_NoActionBar);
        builder.setTitle("알림")
                .setMessage(text)
                .setCancelable(false)
                .setPositiveButton("확인", new DialogInterface.OnClickListener(){
                    public void onClick(DialogInterface dialog, int whichButton){
                        if(dialogOkListener != null)
                            dialogOkListener.DialogOkCancelListener();
                        dialog.cancel();;
                    }
                })
                .setNegativeButton("취소", new DialogInterface.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialog, int which) {

                    }
                });
        builder.create().show();
    }
}
