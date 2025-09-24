using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace PWRISimulator
{
    /// <summary>
    /// �m�F�_�C�A���O����
    /// </summary>
    public class ConfirmDialog : MonoBehaviour
    {
        private void OnEnable()
        {
            var root = this.GetComponent<UIDocument>().rootVisualElement;

            var closeButton = root.Q<Button>("Cancel");
            if (closeButton != null)
            {
                closeButton.clicked += () =>
                {
                    GlobalVariables.ConfirmWaitFlag = 0;

                    // �E�B���h�E�����
                    Destroy(this.gameObject); // Destroy the target GameObject
                };
            }

            var okButton = root.Q<Button>("OK");
            if (okButton != null)
            {
                okButton.clicked += () =>
                {
                    GlobalVariables.ConfirmWaitFlag = 2;

                    // �E�B���h�E�����
                    Destroy(this.gameObject); // Destroy the target GameObject
                };
            }
        }
    }
}
