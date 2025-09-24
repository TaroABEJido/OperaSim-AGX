using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

using Debug = UnityEngine.Debug;
 
namespace PWRISimulator
{
    /// <summary>
    /// ゲーム画面に表示するカメラ等の切り替え処理
    /// </summary>
    public class CameraChanger : MonoBehaviour
    {
        public List<Camera> cameras = new List<Camera>();
        private int CameraIndex = 0;
        private int CameraDepth_Default = 99;
        private int clsize = 0;

        private string MainCameraName = "MainCamera";


        //public static List<Camera> cameras = new List<Camera>();
        //private static int CameraIndex = 0;
        //private static int CameraDepth_Default = 99;
        //public static int clsize = 0;

        //private static string MainCameraName = "MainCamera";

        public static bool resetFlag = false;


        public static List<Camera> FindGameCameras(bool inactiveFlg = false)
        {
            List<Camera> cameraList = new List<Camera>();
            var allCamArray = FindObjectsOfType<Camera>(inactiveFlg);

            foreach(Camera camObj in allCamArray)
            {
                if (!camObj.gameObject.name.Contains("Str")) {
                    cameraList.Add(camObj);
                    //UnityEngine.Debug.Log(camObj.gameObject.name);
                }
            }

            return cameraList;
        }

        public static List<Camera> FindGameStreamingCameras(bool inactiveFlg = false)
        {
            List<Camera> cameraList = new List<Camera>();
            var allCamArray = FindObjectsOfType<Camera>(inactiveFlg);

            foreach (Camera camObj in allCamArray)
            {
                if (camObj.gameObject.name.Contains("Str"))
                {
                    cameraList.Add(camObj);
                    //UnityEngine.Debug.Log(camObj.gameObject.name);
                }
            }

            return cameraList;
        }


        public static void Reset()
        {
            resetFlag = true;
        }


        // Start is called before the first frame update
        void Start()
        {
            //cameras.Add(Camera.main);
            //cameras.AddRange(FindObjectsOfType<Camera>());
            cameras.AddRange(FindGameCameras());
            clsize = cameras.Count;

            GameObject mainCamera = GameObject.FindGameObjectWithTag(MainCameraName);

            for (int i = 0; i < clsize; i++)
            {
                if (cameras[i].gameObject.name == mainCamera.name) { 
                    cameras[i].gameObject.SetActive(true);
                }
                else
                {
                    cameras[i].gameObject.SetActive(false);
                }
            }

        }

