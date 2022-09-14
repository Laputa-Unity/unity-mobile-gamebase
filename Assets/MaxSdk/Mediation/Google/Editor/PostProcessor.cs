//
//  PostProcessor.cs
//  AppLovin MAX Unity Plugin
//
//  Created by Santosh Bagadi on 6/4/19.
//  Copyright © 2019 AppLovin. All rights reserved.
//

#if UNITY_IPHONE || UNITY_IOS

using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;

namespace AppLovinMax.Mediation.Google.Editor
{
    /// <summary>
    /// A post processor that will add the AdMob App ID to the <c>info.plist</c> file.
    /// </summary>
    public class PostProcessor
    {
        [PostProcessBuild]
        public static void OnPostProcessBuild(BuildTarget buildTarget, string buildPath)
        {
            var appId = MaxMediationGoogleUtils.GetAppIdFromAppLovinSettings("AdMobIosAppId");

            // Log error if the App ID is not set.
            if (string.IsNullOrEmpty(appId) || !appId.StartsWith("ca-app-pub-"))
            {
                Debug.LogError("[AppLovin MAX] AdMob App ID is not set. Please enter a valid app ID within the AppLovin Integration Manager window.");
                return;
            }

            var plistPath = Path.Combine(buildPath, "Info.plist");
            var plist = new PlistDocument();
            plist.ReadFromFile(plistPath);

            // Actually set (then write) AdMob app id to Info.plist if valid
            plist.root.SetString("GADApplicationIdentifier", appId);

            File.WriteAllText(plistPath, plist.WriteToString());
        }
    }
}

#endif
