using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

using Debug = UnityEngine.Debug;

namespace PWRISimulator
{
    public class GetPosition : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            if (GlobalVariables.ActionMode == 3) {
                Debug.Log("**** play mode *****");

                if (GlobalVariables.Dump_ObjList.Count > 0)
                {
                    for (int i = 0; i < GlobalVariables.Dump_ObjList.Count; i++)
                    {
                        GameObject obj_dump = GlobalVariables.Dump_ObjList[i];
                        Vector3 tmp = obj_dump.transform.position;
                        Debug.Log("ic120 id= " + i + ", x= " + tmp.x + ", y= " + tmp.y + ", z= " + tmp.z);
                    }
                }
            }
        }
    }
}
