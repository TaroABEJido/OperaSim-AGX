using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace PWRISimulator
{
    public class ConfirmDialog : MonoBehaviour
    {
        public static GameObject targetObject;

        private void OnEnable()
        {
            var root = this.GetComponent<UIDocument>().rootVisualElement;

            var closeButton = root.Q<Button>("Cancel");
            if (closeButton != null)
            {
                closeButton.clicked += () =>
                {
                    Destroy(this.gameObject); // Destroy the target GameObject
                };
            }

            var okButton = root.Q<Button>("OK");
            if (okButton != null)
            {
                okButton.clicked += () =>
                {
                    if (targetObject.name.Contains("ic120_") == true)
                    {
                        GlobalVariables.ic120Counter = GlobalVariables.ic120Counter - 1;

                        int length = targetObject.name.IndexOf("ic120_");
                        string id = targetObject.name.Substring(0, length);
                        GlobalVariables.Dump_IDList.Remove(id);

                        Destroy(targetObject);
                        GameObject objMassBody = GameObject.Find(targetObject.name + "_SoilMassBody");
                        if (objMassBody != null) Destroy(objMassBody);
                        GameObject objMassJoint = GameObject.Find(targetObject.name + "_SoilMassJoint");
                        if (objMassJoint != null) Destroy(objMassJoint);

                    }
                    else if (targetObject.name.Contains("Camera_") == true)
                    {
                        Destroy(targetObject);
                        GlobalVariables.CameraCounter = GlobalVariables.CameraCounter - 1;
                    }

                    Destroy(this.gameObject); // Destroy the target GameObject
                };
            }
        }


        //// Start is called before the first frame update
        //void Start()
        //{

        //}

        //// Update is called once per frame
        //void Update()
        //{

        //}
    }
}