        // Update is called once per frame
        void Update()
        {
            if (resetFlag) {

                //GameObjectを取得
                var gameObjectList = FindObjectsOfType<Camera>(true);
                //GameObject mainCamera = gameObjectList.FindGameObjectWithTag(MainCameraName);

                GameObject mainCamera = null;
                foreach (Camera component in gameObjectList)
                {
                    // コンポーネントを持つGameObjectのタグが指定したタグと一致するか確認する
                    if (component.gameObject.CompareTag(MainCameraName))
                    {
                        mainCamera = component.gameObject;
                        mainCamera.SetActive(true);
                        mainCamera.transform.root.gameObject.SetActive(true);
                        //cameras.Add(component);
                    }
                }

                cameras.Clear();
                cameras.AddRange(FindGameCameras(true));
                clsize = cameras.Count;

                UnityEngine.Debug.Log(string.Join(",", cameras));
                UnityEngine.Debug.Log("clsize: " + clsize);


                for (int i = 0; i < clsize; i++)
                {
                    if (mainCamera)
                    {
                        UnityEngine.Debug.Log(cameras[i].gameObject.name);
                        UnityEngine.Debug.Log(mainCamera.name);

                        if (cameras[i].gameObject.name == mainCamera.name)
                        {
                            cameras[i].gameObject.SetActive(true);
                            CameraIndex = i;
                        }
                        else
                        {
                            cameras[i].gameObject.SetActive(false);
                        }

                    }
                    else
                    {
                        if (i == 0)
                        {
                            cameras[i].gameObject.SetActive(true);
                        }
                        else
                        {
                            cameras[i].gameObject.SetActive(false);
                        }
                    }
                }

                //UnityEngine.Debug.Log("CameraIndex: " + CameraIndex);

                GlobalVariables.ForceCameraChange = true;

                resetFlag = false;
            }



            //UnityEngine.Debug.Log("CameraIndex: " + CameraIndex);

            if (Input.GetKeyUp(KeyCode.Q))
            {
                cameras.Clear();
                //cameras.AddRange(FindObjectsOfType<Camera>(true));
                cameras.AddRange(FindGameCameras(true));
                clsize = cameras.Count;

                for (int i = 0; i < clsize; i++)
                {
                    cameras[i].gameObject.SetActive(true);
                }

            }


            cameras.Clear();
            //cameras.AddRange(FindObjectsOfType<Camera>());
            cameras.AddRange(FindGameCameras());
            clsize = cameras.Count;


            if (Input.GetKeyUp(KeyCode.C) || GlobalVariables.ForceCameraChange)
            {

                clsize = cameras.Count;
                
                if (GlobalVariables.ForceCameraChange)
                {
                    CameraIndex = 0;
                    for (int i = 0; i < clsize; i++)
                    {
                        cameras[i].depth = CameraDepth_Default;
                        cameras[i].rect = new Rect(0,0,1,1);
                    }

                    GlobalVariables.ForceCameraChange = false;
                }


                if (CameraIndex < clsize)
                {

                    for (int i = 0; i < clsize; i++)
                    {
                        //Debug.Log(cameras[i].depth);
                        if (i == CameraIndex)
                        {
                            cameras[i].depth = CameraDepth_Default + 1;
                            cameras[i].targetTexture = null;
                            cameras[i].enabled = true;
                            cameras[i].rect = new Rect(0, 0, 1, 1);
                        }
                        else
                        {
                            cameras[i].depth = CameraDepth_Default;
                            cameras[i].enabled = false;
                            cameras[i].rect = new Rect(0, 0, 1, 1);
                        }
                    }
                }
                else if (CameraIndex >= clsize && CameraIndex < clsize * 2)
                {
                    var NumSplit = 3;
                    var baseRate = 1.0f / NumSplit;
                    var baseHeight = 1.0f / (clsize - 1);

                    for (int i = 0; i < clsize; i++)
                    {
                        cameras[i].enabled = true;
                        //cameras[i].rect = new Rect(0, baseLength * i, 1, baseLength); //縦並び
                        //cameras[i].rect = new Rect(baseLength * i, 0, baseLength, 1); //横並び
                        if (i == CameraIndex - clsize)
                        {
                            cameras[i].rect = new Rect(0, 0, (NumSplit - 1) * baseRate, 1);
                        }
                        else if (i < CameraIndex - clsize)
                        {
                            cameras[i].rect = new Rect((NumSplit - 1) * baseRate, baseHeight * i, 1, baseHeight);
                        }
                        else if (i > CameraIndex - clsize)
                        {
                            cameras[i].rect = new Rect((NumSplit - 1) * baseRate, baseHeight * (i - 1), 1, baseHeight);
                        }

                    }
                }
                else
                {
                    var baseLength = 1.0f / clsize;
                    for (int i = 0; i < clsize; i++)
                    {
                        cameras[i].enabled = true;
                        if (CameraIndex % 2 == 0)
                        {
                            cameras[i].rect = new Rect(0, baseLength * i, 1, baseLength); //縦並び
                        }
                        else {
                            cameras[i].rect = new Rect(baseLength * i, 0, baseLength, 1); //横並び
                        }
                    }
                }

                    //if (CameraIndex == clsize - 1)
                if (CameraIndex == clsize * 2 -1 + 2)
                {
                    CameraIndex = 0;
                }
                else
                {
                    CameraIndex = CameraIndex + 1;
                }

            }

        }
    }
}
