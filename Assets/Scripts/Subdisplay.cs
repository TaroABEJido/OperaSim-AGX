using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UIElements;

namespace PWRISimulator
{
    public class Subdisplay : MonoBehaviour
    {

        private VisualElement renderTextureElement;

        // Start is called before the first frame update
        void Start()
        {

            VisualElement root = this.GetComponent<UIDocument>().rootVisualElement;

            renderTextureElement = root.Q<VisualElement>("display");

        }

        // Update is called once per frame
        //void Update()
        //{
        //
        //}

        public void SetDisplay(Camera camera)
        {
            //RenderTexture renderTexture = camera.targetTexture;

            VisualElement root = this.GetComponent<UIDocument>().rootVisualElement;

            //UnityEngine.Debug.Log(root.Q<VisualElement>("display").resolvedStyle.width);
            //UnityEngine.Debug.Log(root.Q<VisualElement>("display").resolvedStyle.height);

            var width = root.Q<VisualElement>("display").resolvedStyle.width;
            var height = root.Q<VisualElement>("display").resolvedStyle.height;

            RenderTexture cameraTexture = new RenderTexture((int)width, (int)height, 24);
            camera.enabled = true;
            camera.targetTexture = cameraTexture;

            renderTextureElement.style.backgroundImage = new StyleBackground(Background.FromRenderTexture(camera.targetTexture));

            UnityEngine.Debug.Log("SetDisplay");

        }


    }
}
