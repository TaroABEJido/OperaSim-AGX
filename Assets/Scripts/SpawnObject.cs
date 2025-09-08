using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using Debug = UnityEngine.Debug;

using UnityEngine.UIElements;

namespace PWRISimulator
{
    //[RequireComponent(typeof(Camera))]
    public class SpawnObject : MonoBehaviour
    {

        [SerializeField] GameObject MessageDaialogUI;
        private GameObject MessageDaialogUIobj;
        private UIDocument _uiMessageDaialogDocument;


        public const string ic120_path = "Prefabs/ic120_prefVar";
        public const string zx120_path = "Prefabs/zx120_prefVar";
        public const string zx200_path = "Prefabs/zx200_prefVar";
        public const string camera_path = "Prefabs/CameraObj_prefVariant";

        public const string ic120_objName = "ic120_prefVar";
        public const string zx120_objName = "zx120_prefVar";
        public const string zx200_objName = "zx200_prefVar";

        private Vector3 mousePosition;

        public Camera myCamera { get; private set; }
        public List<Camera> cameras = new List<Camera>();

        public float deltaTime = 1.0f;


        private LayerMask selectionMask = Physics.DefaultRaycastLayers;

        private Mouse mouse;

        //private int ic120Counter = 0;
        //private int CameraCounter = 0;

        private ic120obj ic120obj;
        private cameraObj cameraObj;



        void Awake()
        {
            myCamera = GetComponent<Camera>();

        }


            // Start is called before the first frame update
            void Start()
        {
            UnityEngine.Cursor.visible = true;
            //Screen.lockCursor = false;        //old

            mouse = Mouse.current;
            if (mouse == null)
            {
                //Debug.Log(mouse);
                InputSystem.EnableDevice(mouse);
            }

            ic120obj = new ic120obj();
            cameraObj = new cameraObj();

        }

        // Update is called once per frame
        void Update()
        {

            if (GlobalVariables.ActionMode < 0)
            {
                return;
            }



            if (mouse.middleButton.wasReleasedThisFrame)
            {
                Debug.Log("Click");

                cameras.Clear();

                //現在有効なカメラ取得
                //cameras.AddRange(FindObjectsOfType<Camera>());
                cameras.AddRange(CameraChanger.FindGameCameras());
                for (int i = 0; i < cameras.Count; i++)
                {
                    if (cameras[i].enabled == true && cameras[i].gameObject.name == "Main Camera")
                    {
                        myCamera = cameras[i];
                    }
                }

                UnityEngine.Debug.Log(myCamera.gameObject.name);

                RaycastHit hitInfo;

                Vector2 mouseP = Mouse.current.position.ReadValue();
                Ray ray = myCamera.ScreenPointToRay(mouseP);

                if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, selectionMask))
                {
                    Debug.Log("Hit");

                    Vector3 mousePosition = hitInfo.point;
                    mousePosition.y = 0.5f + mousePosition.y;
                    Vector3 worldPosition = myCamera.ScreenToWorldPoint(mousePosition);
                    Vector3 mousePosition_ = myCamera.ScreenToWorldPoint(mouse.position.ReadValue());

                    //Debug.Log(Input.mousePosition);
                    Debug.Log(myCamera.ScreenToWorldPoint(mouse.position.ReadValue()));
                    Debug.Log(mousePosition);
                    Debug.Log(worldPosition);


                    if (GlobalVariables.ActionMode == 0) {

                        if (GlobalVariables.ic120Counter < GlobalVariables.MaxDunpTracks)
                        {

                            int objID = findSpawnObjID("ic120_", GlobalVariables.ic120Counter, GlobalVariables.MaxDunpTracks);

                            ic120obj.Spawn_ic120(mousePosition, Quaternion.identity, objID, ic120_path);
                            GlobalVariables.ic120Counter = GlobalVariables.ic120Counter + 1;
                            //GameObject.Find(ic120_pref.name + "/base_link/track_link").SetActive(false);
                            Debug.Log("ic120 Spawn");
                        }
                        else
                        {
                            MessageDaialogUIobj = Instantiate(MessageDaialogUI);
                            _uiMessageDaialogDocument = MessageDaialogUIobj.GetComponent<UIDocument>();

                            var root = _uiMessageDaialogDocument.rootVisualElement;
                            //root.Q<UnityEngine.UIElements.Label>("Title").text = "設置可能トラック台数超過エラー";
                            //root.Q<UnityEngine.UIElements.Label>("Message").text = "既にトラックが" + GlobalVariables.MaxDunpTracks.ToString() + "台設置されています。";

                            root.Q<UnityEngine.UIElements.Label>("Title").text = "Error";
                            root.Q<UnityEngine.UIElements.Label>("Message").text = "There are already " + GlobalVariables.MaxDunpTracks.ToString() + " trucks installed.";
                        }

                    }
                    else if (GlobalVariables.ActionMode == 1) {

                        if (GlobalVariables.CameraCounter < GlobalVariables.MaxCameras)
                        {
                            int objID = findSpawnObjID("Camera_", GlobalVariables.CameraCounter, GlobalVariables.MaxCameras);
                            cameraObj.Spawn_Camera(mousePosition, Quaternion.identity, objID, camera_path);
                            GlobalVariables.CameraCounter = GlobalVariables.CameraCounter + 1;
                            Debug.Log("Camera Spawn");
                        }
                        else
                        {
                            MessageDaialogUIobj = Instantiate(MessageDaialogUI);
                            _uiMessageDaialogDocument = MessageDaialogUIobj.GetComponent<UIDocument>();

                            var root = _uiMessageDaialogDocument.rootVisualElement;
                            //root.Q<UnityEngine.UIElements.Label>("Title").text = "設置可能カメラ台数超過エラー";
                            //root.Q<UnityEngine.UIElements.Label>("Message").text = "既にカメラが" + GlobalVariables.MaxCameras.ToString() + "台設置されています。";

                            root.Q<UnityEngine.UIElements.Label>("Title").text = "Error";
                            root.Q<UnityEngine.UIElements.Label>("Message").text = "There are already " + GlobalVariables.MaxCameras.ToString() + " cameras installed.";
                        }
                    }


                    //Debug.Log("Spawn");

                    //float deltaTime = Time.fixedDeltaTime;
                    //Physics.Simulate(deltaTime);
                    //}

                }

            }





        }

        int findSpawnObjID(String ObjeName, int currentNum, int maxNum)
        {
            int id = 0;

            for (int i = 0; i < maxNum; i++)
            {
                GameObject obj = GameObject.Find(ObjeName + i.ToString());
                if (obj == null) {
                    id = i;
                    break;
                }
            }
            return id;
        }
    }
}
