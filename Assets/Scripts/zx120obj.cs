using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using Debug = UnityEngine.Debug;

namespace PWRISimulator
{
    public class zx120obj
    {
            public bool Spawn_zx120(Vector3 pos, Quaternion quat, int spawnID, String zx120_path)
            {

                GameObject zx120_prefab = Resources.Load<GameObject>(zx120_path);

                GameObject zx120_pref = (GameObject)UnityEngine.Object.Instantiate(zx120_prefab,
                                                                  pos,
                                                                  quat);

                zx120_pref.name = "zx120_" + spawnID.ToString();

                return true;

            }

            public bool ReSpawn_zx120(GameObject obj, String zx120_path)
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

                Debug.Log(zx120_path);
                Debug.Log(pos);
                Debug.Log(quat);
                Debug.Log(objName);

                GameObject zx120_prefab = Resources.Load<GameObject>(zx120_path);

                GameObject zx120_pref = (GameObject)UnityEngine.Object.Instantiate(zx120_prefab,
                                                                  pos,
                                                                  quat);

                zx120_pref.name = objName;


                return true;

            }

    }
}
