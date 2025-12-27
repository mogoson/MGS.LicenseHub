/*************************************************************************
 *  Copyright © 2025 Mogoson All rights reserved.
 *------------------------------------------------------------------------
 *  File         :  LicenseActivateEditor.cs
 *  Description  :  Default.
 *------------------------------------------------------------------------
 *  Author       :  Mogoson
 *  Version      :  1.0.0
 *  Date         :  12/27/2025
 *  Description  :  Initial development version.
 *************************************************************************/

using UnityEditor;
using UnityEngine;

namespace MGS.License.Editors
{
    class LicenseActivateEditor
    {
        [MenuItem("Tools/License/Clear", priority = 2)]
        static void LicenseClear()
        {
            var message = "Are you sure clear the license activate information?";
            var isClear = EditorUtility.DisplayDialog("License Clear", message, "Clear", "Cancel");
            if (isClear)
            {
                LicenseHub.ClearLicense();
                Debug.Log("The license activate information has been cleared from current device.");
            }
        }
    }
}