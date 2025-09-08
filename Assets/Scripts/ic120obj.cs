using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using Debug = UnityEngine.Debug;

namespace PWRISimulator
{
    public class ic120obj
    {
            public bool Spawn_ic120(Vector3 pos, Quaternion quat, int spawnID, String ic120_path)
            {

                GameObject ic120_prefab = Resources.Load<GameObject>(ic120_path);

                GameObject ic120_pref = (GameObject)UnityEngine.Object.Instantiate(ic120_prefab,
                                                                  pos,
                                                                  quat);

                ic120_pref.name = "ic120_" + spawnID.ToString();

                // add 202507
                // IDÇï€éù
                GlobalVariables.Dump_IDList.Add(spawnID.ToString());


            var parentAndChildren = ic120_pref.transform.GetComponentsInChildren<Camera>(true);

            foreach (Camera childCamera in parentAndChildren)
            {
                //Debug.Log(childtrnsform.gameObject.name);
                //if (childtrnsform.gameObject.name == "Camera")
                //{
                //childtrnsform.enabled = false;
                //}
                childCamera.gameObject.SetActive(false);

            }

            return true;

            }

            public bool ReSpawn_ic120(GameObject obj, String ic120_path)
            {
                if (obj == null) { return false; }

                Quaternion quat = obj.transform.rotation;
                Vector3 pos = obj.transform.position;
                String objName = obj.name;

                UnityEngine.Object.Destroy(obj);

                GameObject DSSMJ = GameObject.Find(objName + "_SoilMassJoint");
                Debug.Log(DSSMJ);
                UnityEngine.Object.Destroy(DSSMJ);
                GameObject DSSMB = GameObject.Find(objName + "_SoilMassBody");
                Debug.Log(DSSMB);
                UnityEngine.Object.Destroy(DSSMB);




                Debug.Log(ic120_path);
                Debug.Log(pos);
                Debug.Log(quat);
                Debug.Log(objName);

                GameObject ic120_prefab = Resources.Load<GameObject>(ic120_path);

                GameObject ic120_pref = (GameObject)UnityEngine.Object.Instantiate(ic120_prefab,
                                                                  pos,
                                                                  quat);

                ic120_pref.name = objName;


            var parentAndChildren = ic120_pref.transform.GetComponentsInChildren<Camera>(true);

            foreach (Camera childCamera in parentAndChildren)
            {
                //Debug.Log(childtrnsform.gameObject.name);
                //if (childtrnsform.gameObject.name == "Camera")
                //{
                //childtrnsform.enabled = false;
                //}
                childCamera.gameObject.SetActive(false);
            }

            return true;

            }

    }
}
