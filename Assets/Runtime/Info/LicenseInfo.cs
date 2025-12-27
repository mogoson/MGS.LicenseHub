/*************************************************************************
 *  Copyright © 2025 Mogoson All rights reserved.
 *------------------------------------------------------------------------
 *  File         :  LicenseInfo.cs
 *  Description  :  Default.
 *------------------------------------------------------------------------
 *  Author       :  Mogoson
 *  Version      :  1.0.0
 *  Date         :  12/05/2025
 *  Description  :  Initial development version.
 *************************************************************************/

using System;

namespace MGS.License
{
    [Serializable]
    struct LicenseInfo
    {
        public string content;
        public string signature;
    }

    [Serializable]
    struct LicenseContent
    {
        public string version;
        public string device;
        public DateTime creation;
        public DateTime expiry;
        public EntitlementInfo[] entitlements;
    }
}