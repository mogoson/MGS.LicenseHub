/*************************************************************************
 *  Copyright © 2025 Mogoson All rights reserved.
 *------------------------------------------------------------------------
 *  File         :  LicenseSettingsEditor.cs
 *  Description  :  Default.
 *------------------------------------------------------------------------
 *  Author       :  Mogoson
 *  Version      :  1.0.0
 *  Date         :  12/08/2025
 *  Description  :  Initial development version.
 *************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace MGS.License.Editors
{
    sealed class LicenseSettingsEditor : EditorWindow, IPreprocessBuildWithReport
    {
        #region
        [MenuItem("Tools/License/Settings", priority = 0)]
        static void ShowEditor()
        {
            GetWindow<LicenseSettingsEditor>("License Settings").Show();
        }
        #endregion

        #region
        void OnEnable()
        {
            InitSettings();
        }

        void OnGUI()
        {
            DrawEditor();
        }
        #endregion

        #region
        string publicKey;
        string privateKey;
        string version = "1.0.0";
        List<string> entitlements = new();

        void InitSettings()
        {
            var licSettings = LoadLicenseSettings();
            if (licSettings)
            {
                publicKey = licSettings.publicKey;
                privateKey = licSettings.privateKey;
                version = licSettings.version;
            }
            var proSettings = LoadProductSettings();
            if (proSettings)
            {
                entitlements.AddRange(proSettings.entitlements);
            }
        }
        #endregion

        #region
        const string LICENSE_SETTINGS_PATH = "Assets/LicenseSettings/Editor/LicenseSettings.asset";

        internal static LicenseSettings LoadLicenseSettings()
        {
            return AssetDatabase.LoadAssetAtPath<LicenseSettings>(LICENSE_SETTINGS_PATH);
        }

        LicenseSettings RequireLicenseSettings()
        {
            var settings = LoadLicenseSettings();
            if (settings == null)
            {
                RequireDirectory(LICENSE_SETTINGS_PATH);
                settings = CreateInstance<LicenseSettings>();
                AssetDatabase.CreateAsset(settings, LICENSE_SETTINGS_PATH);
                Debug.Log($"LicenseSettingsEditor create LicenseSettings at {LICENSE_SETTINGS_PATH}");
            }
            return settings;
        }

        void ApplyLicenseSettings()
        {
            var settings = RequireLicenseSettings();
            settings.publicKey = publicKey;
            settings.privateKey = privateKey;
            settings.version = version;
            AssetDatabaseSave(settings);
            Debug.Log("LicenseSettingsEditor has applied parameters to the LicenseSettings.");
        }
        #endregion

        #region
        const string PRODUCT_SETTINGS_PATH = "Assets/LicenseSettings/Resources/ProductSettings.asset";

        ProductSettings LoadProductSettings()
        {
            return AssetDatabase.LoadAssetAtPath<ProductSettings>(PRODUCT_SETTINGS_PATH);
        }

        ProductSettings RequireProductSettings()
        {
            var settings = LoadProductSettings();
            if (settings == null)
            {
                RequireDirectory(PRODUCT_SETTINGS_PATH);
                settings = CreateInstance<ProductSettings>();
                AssetDatabase.CreateAsset(settings, PRODUCT_SETTINGS_PATH);
                Debug.Log($"LicenseSettingsEditor create ProductSettings at {PRODUCT_SETTINGS_PATH}");
            }
            return settings;
        }

        void ApplyProductSettings()
        {
            var settings = RequireProductSettings();
            if (settings.licenseKey != privateKey)
            {
                LicenseHub.ClearTimestamp();
            }

            settings.licenseKey = privateKey;
            settings.supportedLicenseVersion = version;
            settings.timestamp = DateTime.UtcNow.ToBinary();
            settings.entitlements = entitlements.ToArray();
            AssetDatabaseSave(settings);
            Debug.Log("LicenseSettingsEditor has applied parameters to the ProductSettings.");
        }
        #endregion

        #region
        void ApplySettins()
        {
            if (!CheckSettings())
            {
                return;
            }
            ApplyLicenseSettings();
            ApplyProductSettings();
        }

        bool CheckSettings()
        {
            if (string.IsNullOrEmpty(publicKey) || string.IsNullOrEmpty(privateKey))
            {
                Debug.LogError("The key is invalid.");
                return false;
            }
            if (string.IsNullOrEmpty(version))
            {
                Debug.LogError("The version is invalid.");
                return false;
            }
            var temp = new List<string>();
            foreach (var entitlement in entitlements)
            {
                if (string.IsNullOrEmpty(entitlement))
                {
                    Debug.LogError("The empty entitlement item is invalid.");
                    return false;
                }
                if (temp.Contains(entitlement))
                {
                    Debug.LogError($"The repeat entitlement item {entitlement} is invalid.");
                    return false;
                }
                temp.Add(entitlement);
            }
            return true;
        }

        void RequireDirectory(string path)
        {
            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

        void AssetDatabaseSave(UnityEngine.Object asset)
        {
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssetIfDirty(asset);
        }
        #endregion

        #region
        void GenerateKey(out string publicKey, out string privateKey)
        {
            using var rsa = new RSACryptoServiceProvider();
            publicKey = rsa.ToXmlString(false);
            privateKey = rsa.ToXmlString(true);
        }
        #endregion

        #region
        void DrawEditor()
        {
            DrawKeySettings();
            DrawEntitlementSettings();
            DrawVersionSettings();
            DrawApplySettings();
        }

        void DrawKeySettings()
        {
            GUILayout.Label("Public Key");
            GUILayout.Box(publicKey, GUILayout.ExpandWidth(true));
            GUILayout.Label("Private Key");
            GUILayout.Box(privateKey, GUILayout.ExpandWidth(true));
            if (GUILayout.Button("Generate"))
            {
                GenerateKey(out publicKey, out privateKey);
            }
        }

        void DrawEntitlementSettings()
        {
            GUILayout.Label("Entitlements");
            for (int i = 0; i < entitlements.Count; i++)
            {
                entitlements[i] = GUILayout.TextField(entitlements[i]);
            }
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add"))
            {
                entitlements.Add(string.Empty);
            }
            if (GUILayout.Button("Remove"))
            {
                if (entitlements.Count > 0)
                {
                    entitlements.RemoveAt(entitlements.Count - 1);
                }
            }
            GUILayout.EndHorizontal();
        }

        void DrawVersionSettings()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Version", GUILayout.Width(80));
            version = GUILayout.TextField(version, GUILayout.Width(120));
            GUILayout.EndHorizontal();
        }

        void DrawApplySettings()
        {
            if (GUILayout.Button("Apply"))
            {
                ApplySettins();
            }
        }
        #endregion

        #region
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            var settings = LoadProductSettings();
            if (settings)
            {
                settings.timestamp = DateTime.UtcNow.ToBinary();
                AssetDatabaseSave(settings);
            }
        }
        #endregion
    }
}