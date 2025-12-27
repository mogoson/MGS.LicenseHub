/*************************************************************************
 *  Copyright © 2025 Mogoson All rights reserved.
 *------------------------------------------------------------------------
 *  File         :  ActivatePanel.cs
 *  Description  :  Default.
 *------------------------------------------------------------------------
 *  Author       :  Mogoson
 *  Version      :  1.0.0
 *  Date         :  12/06/2025
 *  Description  :  Initial development version.
 *************************************************************************/

using System;
using UnityEngine;
using UnityEngine.UI;

namespace MGS.License.UI
{
    public class ActivatePanel : MonoBehaviour
    {
        public Text texRequest;
        public Button btnCopy;
        public InputField iptLicense;
        public Button btnPaste;
        public Button btnCancel;
        public Text texCancel;
        public Button btnActivate;
        public Text texActivate;
        protected Action<bool, string> onSelected;

        protected virtual void Awake()
        {
            btnCopy.onClick.AddListener(OnCopyBtnClick);
            btnPaste.onClick.AddListener(OnPasteBtnClick);
            btnCancel.onClick.AddListener(OnCancelBtnClick);
            btnActivate.onClick.AddListener(OnActivateBtnClick);
        }

        void OnCopyBtnClick()
        {
            GUIUtility.systemCopyBuffer = texRequest.text;
        }

        void OnPasteBtnClick()
        {
            iptLicense.text = GUIUtility.systemCopyBuffer;
        }

        void OnCancelBtnClick()
        {
            onSelected?.Invoke(false, null);
            Close();
        }

        void OnActivateBtnClick()
        {
            onSelected?.Invoke(true, iptLicense.text);
        }

        public virtual void Show(string requestText, string cancel, string activate, Action<bool, string> onSelected)
        {
            this.onSelected = onSelected;

            texRequest.text = requestText;
            texCancel.text = cancel;
            btnCancel.gameObject.SetActive(!string.IsNullOrEmpty(cancel));

            texActivate.text = activate;
            btnActivate.gameObject.SetActive(!string.IsNullOrEmpty(activate));

            gameObject.SetActive(true);
        }

        public virtual void Close()
        {
            gameObject.SetActive(false);
            onSelected = null;
        }
    }
}