/*************************************************************************
 *  Copyright © 2025 Mogoson All rights reserved.
 *------------------------------------------------------------------------
 *  File         :  EntitlementInfo.cs
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
    struct EntitlementInfo
    {
        public string definition;
        public DateTime expiry;
    }
}