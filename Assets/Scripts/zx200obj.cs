using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using Debug = UnityEngine.Debug;

namespace PWRISimulator
{
    public class zx200obj
    {
            public bool Spawn_zx200(Vector3 pos, Quaternion quat, int spawnID, String zx200_path)
            {

                GameObject zx200_prefab = Resources.Load<GameObject>(zx200_path);

                GameObject zx200_pref = (GameObject)UnityEngine.Object.Instantiate(zx200_prefab,
                                                                  pos,
                                                                  quat);

                zx200_pref.name = "zx200_" + spawnID.ToString();

                return true;

            }

            public bool ReSpawn_zx200(GameObject obj, String zx200_path)
            {
                if (obj == null) { return false; }

                Quaternion quat = obj.transform.rotation;
                Vector3 pos = obj.transform.position;
                String objName = obj.name;

                UnityEngine.Object.Destroy(obj);

                //GameObject DSSMJ = GameObject.Find(objName + "_SoilMassJoint");
                //Debug.Log(DSSMJ);
                //UnityEngine.Object.Destroy(DSSMJ);
                //GameObject DSSMB = GameObject.Find(objName + "_SoilMassBody");
                //Debug.Log(DSSMB);
                //UnityEngine.Object.Destroy(DSSMB);

                Debug.Log(zx200_path);
                Debug.Log(pos);
                Debug.Log(quat);
                Debug.Log(objName);

                GameObject zx200_prefab = Resources.Load<GameObject>(zx200_path);

                GameObject zx200_pref = (GameObject)UnityEngine.Object.Instantiate(zx200_prefab,
                                                                  pos,
                                                                  quat);

                zx200_pref.name = objName;


                return true;

            }

    }
}
