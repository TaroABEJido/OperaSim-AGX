using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using Debug = UnityEngine.Debug;

namespace PWRISimulator
{
    /// <summary>
    /// ダンプクローラオブジェクト生成処理
    /// </summary>
    public class ic120obj
    {
        public bool Spawn_ic120(Vector3 pos, Quaternion quat, int spawnID, String ic120_path)
        {

            GameObject ic120_prefab = Resources.Load<GameObject>(ic120_path);

            GameObject ic120_pref = (GameObject)UnityEngine.Object.Instantiate(ic120_prefab,
                                                              pos,
                                                              quat);

            ic120_pref.name = "ic120_" + spawnID.ToString();

            // リストを更新
            GlobalVariables.Dump_IDList.Add(spawnID.ToString());
            GlobalVariables.Dump_ObjList.Add(ic120_pref);

            Debug.Log("Dump_IDList.Count: " + GlobalVariables.Dump_IDList.Count);
            Debug.Log("Dump_ObjList.Count: " + GlobalVariables.Dump_ObjList.Count);


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


            int length = ic120_pref.name.LastIndexOf("_");
            string id = ic120_pref.name.Substring(length + 1);

            // リストを更新
            GlobalVariables.Dump_IDList.Add(id);
            GlobalVariables.Dump_ObjList.Add(ic120_pref);


            return true;

        }

    }
}
