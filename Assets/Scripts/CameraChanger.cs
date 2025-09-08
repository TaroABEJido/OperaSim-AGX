using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

using Debug = UnityEngine.Debug;
 
namespace PWRISimulator
{
    public class CameraChanger : MonoBehaviour
    {
        public List<Camera> cameras = new List<Camera>();
        private int CameraIndex = 0;
        private int CameraDepth_Default = 99;
        private int clsize = 0;

        private string MainCameraName = "MainCamera";


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


            if (Input.GetKeyUp(KeyCode.C) || GlobalVariables.ForceCameraChabge)
            {

                clsize = cameras.Count;
                
                if (GlobalVariables.ForceCameraChabge)
                {
                    CameraIndex = 0;
                    for (int i = 0; i < clsize; i++)
                    {
                        cameras[i].depth = CameraDepth_Default;
                        cameras[i].rect = new Rect(0,0,1,1);
                    }

                    GlobalVariables.ForceCameraChabge = false;
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
                        //cameras[i].rect = new Rect(0, baseLength * i, 1, baseLength); //c•À‚Ñ
                        //cameras[i].rect = new Rect(baseLength * i, 0, baseLength, 1); //‰¡•À‚Ñ
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
                            cameras[i].rect = new Rect(0, baseLength * i, 1, baseLength); //c•À‚Ñ
                        }
                        else {
                            cameras[i].rect = new Rect(baseLength * i, 0, baseLength, 1); //‰¡•À‚Ñ
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
