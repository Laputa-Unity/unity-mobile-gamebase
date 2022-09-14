//
//  MaxGoogleInitialize.cs
//  AppLovin MAX Unity Plugin
//
//  Created by Santosh Bagadi on 10/5/20.
//  Copyright © 2020 AppLovin. All rights reserved.
//

using System.IO;
using UnityEditor;
using UnityEngine;

namespace AppLovinMax.Mediation.Google.Editor
{
    [InitializeOnLoad]
    public class MaxGoogleInitialize
    {
        private static readonly string LegacyMaxMediationGoogleDir = Path.Combine("Assets", "Plugins/Android/MaxMediationGoogle");

        static MaxGoogleInitialize()
        {
            // Check if the MaxMediationGoogle directory exists and append .androidlib to it.
            if (Directory.Exists(LegacyMaxMediationGoogleDir))
            {
                Debug.Log("[AppLovin MAX] Updating Google Android library directory name to make it compatible with Unity 2020+ versions.");

                FileUtil.MoveFileOrDirectory(LegacyMaxMediationGoogleDir, LegacyMaxMediationGoogleDir + ".androidlib");
                FileUtil.MoveFileOrDirectory(LegacyMaxMediationGoogleDir + ".meta", LegacyMaxMediationGoogleDir + ".androidlib" + ".meta");
                AssetDatabase.Refresh();
            }
        }
    }
}
