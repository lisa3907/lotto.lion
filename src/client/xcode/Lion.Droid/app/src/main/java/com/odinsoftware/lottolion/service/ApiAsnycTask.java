package com.odinsoftware.lottolion.service;

import android.os.AsyncTask;

import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.InputStreamReader;
import java.io.OutputStream;
import java.io.OutputStreamWriter;
import java.io.UnsupportedEncodingException;
import java.net.HttpURLConnection;
import java.net.URL;
import java.net.URLEncoder;
import java.util.HashMap;
import java.util.Map;

/**
 * Created by limyg on 2017-04-21.
 */
public class ApiAsnycTask extends AsyncTask<HashMap<String, String>, Void, String> {
    private String SERVER_URL;
    private String KEY;

    public interface Finish{
        void Finish(String json);
    }

    public Finish finish;

    public ApiAsnycTask(String p_serverUrl, String p_key){
        SERVER_URL = p_serverUrl;
        KEY = p_key;
    }

    @Override
    protected void onPostExecute(String s) {
        if(finish!=null)
            finish.Finish(s);
    }

    @Override
    protected String doInBackground(HashMap<String, String>... params) {
        BufferedReader bufferedReader = null;
        StringBuilder stringBuilder = new StringBuilder();
        HttpURLConnection urlConnection = null;
        try {
            URL url = new URL(SERVER_URL);
            urlConnection = (HttpURLConnection) url.openConnection();
           // if(KEY.equals("") == false)
                urlConnection.setRequestProperty("Authorization","Bearer "+ KEY);
            urlConnection.setDoInput(true);
            urlConnection.setDoOutput(true);
            urlConnection.setRequestMethod("POST");
            urlConnection.setConnectTimeout(5000);

            if (params.length > 0&& params[0] != null) { // 웹 서버로 보낼 매개변수가 있는 경우우
                OutputStream os = urlConnection.getOutputStream(); // 서버로 보내기 위한 출력 스트림
                BufferedWriter bw = new BufferedWriter(new OutputStreamWriter(os, "UTF-8")); // UTF-8로 전송
                bw.write(getPostString(params[0])); // 매개변수 전송
                bw.flush();
                bw.close();
                os.close();
            }

            if(urlConnection.getResponseCode() == HttpURLConnection.HTTP_OK){
                bufferedReader = new BufferedReader(new InputStreamReader(urlConnection.getInputStream()));
                stringBuilder = new StringBuilder();
                String line;
                while ((line = bufferedReader.readLine()) != null) {
                    stringBuilder.append(line).append("\n");
                }
            }
        }
        catch(Exception e) {
            String message = e.getMessage();
        }
        finally{
            if(urlConnection != null)
                urlConnection.disconnect();
        }
        return stringBuilder.toString();
    }
    private String getPostString(HashMap<String, String> map) {
        StringBuilder result = new StringBuilder();
        boolean first = true; // 첫 번째 매개변수 여부

        for (Map.Entry<String, String> entry : map.entrySet()) {
            if (first)
                first = false;
            else // 첫 번째 매개변수가 아닌 경우엔 앞에 &를 붙임
                result.append("&");

            try { // UTF-8로 주소에 키와 값을 붙임
                result.append(URLEncoder.encode(entry.getKey(), "UTF-8"));
                result.append("=");
                result.append(URLEncoder.encode(entry.getValue(), "UTF-8"));
            } catch (UnsupportedEncodingException ue) {
                ue.printStackTrace();
            } catch (Exception e) {
                e.printStackTrace();
            }
        }

        return result.toString();
    }

}
