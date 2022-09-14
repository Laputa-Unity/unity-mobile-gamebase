//
//  MaxMediationGoogleUtils.cs
//  AppLovin MAX Unity Plugin
//
//  Created by Santosh Bagadi on 11/7/19.
//  Copyright © 2019 AppLovin. All rights reserved.
//

using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace AppLovinMax.Mediation.Google.Editor
{
    /// <summary>
    /// An Utils class containing shared convenience methods.
    /// </summary>
    public static class MaxMediationGoogleUtils
    {
        private const string AppLovinSettingsExportPath = "MaxSdk/Resources/AppLovinSettings.asset";

        /// <summary>
        /// Loads the AppLovin Settings asset if it is available and returns the value for the given property name.
        /// </summary>
        /// <param name="property">The name of the property for which to get the value of from <c>AppLovinSettings.asset</c> file.</param>
        /// <returns>The string value of the property if found.</returns>
        public static string GetAppIdFromAppLovinSettings(string property)
        {
            var settingsFileName = GetAppLovinSettingsAssetPath();
            if (!File.Exists(settingsFileName))
            {
                Debug.LogError("[AppLovin MAX] The current plugin version is incompatible with the AdMob adapter. Please update the AppLovin MAX plugin to version 2.4.0 or higher.");
                return null;
            }

            var instance = AssetDatabase.LoadAssetAtPath(settingsFileName, Type.GetType("AppLovinSettings, MaxSdk.Scripts.IntegrationManager.Editor"));
            if (instance == null)
            {
                Debug.LogError("[AppLovin MAX] The current plugin version is incompatible with the AdMob adapter. Please update the AppLovin MAX plugin to version 2.4.15 or higher");
                return null;
            }

            var adMobAppIdProperty = instance.GetType().GetProperty(property);
            return adMobAppIdProperty == null ? null : adMobAppIdProperty.GetValue(instance, null).ToString();
        }

        private static string GetAppLovinSettingsAssetPath()
        {
            // Since the settings asset is generated during compile time, the asset label will contain platform specific path separator. So, use platform specific export path.  
            var assetLabel = "l:al_max_export_path-" + AppLovinSettingsExportPath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            var guids = AssetDatabase.FindAssets(assetLabel);

            var defaultPath = Path.Combine("Assets", AppLovinSettingsExportPath);

            return guids.Length > 0 ? AssetDatabase.GUIDToAssetPath(guids[0]) : defaultPath;
        }
    }
}
