/*************************************************************************
 *  Copyright © 2025 Mogoson All rights reserved.
 *------------------------------------------------------------------------
 *  File         :  ProductSettings.cs
 *  Description  :  Default.
 *------------------------------------------------------------------------
 *  Author       :  Mogoson
 *  Version      :  1.0.0
 *  Date         :  12/05/2025
 *  Description  :  Initial development version.
 *************************************************************************/

using UnityEngine;

namespace MGS.License
{
    class ProductSettings : ScriptableObject
    {
        public string licenseKey;
        public string supportedLicenseVersion;
        public long timestamp;
        public string[] entitlements;
    }
}