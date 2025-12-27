/*************************************************************************
 *  Copyright © 2025 Mogoson All rights reserved.
 *------------------------------------------------------------------------
 *  File         :  LicenseBuilderEditor.cs
 *  Description  :  Default.
 *------------------------------------------------------------------------
 *  Author       :  Mogoson
 *  Version      :  1.0.0
 *  Date         :  11/30/2025
 *  Description  :  Initial development version.
 *************************************************************************/

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace MGS.License.Editors
{
    sealed class LicenseBuilderEditor : EditorWindow
    {
        #region
        [MenuItem("Tools/License/Builder", priority = 1)]
        static void ShowEditor()
        {
            GetWindow<LicenseBuilderEditor>("License Builder").Show();
        }
        #endregion

        #region
        void OnGUI()
        {
            DrawEditor();
        }
        #endregion

        #region
        string request;
        bool permanent;
        int days = 30;
        string license;

        class Entitlement
        {
            public bool permanent;
            public int days = 30;
        }

        Dictionary<string, Entitlement> entitlements = new();
        #endregion

        #region
        string BuildLicense()
        {
            if (!CheckInputValid())
            {
                return null;
            }
            var settings = LicenseSettingsEditor.LoadLicenseSettings();
            if (!CheckSettingsValid(settings))
            {
                return null;
            }
            var info = LicenseHub.FromRequestText(request);
            if (!CheckRequestValid(info, settings))
            {
                return null;
            }
            return BuildLicense(info, settings);
        }

        string BuildLicense(RequestInfo info, LicenseSettings settings)
        {
            var content = new LicenseContent()
            {
                version = settings.version,
                device = LicenseHub.ComputeDeviceID(info.deviceInfo),
                creation = DateTime.UtcNow,
                expiry = permanent ? DateTime.MaxValue : DateTime.UtcNow.AddDays(days),
                entitlements = BuildEntitlements()
            };

            var json = LicenseHub.ToJson(content);
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }

            var signText = SignData(json, settings.privateKey);
            var licenseInfo = new LicenseInfo()
            {
                content = json,
                signature = signText,
            };
            return LicenseHub.ToLicenseText(licenseInfo);
        }

        EntitlementInfo[] BuildEntitlements()
        {
            if (entitlements.Count == 0)
            {
                return null;
            }

            var index = 0;
            var entitles = new EntitlementInfo[entitlements.Count];
            foreach (var entitlement in entitlements)
            {
                entitles[index] = new EntitlementInfo()
                {
                    definition = entitlement.Key,
                    expiry = entitlement.Value.permanent ? DateTime.MaxValue : DateTime.UtcNow.AddDays(entitlement.Value.days)
                };
                index++;
            }
            return entitles;
        }

        bool CheckInputValid()
        {
            if (string.IsNullOrEmpty(request))
            {
                Debug.LogError("The request content is null.");
                return false;
            }
            if (!permanent && days <= 0)
            {
                Debug.LogError($"The days value {days} for license is meaningless.");
                return false;
            }
            foreach (var entitlement in entitlements)
            {
                if (!entitlement.Value.permanent && entitlement.Value.days < 0)
                {
                    Debug.LogError($"The days value {entitlement.Value.days} for entitlement {entitlement.Key} is meaningless.");
                    return false;
                }
            }
            return true;
        }

        bool CheckSettingsValid(LicenseSettings settings)
        {
            if (settings == null)
            {
                Debug.LogError($"The LicenseSettings not found. Open the License Settings to create it.");
                return false;
            }
            if (string.IsNullOrEmpty(settings.publicKey))
            {
                Debug.LogError("The publicKey of LicenseSettings is null.");
                return false;
            }
            if (string.IsNullOrEmpty(settings.privateKey))
            {
                Debug.LogError("The privateKey of LicenseSettings is null.");
                return false;
            }
            if (string.IsNullOrEmpty(settings.version))
            {
                Debug.LogError("The version of LicenseSettings is null.");
                return false;
            }
            return true;
        }

        bool CheckRequestValid(RequestInfo info, LicenseSettings settings)
        {
            if (string.IsNullOrEmpty(info.deviceInfo.operatingSystem))
            {
                Debug.LogError("The operatingSystem of request is null.");
                return false;
            }

            if (string.IsNullOrEmpty(info.deviceInfo.processorType))
            {
                Debug.LogError("The processorType of request is null.");
                return false;
            }

            if (string.IsNullOrEmpty(info.deviceInfo.deviceModel))
            {
                Debug.LogError("The deviceModel of request is null.");
                return false;
            }

            if (info.productInfo.productName != Application.productName)
            {
                Debug.LogError($"The productName {info.productInfo.productName} of request miss match current product {Application.productName}");
                return false;
            }

            if (info.productInfo.supportedLicenseVersion != settings.version)
            {
                Debug.LogError($"The supportedLicenseVersion {info.productInfo.supportedLicenseVersion} of request miss match cuttent LicenseSettings version {settings.version}");
                return false;
            }

            return true;
        }

        void OnRequestEdit(string request)
        {
            if (request == this.request)
            {
                return;
            }
            this.request = request;
            var info = LicenseHub.FromRequestText(request);
            SetEntitlements(info.productInfo.entitlements);
        }

        void SetEntitlements(IEnumerable<string> items)
        {
            entitlements.Clear();
            if (items == null)
            {
                return;
            }
            foreach (var item in items)
            {
                if (string.IsNullOrEmpty(item))
                {
                    continue;
                }
                if (entitlements.ContainsKey(item))
                {
                    continue;
                }
                entitlements.Add(item, new Entitlement());
            }
        }
        #endregion

        #region
        string SignData(string data, string privateKey)
        {
            using var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(privateKey);

            var dataBytes = Encoding.UTF8.GetBytes(data);
            var signature = rsa.SignData(dataBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

            return Convert.ToBase64String(signature);
        }
        #endregion

        #region
        Vector2 requestPos;
        Vector2 licensePos;

        void DrawEditor()
        {
            DrawRequestArea();
            DrawBuildArea();
            DrawLicenseArea();
        }

        void DrawRequestArea()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Request");
            if (GUILayout.Button("Paste"))
            {
                OnRequestEdit(GUIUtility.systemCopyBuffer);
            }
            GUILayout.EndHorizontal();

            requestPos = GUILayout.BeginScrollView(requestPos, GUILayout.Height(120));
            OnRequestEdit(GUILayout.TextArea(request, GUILayout.ExpandHeight(true)));
            GUILayout.EndScrollView();
        }

        void DrawBuildArea()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Valid");
            GUILayout.FlexibleSpace();
            if (!permanent)
            {
                days = EditorGUILayout.IntField(days);
                GUILayout.Label("Days");
            }
            permanent = GUILayout.Toggle(permanent, "Permanent");
            GUILayout.EndHorizontal();

            DrawEntitlements();

            if (GUILayout.Button("Build"))
            {
                license = BuildLicense();
            }
        }

        void DrawLicenseArea()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("License");
            if (GUILayout.Button("Copy"))
            {
                GUIUtility.systemCopyBuffer = license;
            }
            GUILayout.EndHorizontal();

            licensePos = GUILayout.BeginScrollView(licensePos);
            license = GUILayout.TextArea(license, GUILayout.ExpandHeight(true));
            GUILayout.EndScrollView();
        }

        void DrawEntitlements()
        {
            if (entitlements.Count == 0)
            {
                return;
            }

            GUILayout.Label("Entitlements");
            foreach (var entitlement in entitlements)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Box(entitlement.Key);
                GUILayout.FlexibleSpace();
                if (!entitlement.Value.permanent)
                {
                    entitlement.Value.days = EditorGUILayout.IntField(entitlement.Value.days);
                    GUILayout.Label("Days");
                }
                entitlement.Value.permanent = GUILayout.Toggle(entitlement.Value.permanent, "Permanent");
                GUILayout.EndHorizontal();
            }
        }
        #endregion
    }
}