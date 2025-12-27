/*************************************************************************
 *  Copyright © 2025 Mogoson All rights reserved.
 *------------------------------------------------------------------------
 *  File         :  ProductInfo.cs
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
    struct ProductInfo
    {
        public string productName;
        public string productVersion;
        public string supportedLicenseVersion;
        public string[] entitlements;
    }
}