using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using Debug = UnityEngine.Debug;

using UnityEngine.Rendering;

namespace PWRISimulator
{
    public class cameraObj
    {
        public RenderTexture cameraTexture;

        public bool Spawn_Camera(Vector3 pos, Quaternion quat, int spawnID, String camera_path)
        {

            GameObject camera_prefab = Resources.Load<GameObject>(camera_path);

            GameObject camera_pref = (GameObject)UnityEngine.Object.Instantiate(camera_prefab,
                                                                  pos,
                                                                  quat);

            camera_pref.name = "Camera_" + spawnID.ToString();

            //ActivateAllDisplays();

            //camera_pref.transform.Find("Camera").gameObject.GetComponent< Camera >().rect = new Rect(0.9f,0.0f,0.1f,0.1f);
            //camera_pref.transform.Find("Camera").gameObject.GetComponent<Camera>().enabled = true;
            camera_pref.transform.Find("CameraStr").gameObject.GetComponent<Camera>().enabled = true;
            camera_pref.transform.Find("CameraStr").Find("Camera").gameObject.GetComponent<Camera>().enabled = true;


            int width = Screen.width;
            int height = Screen.height;

            cameraTexture = new RenderTexture(width, height, 24);

            Camera camStr = camera_pref.transform.Find("CameraStr").gameObject.GetComponent<Camera>();
            camStr.targetTexture = cameraTexture;

            camStr.GetComponent<Unity.RenderStreaming.VideoStreamSender>().source = Unity.RenderStreaming.VideoStreamSource.Texture;
            camStr.GetComponent<Unity.RenderStreaming.VideoStreamSender>().sourceTexture = cameraTexture;


            //Debug.Log("ディスプレイの数: " + Display.displays.Length);

            //RenderTexture cameraTexture = new RenderTexture(160, 90, 24);
            //camera_pref.transform.Find("Camera").gameObject.GetComponent<Camera>().targetTexture = cameraTexture;

            var subdisp = GameObject.Find("SubdisplayForSpawnCamera");
            if (subdisp != null) {
                Debug.Log("test");
                //subdisp.GetComponent<Subdisplay>().SetDisplay(camera_pref.transform.Find("Camera").gameObject.GetComponent<Camera>());
                subdisp.GetComponent<Subdisplay>().SetDisplay(camera_pref.transform.Find("CameraStr").Find("Camera").gameObject.GetComponent<Camera>());
            }

            return true;

        }



        public bool ReSpawn_Camera(GameObject obj, String camera_path)
        {
            if (obj == null) { return false; }

            Quaternion quat = obj.transform.rotation;
            Vector3 pos = obj.transform.position;
            String objName = obj.name;

            UnityEngine.Object.Destroy(obj);

            //Debug.Log(camera_path);
            //Debug.Log(pos);
            //Debug.Log(quat);
            //Debug.Log(objName);

            GameObject camera_prefab = Resources.Load<GameObject>(camera_path);

            GameObject camera_pref = (GameObject)UnityEngine.Object.Instantiate(camera_prefab,
                                                                  pos,
                                                                  quat);

            camera_pref.name = objName;

            return true;

        }

    }
}
