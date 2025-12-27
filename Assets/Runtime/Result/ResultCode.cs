/*************************************************************************
 *  Copyright © 2025 Mogoson All rights reserved.
 *------------------------------------------------------------------------
 *  File         :  ResultCode.cs
 *  Description  :  Default.
 *------------------------------------------------------------------------
 *  Author       :  Mogoson
 *  Version      :  1.0.0
 *  Date         :  12/05/2025
 *  Description  :  Initial development version.
 *************************************************************************/

namespace MGS.License
{
    public enum ResultCode
    {
        None,
        ContentInvalid,
        VersionNotSupport,
        DeviceNotMatch,
        TimestampInvalid,
        Expiry,
        Valid
    }
}