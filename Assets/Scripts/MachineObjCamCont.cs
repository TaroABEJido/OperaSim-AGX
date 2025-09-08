
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PWRISimulator
{
    public class MachineObjCamCont 
    {

        public String MachineCamContPath = "Prefabs/MachineCamCont_Variant";
        public GameObject MachineCamContUI;
        private GameObject MachineCamContUIobj;

        public void machineSelected(String MachineName)
        {

            //machineDeselected();

            if (MachineCamContUIobj == null)
            {
                MachineCamContUI = Resources.Load<GameObject>(MachineCamContPath);
                MachineCamContUIobj = UnityEngine.Object.Instantiate(MachineCamContUI);
                MachineCamContUIobj.name = MachineName + "_ControlForMachineCamera";
                Debug.Log(MachineCamContUIobj.name);
                MachineCamControl script = MachineCamContUIobj.GetComponent<MachineCamControl>();
                var obj = GameObject.Find(MachineName);
                script.Initialize(obj);
            }
            else
            {
                Debug.Log("MachineCamContUIobj Not NULL");
            }

        }

        public void machineDeselected()
        {

            GameObject[] allObjects = UnityEngine.GameObject.FindObjectsOfType<GameObject>();

            foreach (GameObject obj in allObjects)
            {
                if (obj.name.Contains("_ControlForMachineCamera"))
                {
                    MachineCamControl script = obj.GetComponent<MachineCamControl>();
                    script.ClearCallBack();
                    UnityEngine.Object.Destroy(obj);   
                }
            }
            MachineCamContUIobj = null;

        }

    }
}
