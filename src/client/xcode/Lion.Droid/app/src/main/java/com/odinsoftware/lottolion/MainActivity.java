package com.odinsoftware.lottolion;

import android.content.Intent;
import android.graphics.Color;
import android.graphics.PorterDuff;
import android.media.Image;
import android.support.design.widget.TabLayout;
import android.support.v4.view.ViewPager;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.widget.ImageView;
import android.widget.TextView;

import com.google.android.gms.ads.AdRequest;
import com.google.android.gms.ads.AdView;
import com.google.android.gms.ads.MobileAds;
import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;
import com.odinsoftware.lottolion.control.MainPagerAdapter;
import com.odinsoftware.lottolion.control.NumPagerFragment;
import com.odinsoftware.lottolion.control.ResultPagerFragment;
import com.odinsoftware.lottolion.control.SettingPagerFragment;
import com.odinsoftware.lottolion.control.StorePagerFragment;
import com.odinsoftware.lottolion.service.ApiAsnycTask;
import com.odinsoftware.lottolion.service.LLCommon;
import com.odinsoftware.lottolion.service.LLSharedPreferences;
import com.odinsoftware.lottolion.type.ApiResult;

import java.util.HashMap;

public class MainActivity extends AppCompatActivity {

    MainPagerAdapter mainPagerAdapter;
    int selectColor = Color.parseColor("#3296DC");
    int deSelectColor = Color.parseColor("#9c9c9c");
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        InitTab();
        InitEvent();
        InitAdMob();

    }

    private void InitAdMob() {
        MobileAds.initialize(getApplicationContext(), getString(R.string.banner_ad_unit_id));
        AdView mAdView = (AdView) findViewById(R.id.adView);
        AdRequest adRequest = new AdRequest.Builder().build();
        mAdView.loadAd(adRequest);
    }

    private void InitEvent() {
        TabLayout tl_main = (TabLayout)findViewById(R.id.tl_main);
        tl_main.setOnTabSelectedListener(listener_tab);
    }

    private void InitTab() {
        mainPagerAdapter = new MainPagerAdapter(getSupportFragmentManager());

        ResultPagerFragment result1 = new ResultPagerFragment();
        NumPagerFragment result2 = new NumPagerFragment();
        StorePagerFragment result3 = new StorePagerFragment();
        SettingPagerFragment result4 = new SettingPagerFragment ();
        mainPagerAdapter.addFragment(result1);
        mainPagerAdapter.addFragment(result2);
        mainPagerAdapter.addFragment(result3);
        mainPagerAdapter.addFragment(result4);
        result4.refresh=new SettingPagerFragment.Refresh() {
            @Override
            public void onRefresh() {
                onResume();
            }
        };

        ViewPager vp_main = (ViewPager) findViewById(R.id.vp_main);
        vp_main.setAdapter(mainPagerAdapter);

        TabLayout tl_main = (TabLayout)findViewById(R.id.tl_main);
        tl_main.setupWithViewPager(vp_main);
        setupTabIcons();
    }

    @Override
    protected void onResume() {
        super.onResume();
        TabLayout tl_main = (TabLayout)findViewById(R.id.tl_main);
        if(LLCommon.Get().CheckExpired(LLSharedPreferences.LL(getApplicationContext()).getSaveSession()) && (position == 1 || position == 3)) {
            TabLayout.Tab tab = tl_main.getTabAt(0);
            tab.select();
        }
        else{
            if(mainPagerAdapter.getItem(position) instanceof NumPagerFragment){
                TabLayout.Tab tab = tl_main.getTabAt(position);
                tab.select();
                ViewPager vp_main = (ViewPager) findViewById(R.id.vp_main);
                vp_main.setCurrentItem(position);
                ((NumPagerFragment)mainPagerAdapter.getItem(position)).Init();
            }
            if(mainPagerAdapter.getItem(position) instanceof SettingPagerFragment){
                TabLayout.Tab tab = tl_main.getTabAt(position);
                tab.select();
                ViewPager vp_main = (ViewPager) findViewById(R.id.vp_main);
                vp_main.setCurrentItem(position);
                ((SettingPagerFragment)mainPagerAdapter.getItem(position)).Init();
            }
        }
    }

    int position=0;
    TabLayout.OnTabSelectedListener listener_tab = new TabLayout.OnTabSelectedListener() {
        @Override
        public void onTabSelected(TabLayout.Tab tab) {
            position = tab.getPosition();
            ViewPager vp_main = (ViewPager) findViewById(R.id.vp_main);
            if(LLCommon.Get().CheckExpired(LLSharedPreferences.LL(getApplicationContext()).getSaveSession())&& (tab.getPosition() ==1 || tab.getPosition() ==3)){
                vp_main.setCurrentItem(0);
                Intent intent = new Intent(MainActivity.this, LoginActivity.class);
                startActivity(intent);
            }
            else{
                if (tab.getPosition() != vp_main.getCurrentItem()) {
                    vp_main.setCurrentItem(tab.getPosition());
                }
                if(mainPagerAdapter.getItem(position) instanceof NumPagerFragment){
                    ((NumPagerFragment)mainPagerAdapter.getItem(position)).Init();
                }
                if(mainPagerAdapter.getItem(position) instanceof SettingPagerFragment){
                    ((SettingPagerFragment)mainPagerAdapter.getItem(position)).Init();
                }

                TextView tv_tab = (TextView) tab.getCustomView().findViewById(R.id.tv_tab);
                tv_tab.setTextColor(selectColor);
                ImageView iv_tab = (ImageView) tab.getCustomView().findViewById(R.id.iv_tab);
                iv_tab.setColorFilter(selectColor,PorterDuff.Mode.SRC_IN);
            }
        }

        @Override
        public void onTabUnselected(TabLayout.Tab tab) {
            ViewPager vp_main = (ViewPager) findViewById(R.id.vp_main);
            if (tab.getPosition() != vp_main.getCurrentItem()) {
                vp_main.setCurrentItem(tab.getPosition());
            }
            TextView tv_tab = (TextView) tab.getCustomView().findViewById(R.id.tv_tab);
            tv_tab.setTextColor(deSelectColor);
            ImageView iv_tab = (ImageView) tab.getCustomView().findViewById(R.id.iv_tab);
            iv_tab.setColorFilter(deSelectColor,PorterDuff.Mode.SRC_IN);
        }

        @Override
        public void onTabReselected(TabLayout.Tab tab) {
            String s = "";
        }
    };
    private void setupTabIcons() {
        TabLayout tl_main = (TabLayout)findViewById(R.id.tl_main);
        View view1 = LayoutInflater.from(this).inflate(R.layout.main_tab, null);
        ImageView iv_tab1 = (ImageView)view1.findViewById(R.id.iv_tab);
        TextView tv_tab1 = (TextView)view1.findViewById(R.id.tv_tab);
        tv_tab1.setText(mainPagerAdapter.getPageTitle(0));
        iv_tab1.setImageResource(R.drawable.main1);
        tv_tab1.setTextColor(selectColor);
        iv_tab1.setColorFilter(selectColor,PorterDuff.Mode.SRC_IN);
        tl_main.getTabAt(0).setCustomView(view1);


        View view2 = LayoutInflater.from(this).inflate(R.layout.main_tab, null);
        ImageView iv_tab2 = (ImageView)view2.findViewById(R.id.iv_tab);
        TextView tv_tab2 = (TextView)view2.findViewById(R.id.tv_tab);
        tv_tab2.setText(mainPagerAdapter.getPageTitle(1));
        iv_tab2.setImageResource(R.drawable.main2);
        tv_tab2.setTextColor(deSelectColor);
        iv_tab2.setColorFilter(deSelectColor,PorterDuff.Mode.SRC_IN);
        tl_main.getTabAt(1).setCustomView(view2);

        View view3 = LayoutInflater.from(this).inflate(R.layout.main_tab, null);
        ImageView iv_tab3 = (ImageView)view3.findViewById(R.id.iv_tab);
        TextView tv_tab3 = (TextView)view3.findViewById(R.id.tv_tab);
        tv_tab3.setText(mainPagerAdapter.getPageTitle(2));
        iv_tab3.setImageResource(R.drawable.main3);
        tv_tab3.setTextColor(deSelectColor);
        iv_tab3.setColorFilter(deSelectColor,PorterDuff.Mode.SRC_IN);
        tl_main.getTabAt(2).setCustomView(view3);

        View view4 = LayoutInflater.from(this).inflate(R.layout.main_tab, null);
        ImageView iv_tab4 = (ImageView)view4.findViewById(R.id.iv_tab);
        TextView tv_tab4 = (TextView)view4.findViewById(R.id.tv_tab);
        tv_tab4.setText(mainPagerAdapter.getPageTitle(3));
        iv_tab4.setImageResource(R.drawable.main4);
        tv_tab4.setTextColor(deSelectColor);
        iv_tab4.setColorFilter(deSelectColor,PorterDuff.Mode.SRC_IN);
        tl_main.getTabAt(3).setCustomView(view4);
    }


}
