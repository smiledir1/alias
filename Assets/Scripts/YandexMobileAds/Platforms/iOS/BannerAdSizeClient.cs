/*
 * This file is a part of the Yandex Advertising Network
 *
 * Version for iOS (C) 2023 YANDEX
 *
 * You may not use this file except in compliance with the License.
 * You may obtain a copy of the License at https://legal.yandex.com/partner_ch/
 */

using System;
using YandexMobileAds.Base;
using System.Collections.Generic;

namespace YandexMobileAds.Platforms.iOS
{
    #if (UNITY_5 && UNITY_IOS) || UNITY_IPHONE

    internal class BannerAdSizeClient : IDisposable
    {
        public string ObjectId { get; private set; }

        public BannerAdSizeClient(BannerAdSize adSize)
        {
            if (adSize.AdSizeType == BannerAdSizeType.Sticky)
            {
                this.ObjectId = BannerAdSizeBridge.YMAUnityCreateStickyBannerAdSize(adSize.Width);
            }
            else if (adSize.AdSizeType == BannerAdSizeType.Inline)
            {
                this.ObjectId = BannerAdSizeBridge.YMAUnityCreateInlineBannerAdSize(adSize.Width, adSize.Height);
            }
            else if (adSize.AdSizeType == BannerAdSizeType.Fixed)
            {
                this.ObjectId = BannerAdSizeBridge.YMAUnityCreateFixedBannerAdSize(adSize.Width, adSize.Height);
            }
        }

        public void Destroy()
        {
            ObjectBridge.YMAUnityDestroyObject(this.ObjectId);
        }

        public void Dispose()
        {
            this.Destroy();
        }

        ~BannerAdSizeClient()
        {
            this.Destroy();
        }
    }

    #endif
}
