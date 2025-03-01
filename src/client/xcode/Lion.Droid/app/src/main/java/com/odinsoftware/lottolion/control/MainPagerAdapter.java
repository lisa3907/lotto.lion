package com.odinsoftware.lottolion.control;

import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentManager;
import android.support.v4.app.FragmentPagerAdapter;
import android.support.v4.view.PagerAdapter;

import java.util.ArrayList;

/**
 * Created by limyg on 2017-04-24.
 */
public class MainPagerAdapter extends FragmentPagerAdapter {
    public interface MainPager{
        int 추첨결과 = 0;
        int 번호관리 = 1;
        int 로또판매점 = 2;
        int 설정 = 3;
    }
    private final ArrayList<Fragment> mFragments = new ArrayList<>();
    public MainPagerAdapter(FragmentManager fm) {
        super(fm);
    }
    public void addFragment(Fragment fragment)
    {
        mFragments.add(fragment);
    }
    @Override
    public Fragment getItem(int position) {
        return mFragments.get(position);
    }

    @Override
    public int getCount() {
        return mFragments.size();
    }

    @Override
    public CharSequence getPageTitle(int position) {
        switch (position) {
            case MainPager.추첨결과:
                return "추첨결과";
            case MainPager.번호관리:
                return "번호관리";
            case MainPager.로또판매점:
                return "로또판매점";
            case MainPager.설정:
                return "설정";
        }
        return "알수없음";
    }

}
