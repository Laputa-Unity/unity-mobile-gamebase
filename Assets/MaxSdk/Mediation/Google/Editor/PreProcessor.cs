//
//  PreProcessor.cs
//  AppLovin MAX Unity Plugin
//
//  Created by Santosh Bagadi on 11/7/19.
//  Copyright © 2019 AppLovin. All rights reserved.
//

#if UNITY_ANDROID

using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEditor;
using UnityEditor.Build;
#if UNITY_2018_1_OR_NEWER
using UnityEditor.Build.Reporting;
#endif
using UnityEngine;

namespace AppLovinMax.Mediation.Google.Editor
{
    /// <summary>
    /// A pre processor that will add the AdMob App ID to the <c>AndroidManifest.xml</c> file.
    /// </summary>
    public class PreProcessor :
#if UNITY_2018_1_OR_NEWER
        IPreprocessBuildWithReport
#else
        IPreprocessBuild
#endif
    {
#if UNITY_2018_1_OR_NEWER
        public void OnPreprocessBuild(BuildReport report)
#else
        public void OnPreprocessBuild(BuildTarget target, string path)
#endif
        {
            var appId = MaxMediationGoogleUtils.GetAppIdFromAppLovinSettings("AdMobAndroidAppId");
            if (string.IsNullOrEmpty(appId))
            {
                Debug.LogError("[AppLovin MAX] AdMob App ID is not set. Please enter a valid app ID within the ");
                return;
            }

            var manifestPath = Path.Combine(Application.dataPath, "Plugins/Android/MaxMediationGoogle.androidlib/AndroidManifest.xml");

            XDocument manifest;
            try
            {
                manifest = XDocument.Load(manifestPath);
            }
#pragma warning disable 0168
            catch (IOException exception)
#pragma warning restore 0168
            {
                Debug.LogError("[AppLovin MAX] Google mediation AndroidManifest.xml is missing. Ensure that MAX Google mediation plugin is imported correctly.");
                return;
            }

            // Get the `manifest` element.
            var elementManifest = manifest.Element("manifest");
            if (elementManifest == null)
            {
                Debug.LogError("[AppLovin MAX] Google mediation AndroidManifest.xml is invalid. Ensure that MAX Google mediation plugin is imported correctly.");
                return;
            }

            // Get the `application` element under `manifest`.
            var elementApplication = elementManifest.Element("application");
            if (elementApplication == null)
            {
                Debug.LogError("[AppLovin MAX] Google mediation AndroidManifest.xml is invalid. Ensure that MAX Google mediation plugin is imported correctly.");
                return;
            }

            // Get all the `meta-data` elements under `application`.
            var adMobMetaData = elementApplication.Descendants().First(element => element.Name.LocalName.Equals("meta-data"));
            XNamespace androidNamespace = "http://schemas.android.com/apk/res/android";

            if (!adMobMetaData.FirstAttribute.Name.Namespace.Equals(androidNamespace) ||
                !adMobMetaData.FirstAttribute.Name.LocalName.Equals("name") ||
                !adMobMetaData.FirstAttribute.Value.Equals("com.google.android.gms.ads.APPLICATION_ID"))
            {
                Debug.LogError("[AppLovin MAX] Google mediation AndroidManifest.xml is invalid. Ensure that MAX Google mediation plugin is imported correctly.");
                return;
            }

            var lastAttribute = adMobMetaData.LastAttribute;
            // Log error if the AdMob App ID is not set.
            if (!lastAttribute.Name.LocalName.Equals("value"))
            {
                Debug.LogError("[AppLovin MAX] Google mediation AndroidManifest.xml is invalid. Ensure that MAX Google mediation plugin is imported correctly.");
            }

            // Set the App ID value.
            lastAttribute.Value = appId;

            // Save the updated manifest file.
            manifest.Save(manifestPath);
        }

        public int callbackOrder
        {
            get { return 0; }
        }
    }
}

#endif