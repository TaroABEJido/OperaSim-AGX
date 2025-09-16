using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.UIElements;

namespace PWRISimulator
{
    /// <summary>
    /// èÛë‘ï\é¶èàóù
    /// ÉQÅ[ÉÄâÊñ è„ïîÇ…ï\é¶
    /// </summary>
    public class StatusBoard : MonoBehaviour
    {

        private UIDocument _uiStatusBoardDocument;
        private GameObject StatusBoardUIobj;

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

        public void SetStatusMessage(string smessage)
        {
            var root = this.GetComponent<UIDocument>().rootVisualElement;
            var sLabel = root.Q<UnityEngine.UIElements.Label>("StatusLabel");
            sLabel.text = smessage;
        }


        public void DeleteStatusMessage()
        {
            var root = this.GetComponent<UIDocument>().rootVisualElement;
            var sLabel = root.Q<UnityEngine.UIElements.Label>("StatusLabel");
            sLabel.text = "";

        }

        public void SetMessageColor(Color color)
        {
            var root = this.GetComponent<UIDocument>().rootVisualElement;
            var sLabel = root.Q<UnityEngine.UIElements.Label>("StatusLabel");
            sLabel.style.color = color;
        }



    }
}
