/*
 * This file is a part of the Yandex Advertising Network
 *
 * Version for Android (C) 2023 YANDEX
 *
 * You may not use this file except in compliance with the License.
 * You may obtain a copy of the License at https://legal.yandex.com/partner_ch/
 */

using UnityEngine;
using YandexMobileAds.Base;

namespace YandexMobileAds.Platforms.Android
{
    internal class BannerAdSizeUtils
    {
        public const string AdSizeClassName = "com.yandex.mobile.ads.banner.BannerAdSize";
        public const string FixedSizeMethodName = "fixedSize";
        public const string InlineSizeMethodName = "inlineSize";
        public const string StickySizeMethodName = "stickySize";

        public static AndroidJavaObject GetBannerAdSizeJavaObject(BannerAdSize bannerAdSize)
        {
            AndroidJavaClass adSizeClass = new AndroidJavaClass(AdSizeClassName);
            AndroidJavaObject adSizeJavaObject = null;
            AndroidJavaObject activity = Utils.GetCurrentActivity();
            if (bannerAdSize.AdSizeType == BannerAdSizeType.Sticky)
            {
                adSizeJavaObject = adSizeClass.CallStatic<AndroidJavaObject>(
                    StickySizeMethodName,
                    activity,
                    bannerAdSize.Width);
            }
            else if (bannerAdSize.AdSizeType == BannerAdSizeType.Inline)
            {
                adSizeJavaObject = adSizeClass.CallStatic<AndroidJavaObject>(
                    InlineSizeMethodName,
                    activity,
                    bannerAdSize.Width,
                    bannerAdSize.Height);
            }
            else if (bannerAdSize.AdSizeType == BannerAdSizeType.Fixed)
            {
                adSizeJavaObject = adSizeClass.CallStatic<AndroidJavaObject>(
                    FixedSizeMethodName,
                    activity,
                    bannerAdSize.Width,
                    bannerAdSize.Height);
            }
            return adSizeJavaObject;
        }
    }
}
