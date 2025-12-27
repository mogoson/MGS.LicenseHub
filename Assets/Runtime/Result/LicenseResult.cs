/*************************************************************************
 *  Copyright © 2025 Mogoson All rights reserved.
 *------------------------------------------------------------------------
 *  File         :  LicenseResult.cs
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
    public struct LicenseResult
    {
        public ResultCode code;
        public DateTime expiry;
        public EntitlementResult[] entitlements;

        public bool IsPermanent { get { return expiry == DateTime.MaxValue; } }
    }
}