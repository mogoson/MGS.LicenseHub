/*************************************************************************
 *  Copyright © 2025 Mogoson All rights reserved.
 *------------------------------------------------------------------------
 *  File         :  LicenseUI.cs
 *  Description  :  Default.
 *------------------------------------------------------------------------
 *  Author       :  Mogoson
 *  Version      :  1.0.0
 *  Date         :  11/30/2025
 *  Description  :  Initial development version.
 *************************************************************************/

using System;
using UnityEngine;

namespace MGS.License.UI
{
    public class LicenseUI : MonoBehaviour
    {
        #region
        public ActivatePanel activatePanel;
        public MessagePanel messagePanel;

        protected virtual void Awake()
        {
            //Do not verify license during trial.
            var trialExpiry = new DateTime(2025, 12, 27);
            if (DateTime.UtcNow <= trialExpiry)
            {
                Close();
                return;
            }
            var result = LicenseHub.VerifyLicense();
            OnVerifyResult(result);
        }

        void OnVerifyResult(LicenseResult result)
        {
            if (result.code == ResultCode.Valid)
            {
                OnVerifyValid(result);
                return;
            }
            OnVerifyInvalid(result.code);
        }
        #endregion

        #region
        protected virtual void OnVerifyValid(LicenseResult result)
        {
            CheckEntitlements(result);
            if (!CheckWillExpire(result))
            {
                Close();
            }
        }

        protected void CheckEntitlements(LicenseResult result)
        {
            var valid = result.IsPermanent ? "Permanent" : $"{result.expiry}";
            Debug.Log($"License {result.code} {valid}");

            if (result.entitlements != null)
            {
                foreach (var entitlement in result.entitlements)
                {
                    CheckEntitlement(entitlement);
                }
            }
        }

        protected void CheckEntitlement(EntitlementResult result)
        {
            var valid = result.IsPermanent ? "Permanent" : $"{result.expiry}";
            Debug.Log($"Entitlement {result.definition} {result.code} {valid}");
#if false
            //Use entitlement to do more limit,
            //example professional version features, and other features.
            switch (result.definition)
            {
                case EntitlementDefine.SYSTEM_SKIN_PRO:
                    {
                        if (result.code == ResultCode.Valid)
                        {
                            //Set system skin to pro mode...
                        }
                        else
                        {
                            //Set system skin to normal mode and show entitlement invalid info...
                        }
                    }
                    break;
            }
#endif
        }

        protected bool CheckWillExpire(LicenseResult result)
        {
            if (!result.IsPermanent)
            {
                var totalDays = (result.expiry - DateTime.UtcNow).TotalDays;
                if (totalDays < 15)
                {
                    ShowWillExpireDialog((int)totalDays);
                    return true;
                }
            }
            return false;
        }

        protected void ShowWillExpireDialog(int days)
        {
            var message = $"Your license will expire in {days} days, please activate soon.";
            messagePanel.Show("License", message, "Next Time", "Activate Now", activateNow =>
            {
                if (activateNow)
                {
                    ShowCancelOrActivatePanel();
                    return;
                }
                Close();
            });
        }

        protected void ShowCancelOrActivatePanel()
        {
            var requestText = LicenseHub.GetRequestText();
            activatePanel.Show(requestText, "Cancel", "Activate", OnCancelOrActivate);
        }

        protected void OnCancelOrActivate(bool activate, string licenseText)
        {
            if (activate)
            {
                var result = LicenseHub.ActivateLicense(licenseText);
                OnActivateResult(result);
                return;
            }
            Close();
        }
        #endregion

        #region
        protected virtual void OnVerifyInvalid(ResultCode code)
        {
            var message = ResolveMessage(code);
            ShowQuiteOrActivateDialog(message);
        }

        protected virtual string ResolveMessage(ResultCode code)
        {
            var tip = "Your license is invalid";
            var guide = "please enter a valid license to activate.";
            switch (code)
            {
                case ResultCode.None:
                    tip = "Your app is not activated";
                    break;

                case ResultCode.ContentInvalid:
                    tip = "Your license content is invalid";
                    break;

                case ResultCode.VersionNotSupport:
                    tip = "Your license version is not supported";
                    break;

                case ResultCode.DeviceNotMatch:
                    tip = "Your license dose not match your device";
                    break;

                case ResultCode.TimestampInvalid:
                    tip = "Your system time is invalid";
                    guide = "please require correct system time and retry.";
                    break;

                case ResultCode.Expiry:
                    tip = "Your license is expiry";
                    break;
            }
            return $"{tip}, {guide}";
        }

        protected void ShowQuiteOrActivateDialog(string message)
        {
            messagePanel.Show("License", message, "Quite", "Activate", activateNow =>
            {
                if (activateNow)
                {
                    ShowQuiteOrActivatePanel();
                    return;
                }
                Quit();
            });
        }

        protected void ShowQuiteOrActivatePanel()
        {
            var requestText = LicenseHub.GetRequestText();
            activatePanel.Show(requestText, "Quite", "Activate", OnQuiteOrActivate);
        }

        protected void OnQuiteOrActivate(bool activate, string licenseText)
        {
            if (activate)
            {
                var result = LicenseHub.ActivateLicense(licenseText);
                OnActivateResult(result);
                return;
            }
            Quit();
        }
        #endregion

        #region
        void OnActivateResult(LicenseResult result)
        {
            if (result.code == ResultCode.Valid)
            {
                OnActivateValid(result);
                return;
            }
            OnActivateInvalid(result.code);
        }

        protected virtual void OnActivateValid(LicenseResult result)
        {
            CheckEntitlements(result);
            var message = "Your app has been activated, thank you for your support.";
            ShowOKDialog(message, ok => Close());
        }

        protected virtual void OnActivateInvalid(ResultCode code)
        {
            var message = ResolveMessage(code);
            ShowOKDialog(message);
        }

        protected void ShowOKDialog(string message, Action<bool> onSelected = null)
        {
            messagePanel.Show("License", message, null, "OK", onSelected);
        }
        #endregion

        #region
        protected void Close()
        {
            Destroy(gameObject);
        }

        protected void Quit()
        {
            Application.Quit();
        }
        #endregion
    }
}