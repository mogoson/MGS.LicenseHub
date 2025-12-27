/*************************************************************************
 *  Copyright © 2025 Mogoson All rights reserved.
 *------------------------------------------------------------------------
 *  File         :  EntitlementResult.cs
 *  Description  :  Default.
 *------------------------------------------------------------------------
 *  Author       :  Mogoson
 *  Version      :  1.0.0
 *  Date         :  12/27/2025
 *  Description  :  Initial development version.
 *************************************************************************/

using System;

namespace MGS.License
{
    public struct EntitlementResult
    {
        public ResultCode code;
        public string definition;
        public DateTime expiry;

        public bool IsPermanent { get { return expiry == DateTime.MaxValue; } }
    }
}