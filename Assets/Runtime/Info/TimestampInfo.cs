/*************************************************************************
 *  Copyright © 2025 Mogoson All rights reserved.
 *------------------------------------------------------------------------
 *  File         :  TimestampInfo.cs
 *  Description  :  Default.
 *------------------------------------------------------------------------
 *  Author       :  Mogoson
 *  Version      :  1.0.0
 *  Date         :  12/06/2025
 *  Description  :  Initial development version.
 *************************************************************************/

using System;

namespace MGS.License
{
    [Serializable]
    struct TimestampInfo
    {
        public DateTime dateTime;
        public string signature;
    }
}