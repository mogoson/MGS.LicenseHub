/*************************************************************************
 *  Copyright © 2025 Mogoson All rights reserved.
 *------------------------------------------------------------------------
 *  File         :  LicenseHub.cs
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
using Newtonsoft.Json;
using UnityEngine;

namespace MGS.License
{
    public sealed class LicenseHub
    {
        #region Device
        static string ComputeDeviceID()
        {
            var info = GetDeviceInfo();
            return ComputeDeviceID(info);
        }

        internal static string ComputeDeviceID(DeviceInfo info)
        {
            var infoTex = $"{info.operatingSystem}{info.processorType}{info.deviceModel}";
            return ComputeHash(infoTex);
        }

        static DeviceInfo GetDeviceInfo()
        {
            return new DeviceInfo
            {
                operatingSystem = SystemInfo.operatingSystem,
                processorType = SystemInfo.processorType,
                deviceModel = SystemInfo.deviceModel
            };
        }
        #endregion

        #region Product
        static ProductSettings LoadProductSettings()
        {
            return Resources.Load<ProductSettings>(nameof(ProductSettings));
        }

        static ProductInfo GetProductInfo()
        {
            var settings = LoadProductSettings();
            return new ProductInfo
            {
                productName = Application.productName,
                productVersion = Application.version,
                supportedLicenseVersion = settings.supportedLicenseVersion,
                entitlements = settings.entitlements
            };
        }
        #endregion

        #region Request
        public static string GetRequestText()
        {
            var info = GetRequestInfo();
            var json = ToJson(info);
            return ToBase64String(json);
        }

        static RequestInfo GetRequestInfo()
        {
            var device = GetDeviceInfo();
            var product = GetProductInfo();
            return new RequestInfo
            {
                deviceInfo = device,
                productInfo = product
            };
        }

        internal static RequestInfo FromRequestText(string text)
        {
            var json = FromBase64String(text);
            return FromJson<RequestInfo>(json);
        }
        #endregion

        #region License
        const string KEY_LICENSE_INFO = "KEY_LICENSE_INFO";

        public static LicenseResult VerifyLicense()
        {
            var licenseText = PlayerPrefs.GetString(KEY_LICENSE_INFO);
            if (string.IsNullOrEmpty(licenseText))
            {
                return new LicenseResult() { code = ResultCode.None };
            }
            var settings = LoadProductSettings();
            return VerifyLicense(licenseText, settings);
        }

        static LicenseResult VerifyLicense(string licenseText, ProductSettings settings)
        {
            var licenseInfo = FromLicenseText(licenseText);
            return VerifyLicense(licenseInfo, settings);
        }

        static LicenseResult VerifyLicense(LicenseInfo info, ProductSettings settings)
        {
            var result = new LicenseResult();
            if (string.IsNullOrEmpty(info.content) || string.IsNullOrEmpty(info.signature))
            {
                result.code = ResultCode.ContentInvalid;
                return result;
            }

            var sign = VerifyData(info.content, info.signature, settings.licenseKey);
            if (!sign)
            {
                result.code = ResultCode.ContentInvalid;
                return result;
            }

            var license = FromJson<LicenseContent>(info.content);
            if (license.version != settings.supportedLicenseVersion)
            {
                result.code = ResultCode.VersionNotSupport;
                return result;
            }

            var device = ComputeDeviceID();
            if (license.device != device)
            {
                result.code = ResultCode.DeviceNotMatch;
                return result;
            }

            if (license.expiry < DateTime.MaxValue)
            {
                if (!VerifyTimestamp(settings))
                {
                    result.code = ResultCode.TimestampInvalid;
                    return result;
                }
                if (DateTime.UtcNow < license.creation)
                {
                    result.code = ResultCode.TimestampInvalid;
                    return result;
                }
                if (license.expiry < DateTime.UtcNow)
                {
                    result.code = ResultCode.Expiry;
                    return result;
                }
            }

            result.code = ResultCode.Valid;
            result.expiry = license.expiry;
            result.entitlements = VerifyEntitlements(license.entitlements, settings);
            return result;
        }

        static EntitlementResult[] VerifyEntitlements(IEnumerable<EntitlementInfo> entitlements, ProductSettings settings)
        {
            if (entitlements == null)
            {
                return null;
            }

            var entResults = new List<EntitlementResult>();
            foreach (var entitlement in entitlements)
            {
                var entCode = ResultCode.Valid;
                if (entitlement.expiry < DateTime.MaxValue)
                {
                    if (!VerifyTimestamp(settings))
                    {
                        entCode = ResultCode.TimestampInvalid;
                    }
                    else if (entitlement.expiry < DateTime.UtcNow)
                    {
                        entCode = ResultCode.Expiry;
                    }
                }
                var entResult = new EntitlementResult()
                {
                    code = entCode,
                    definition = entitlement.definition,
                    expiry = entitlement.expiry
                };
                entResults.Add(entResult);
            }
            return entResults.ToArray();
        }

        public static LicenseResult ActivateLicense(string licenseText)
        {
            if (string.IsNullOrEmpty(licenseText))
            {
                return new LicenseResult() { code = ResultCode.ContentInvalid };
            }

            var settings = LoadProductSettings();
            var result = VerifyLicense(licenseText, settings);
            if (result.code == ResultCode.Valid)
            {
                PlayerPrefs.SetString(KEY_LICENSE_INFO, licenseText);
            }
            return result;
        }

        static LicenseInfo FromLicenseText(string licenseText)
        {
            var json = FromBase64String(licenseText);
            return FromJson<LicenseInfo>(json);
        }

        internal static string ToLicenseText(LicenseInfo license)
        {
            var json = ToJson(license);
            return ToBase64String(json);
        }

        internal static void ClearLicense()
        {
            PlayerPrefs.DeleteKey(KEY_LICENSE_INFO);
        }
        #endregion

        #region Timestamp
        const string KEY_LICENSE_TIMESTAMP = "KEY_LICENSE_TIMESTAMP";

        static bool VerifyTimestamp(ProductSettings settings)
        {
            var text = PlayerPrefs.GetString(KEY_LICENSE_TIMESTAMP);
            if (string.IsNullOrEmpty(text))
            {
                var settingsTime = DateTime.FromBinary(settings.timestamp);
                if (DateTime.UtcNow < settingsTime)
                {
                    return false;
                }

                UpdateTimestamp(settings.licenseKey);
                return true;
            }

            var timestamp = FromTimestampText(text);
            if (!VerifyTimestamp(timestamp, settings.licenseKey))
            {
                return false;
            }

            if (DateTime.UtcNow < timestamp.dateTime)
            {
                return false;
            }

            UpdateTimestamp(settings.licenseKey);
            return true;
        }

        static bool VerifyTimestamp(TimestampInfo info, string key)
        {
            return VerifyHash(info.dateTime.ToString(), key, info.signature);
        }

        static TimestampInfo FromTimestampText(string text)
        {
            var json = FromBase64String(text);
            return FromJson<TimestampInfo>(json);
        }

        static string ToTimestampText(DateTime time, string key)
        {
            var timestamp = new TimestampInfo
            {
                dateTime = time,
                signature = ComputeHash(time.ToString(), key)
            };
            var json = ToJson(timestamp);
            return ToBase64String(json);
        }

        static void UpdateTimestamp(string key)
        {
            var timestamp = ToTimestampText(DateTime.UtcNow, key);
            PlayerPrefs.SetString(KEY_LICENSE_TIMESTAMP, timestamp);
        }

        internal static void ClearTimestamp()
        {
            PlayerPrefs.DeleteKey(KEY_LICENSE_TIMESTAMP);
        }
        #endregion

        #region Json
        internal static T FromJson<T>(string json)
        {
            try { return JsonConvert.DeserializeObject<T>(json); }
            catch { }
            return default;
        }

        internal static string ToJson(object value)
        {
            try { return JsonConvert.SerializeObject(value); }
            catch { }
            return null;
        }
        #endregion

        #region Base64
        internal static string ToBase64String(string text)
        {
            try
            {
                var bytes = Encoding.UTF8.GetBytes(text);
                return Convert.ToBase64String(bytes);
            }
            catch { }
            return null;
        }

        internal static string FromBase64String(string base64Str)
        {
            try
            {
                var bytes = Convert.FromBase64String(base64Str);
                return Encoding.UTF8.GetString(bytes);
            }
            catch { }
            return null;
        }
        #endregion

        #region Hash
        static string ComputeHash(string data)
        {
            var hash128 = Hash128.Compute(data);
            return hash128.ToString();
        }

        static string ComputeHash(string data, string key)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var dataBytes = Encoding.UTF8.GetBytes(data);

            using var sha256 = new HMACSHA256(keyBytes);
            var hashBytes = sha256.ComputeHash(dataBytes);
            return Convert.ToBase64String(hashBytes);
        }

        static bool VerifyHash(string data, string key, string dataHash)
        {
            var hash = ComputeHash(data, key);
            return hash == dataHash;
        }
        #endregion

        #region RSA
        static bool VerifyData(string data, string signature, string publicKey)
        {
            using var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(publicKey);

            var dataBytes = Encoding.UTF8.GetBytes(data);
            var signatureBytes = Convert.FromBase64String(signature);

            return rsa.VerifyData(dataBytes, signatureBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }
        #endregion
    }
}