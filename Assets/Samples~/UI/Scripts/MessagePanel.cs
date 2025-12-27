/*************************************************************************
 *  Copyright © 2025 Mogoson All rights reserved.
 *------------------------------------------------------------------------
 *  File         :  MessagePanel.cs
 *  Description  :  Default.
 *------------------------------------------------------------------------
 *  Author       :  Mogoson
 *  Version      :  1.0.0
 *  Date         :  11/30/2025
 *  Description  :  Initial development version.
 *************************************************************************/

using System;
using UnityEngine;
using UnityEngine.UI;

namespace MGS.License.UI
{
    public class MessagePanel : MonoBehaviour
    {
        public Text texTittle;
        public Text texMessage;
        public Button btnCancel;
        public Text texCancel;
        public Button btnOK;
        public Text texOK;

        protected Action<bool> onSelected;

        protected virtual void Awake()
        {
            btnCancel.onClick.AddListener(OnCancelBtnClick);
            btnOK.onClick.AddListener(OnOKBtnClick);
        }

        void OnCancelBtnClick()
        {
            onSelected?.Invoke(false);
            Close();
        }

        void OnOKBtnClick()
        {
            onSelected?.Invoke(true);
            Close();
        }

        public virtual void Show(string tittle, string message, string cancel, string ok, Action<bool> onSelected)
        {
            this.onSelected = onSelected;

            texTittle.text = tittle;
            texMessage.text = message;

            texCancel.text = cancel;
            btnCancel.gameObject.SetActive(!string.IsNullOrEmpty(cancel));

            texOK.text = ok;
            btnOK.gameObject.SetActive(!string.IsNullOrEmpty(ok));

            gameObject.SetActive(true);
        }

        public virtual void Close()
        {
            gameObject.SetActive(false);
            onSelected = null;
        }
    }
}