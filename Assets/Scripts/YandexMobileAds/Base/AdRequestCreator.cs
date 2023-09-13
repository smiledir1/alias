using System.Collections.Generic;
using YandexMobileAds.Base;

namespace YandexMobileAds.Base
{
    public class AdRequestCreator
    {
        private const string PluginTypeParameter = "plugin_type";
        private const string PluginVersionParameter = "plugin_version";

        private const string PluginType = "unity";

        public AdRequest CreateAdRequest(AdRequest adRequest)
        {
            var parameters = adRequest.Parameters ?? new Dictionary<string, string>();
            parameters.Add(PluginTypeParameter, PluginType);
            parameters.Add(PluginVersionParameter, MobileAdsPackageInfo.PackageVersion);

            return new AdRequest.Builder()
                .WithAdRequest(adRequest)
                .WithParameters(parameters)
                .Build();
        }
    }
}
