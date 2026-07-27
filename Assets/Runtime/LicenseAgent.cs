/*************************************************************************
 *  Copyright © 2026 Mogoson All rights reserved.
 *------------------------------------------------------------------------
 *  File         :  LicenseAgent.cs
 *  Description  :  Default.
 *------------------------------------------------------------------------
 *  Author       :  Mogoson
 *  Version      :  1.0.0
 *  Date         :  07/27/2026
 *  Description  :  Initial development version.
 *************************************************************************/

using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace MGS.License
{
    public sealed class LicenseAgent
    {
        #region
        public static LicenseResult VerifyLicense()
        {
            return LicenseHub.VerifyLicense();
        }

        public static IEnumerator VerifyLicense(Action<LicenseResult> finished)
        {
            var result = VerifyLicense();
            if (result.code != ResultCode.Valid)
            {
                CreateRequestFile();
                var license = string.Empty;
                yield return RequestLicense(GetLicenseFile(), tex => license = tex);
                if (!string.IsNullOrEmpty(license))
                {
                    result = ActivateLicense(license);
                }
            }
            finished?.Invoke(result);
        }

        static string GetLicenseFile()
        {
            var fileName = $"{Application.productName}.lic";
            var filePath = $"{Application.persistentDataPath}/{fileName}";
            if (!File.Exists(filePath))
            {
                filePath = $"{Application.streamingAssetsPath}/{fileName}";
            }
            return filePath;
        }
        #endregion

        #region
        public static void CreateRequestFile()
        {
            CreateRequestFile($"{Application.persistentDataPath}/{Application.productName}.lre");
        }

        public static void CreateRequestFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                try
                {
                    var dir = Path.GetDirectoryName(filePath);
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                    File.WriteAllText(filePath, GetRequestText());
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
        }

        public static string GetRequestText()
        {
            return LicenseHub.GetRequestText();
        }
        #endregion

        #region
        public static LicenseResult ActivateLicense(string license)
        {
            return LicenseHub.ActivateLicense(license);
        }

        public static IEnumerator ActivateLicense(string uri, Action<LicenseResult> finished)
        {
            var license = string.Empty;
            yield return RequestLicense(uri, tex => license = tex);
            finished?.Invoke(ActivateLicense(license));
        }

        public static IEnumerator RequestLicense(string uri, Action<string> finished)
        {
            var request = UnityWebRequest.Get(uri);
            yield return request.SendWebRequest();
            if (!string.IsNullOrEmpty(request.error))
            {
                Debug.LogError(request.error);
            }
            finished?.Invoke(request.downloadHandler.text);
        }
        #endregion
    }
}