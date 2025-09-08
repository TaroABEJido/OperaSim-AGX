using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace PWRISimulator
{
    public class MessageDialog : MonoBehaviour
    {


        private void OnEnable()
        {
            var root = this.GetComponent<UIDocument>().rootVisualElement;

            var closeButton = root.Q<Button>("Accept");

            if (closeButton != null)
            {
                closeButton.clicked += () =>
                {
                        Destroy(this.gameObject); // Destroy the target GameObject
                };
            }

        }


        // Start is called before the first frame update
        //void Start()
        //{
        //
        //}

        // Update is called once per frame
        //void Update()
        //{
        //
        //}
    }
}
