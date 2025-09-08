using AGXUnity;
using AGXUnity.Collide;
using AGXUnity.Model;
using PWRISimulator.ROS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UIElements;
using static PWRISimulator.UIcontrol;
using static System.Net.Mime.MediaTypeNames;
//using UnityEngine.EventSystems;

using Debug = UnityEngine.Debug;


namespace PWRISimulator
{
    public class UIcontrol : MonoBehaviour
    {
        [SerializeField] GameObject MenuUI;
        private UIDocument _uiDocument;
        private GameObject munuUIobj;

        [SerializeField] GameObject CameraSelectorUI;
        private UIDocument _uiCameraSelectDocument;
        private GameObject CameraSelectorUIobj;


        [SerializeField] GameObject MessageDaialogUI;
        private GameObject MessageDaialogUIobj;
        private UIDocument _uiMessageDaialogDocument;

        [SerializeField] GameObject CountdownTimerUI;
        private GameObject CountdownTimerUIobj;
        private UIDocument _uiCountdownTimerDocument;


        //[SerializeField] GameObject CountdownTimerUI_disp2;
       // private GameObject CountdownTimerUIobj_disp2;
        //private UIDocument _uiCountdownTimerDocument_disp2;

        [SerializeField] GameObject ScoreUI;
        private GameObject ScoreUIobj;

        //[SerializeField] GameObject ScoreUI_disp2;
        //private GameObject ScoreUIobj_disp2;

        [SerializeField] GameObject StatusBoardUI;
        private GameObject StatusBoardUIobj;
        private StatusBoard sBoard;

        [SerializeField] GameObject SubdisplayUI;
        private GameObject SubdisplayUIobj;


        // セーブ・ロード機能
        [SerializeField] GameObject SaveLoadUI;
        private GameObject SaveLoadUIobj;
        private UIDocument _uiSaveLoadDocument;

        // 並進・回転・削除
        [SerializeField] GameObject ObjectTransRotUI;
        private GameObject ObjectTransRotUIobj;
        private UIDocument _uiObjectTransRotDocument;


        private int prev_SetMoveType;
        private DeformableTerrain terrain;

        private MachineObjCamCont machineObj;


        [System.Serializable]
        public class SaveParticles
        {
            public Particles[] data;
        }

        [System.Serializable]
        public class Particles
        {
            public agxVec3 position;
            public agxVec3 velocity;
            public double radius;
            public double mass;
        }

        [System.Serializable]
        public class agxVec3
        {
            public double x;
            public double y;
            public double z;
        }

        // ダンプトラックの積載
        [System.Serializable]
        public class SaveDumpSoil
        {
            public TransDumpSoil[] data;
        }

        [System.Serializable]
        public class TransDumpSoil
        {
            public string id;
            public double mass;
        }

        // 重機の姿勢(時間とスコアも追加)
        [System.Serializable]
        public class SaveMachines
        {
            public float time;
            public int score;
            public objProperties[] data;
        }

        [System.Serializable]
        public class objJoint
        {
            public double swing_joint;
            public double boom_joint;
            public double arm_joint;
            public double bucket_joint;
            public double right_track;
            public double left_track;
            public double dump_joint;
        }

        [System.Serializable]
        public class objProperties
        {
            public string name;
            public string id;
            public Vector3 p;
            public Quaternion q;
            public objJoint joint;
        }



        // Start is called before the first frame update
        void Start()
        {
            machineObj = new MachineObjCamCont();

            StatusBoardUIobj = Instantiate(StatusBoardUI);
            sBoard = StatusBoardUIobj.GetComponent<StatusBoard>();
            sBoard.SetStatusMessage("Start!!");
            sBoard.SetMessageColor(Color.blue);


            munuUIobj = Instantiate(MenuUI);
            _uiDocument = munuUIobj.GetComponent<UIDocument>();
            var root = _uiDocument.rootVisualElement;

            setupMenuUI();
            //root.Q<Button>("DunpTruckPositionSetting").clicked += () => OnDunpTruckPositionSettingClicked();
            //root.Q<Button>("CameraPositionSetting").clicked += () => OnCameraPositionSettingClicked();
            //root.Q<Button>("SelectCamera").clicked += () => OnSelectCameraClicked();
            //root.Q<Button>("SimulationStart").clicked += () => OnSimulationStartClicked();
            //root.Q<Button>("send").clicked += () => Debug.Log(root.Q<TextField>("TitleText").value);

            GlobalVariables.ActionMode = -1;

            SubdisplayUIobj = null;

            prev_SetMoveType = GlobalVariables.SetMoveType;
        }

        // Update is called once per frame
        void Update()
        {
            if (GlobalVariables.ActionMode != 3 && GlobalVariables.SetMoveType != prev_SetMoveType)
            {
                Debug.Log("SetMoveType: " + GlobalVariables.SetMoveType + ", prev: " + prev_SetMoveType);

                if (ObjectTransRotUIobj != null)
                {
                    if (_uiObjectTransRotDocument.gameObject.activeSelf)
                    {
                        if (GlobalVariables.SetMoveType == 0)
                        {
                            ObjTranslationClicked();
                        }
                        else if (GlobalVariables.SetMoveType == 1)
                        {
                            ObjRotationClicked();
                        }
                        else if (GlobalVariables.SetMoveType == 2)
                        {
                            ObjDeleteClicked();
                        }
                        else if (GlobalVariables.SetMoveType == 3)
                        {
                            MainCameraSettingsClicked();
                        }
                    }
                }
            }

            prev_SetMoveType = GlobalVariables.SetMoveType;
        }


        void OnDunpTruckPositionSettingClicked()
        {
            if (SubdisplayUIobj != null)
            {
                Destroy(SubdisplayUIobj);
                SubdisplayUIobj = null;
            }

            machineObj.machineDeselected();

            //GlobalVariables.ActionMode = 0;
            GlobalVariables.changeActionMode(0);
            Debug.Log("Score: " + GlobalVariables.ActionMode);

            //sBoard.SetStatusMessage("ダンプトラック配置");
            sBoard.SetStatusMessage("Dump truck Settings");

            SubdisplayUIobj = Instantiate(SubdisplayUI);
            SubdisplayUIobj.name = "SubdisplayForSpawnCamera";
            Debug.Log(SubdisplayUIobj.name);


            // 並進・回転・削除ボタン
            if (ObjectTransRotUIobj == null)
            {
                ObjectTransRotUIobj = Instantiate(ObjectTransRotUI);
                _uiObjectTransRotDocument = ObjectTransRotUIobj.GetComponent<UIDocument>();

                SetButtonAddFunction(_uiObjectTransRotDocument);
            }
            else
            {
                _uiObjectTransRotDocument.gameObject.SetActive(true);
                _uiObjectTransRotDocument = ObjectTransRotUIobj.GetComponent<UIDocument>();

                SetButtonAddFunction(_uiObjectTransRotDocument);
            }
        }


        void OnCameraPositionSettingClicked()
        {

            if (SubdisplayUIobj != null)
            {
                Destroy(SubdisplayUIobj);
                SubdisplayUIobj = null;
            }

            machineObj.machineDeselected();

            //GlobalVariables.ActionMode = 1;
            GlobalVariables.changeActionMode(1);
            Debug.Log("Score: " + GlobalVariables.ActionMode);

            //sBoard.SetStatusMessage("カメラ配置");
            sBoard.SetStatusMessage("Camera Settings");

            SubdisplayUIobj = Instantiate(SubdisplayUI);
            SubdisplayUIobj.name = "SubdisplayForSpawnCamera";
            Debug.Log(SubdisplayUIobj.name);


            //// 並進・回転・削除ボタン表示
            //_uiObjectTransRotDocument.gameObject.SetActive(true);


            // 並進・回転・削除ボタン
            if (ObjectTransRotUIobj == null)
            {
                ObjectTransRotUIobj = Instantiate(ObjectTransRotUI);
                _uiObjectTransRotDocument = ObjectTransRotUIobj.GetComponent<UIDocument>();

                SetButtonAddFunction(_uiObjectTransRotDocument);
            }
            else
            {
                _uiObjectTransRotDocument.gameObject.SetActive(true);
                _uiObjectTransRotDocument = ObjectTransRotUIobj.GetComponent<UIDocument>();

                SetButtonAddFunction(_uiObjectTransRotDocument);
            }
        }


        void OnSelectCameraClicked()
        {

            if (SubdisplayUIobj != null)
            {
                Destroy(SubdisplayUIobj);
                SubdisplayUIobj = null;
            }

            machineObj.machineDeselected();

            //GlobalVariables.ActionMode = 2;
            GlobalVariables.changeActionMode(2);
            Debug.Log("Score: " + GlobalVariables.ActionMode);

            _uiDocument.gameObject.SetActive(false);

            // 並進・回転・削除ボタン非表示
            if (_uiObjectTransRotDocument != null)
            {
                _uiObjectTransRotDocument.gameObject.SetActive(false);
            }


            CameraSelectorUIobj = Instantiate(CameraSelectorUI);
            _uiCameraSelectDocument = CameraSelectorUIobj.GetComponent<UIDocument>();
            var camera_root = _uiCameraSelectDocument.rootVisualElement;

            ListView listView = camera_root.Q<ListView>("CameraList");
            List<Camera> cameras = new List<Camera>();
            //cameras.AddRange(FindObjectsOfType<Camera>(true));
            cameras.AddRange(CameraChanger.FindGameCameras(true));
            List<string> CameraNameList = new List<string>();

            camera_root.Q<Button>("CameraSetButton").clicked += () => OnCameraSetClicked(cameras);

            GenerateCheckboxes(_uiCameraSelectDocument, cameras.Count, cameras);

            //sBoard.SetStatusMessage("使用カメラ選択");
            sBoard.SetStatusMessage("Camera Selection");
        }


        void OnSimulationStartClicked()
        {

            if (SubdisplayUIobj != null)
            {
                Destroy(SubdisplayUIobj);
                SubdisplayUIobj = null;
            }

            machineObj.machineDeselected();

            //GlobalVariables.ActionMode = 3;
            GlobalVariables.changeActionMode(3);
            Debug.Log("Score: " + GlobalVariables.ActionMode);

            UnityEngine.Object.Destroy(munuUIobj);

            List<Camera> cameras = new List<Camera>();
            //cameras.AddRange(FindObjectsOfType<Camera>(true));
            cameras.AddRange(CameraChanger.FindGameStreamingCameras(true));

            CountdownTimerUIobj = Instantiate(CountdownTimerUI);
            //CountdownTimerUIobj_disp2 = Instantiate(CountdownTimerUI_disp2);

            CountdownTimerUIobj.SetActive(true);

            //CountdownTimerUIobj.GetComponent<CountdownTimer>().ResetTimer();
            CountdownTimerUIobj.GetComponent<CountdownTimer>().StartTimer();
            //CountdownTimerUIobj_disp2.GetComponent<CountdownTimer>().StartTimer();

            int width = Screen.width;
            int height = Screen.height;

            RenderTexture cameraTexture = new RenderTexture(width, height, 24);

            //CountdownTimerUIobj_disp2.GetComponent<UIDocument>().panelSettings.targetTexture = cameraTexture;

            ScoreUIobj = Instantiate(ScoreUI);
            ScoreUIobj.SetActive(true);

            //ScoreUIobj_disp2 = Instantiate(ScoreUI_disp2);
            //ScoreUIobj_disp2.GetComponent<UIDocument>().panelSettings.targetTexture = cameraTexture;


            sBoard.DeleteStatusMessage();

            CamerasInvisible();


            // add 20250702

            // 配置したダンプトラックのオブジェクトを取得
            if ((int)GlobalVariables.Dump_IDList.Count > 0)
            {
                for (int i = 0; i < GlobalVariables.Dump_IDList.Count; i++)
                {
                    var id = GlobalVariables.Dump_IDList[i];           
                    GameObject dumpObj = GameObject.Find("ic120_" + id);
                    if (dumpObj != null) {
                        GlobalVariables.Dump_ObjList.Add(dumpObj);
                    }
                }
            }


            // 並進・回転・削除ボタン非表示
            if (_uiObjectTransRotDocument != null)
            {
                _uiObjectTransRotDocument.gameObject.SetActive(false);
            }

            GlobalVariables.SetMoveType = 0;


            // セーブ・ロード・リセット機能
            SaveLoadUIobj = Instantiate(SaveLoadUI);

            _uiSaveLoadDocument = SaveLoadUIobj.GetComponent<UIDocument>();
            _uiSaveLoadDocument.gameObject.SetActive(true);

            var save_root = _uiSaveLoadDocument.rootVisualElement;

            save_root.Q<Button>("Save").clicked += () => SaveClicked();
            save_root.Q<Button>("Load").clicked += () => LoadClicked();
            save_root.Q<Button>("Reset").clicked += () => ResetClicked();
        }




        void OnCameraSetClicked(List<Camera> cameras)
        {

            //var root = _uiDocument.rootVisualElement;
            var camera_root = _uiCameraSelectDocument.rootVisualElement;
            var toggles = camera_root.Query<Toggle>().ToList();

            int SelectedCameraCounter = 0;
            foreach (var toggle in toggles)
            {
                if (toggle.value == true)
                {
                    SelectedCameraCounter = SelectedCameraCounter + 1;
                }
            }

            if (SelectedCameraCounter > GlobalVariables.MaxCameras)
            {

                MessageDaialogUIobj = Instantiate(MessageDaialogUI);
                _uiMessageDaialogDocument = MessageDaialogUIobj.GetComponent<UIDocument>();

                var root = _uiMessageDaialogDocument.rootVisualElement;
                //root.Q<UnityEngine.UIElements.Label>("Title").text = "使用カメラ台数超過エラー";
                //root.Q<UnityEngine.UIElements.Label>("Message").text = "カメラが4台以上選択されています。";
                root.Q<UnityEngine.UIElements.Label>("Title").text = "Error";
                root.Q<UnityEngine.UIElements.Label>("Message").text = "More than four cameras are selected.";

                return;

            }
            else if (SelectedCameraCounter == 0)
            {
                MessageDaialogUIobj = Instantiate(MessageDaialogUI);
                _uiMessageDaialogDocument = MessageDaialogUIobj.GetComponent<UIDocument>();

                var root = _uiMessageDaialogDocument.rootVisualElement;
                //root.Q<UnityEngine.UIElements.Label>("Title").text = "使用カメラ未選択エラー";
                //root.Q<UnityEngine.UIElements.Label>("Message").text = "カメラが選択されていません。";
                root.Q<UnityEngine.UIElements.Label>("Title").text = "Error";
                root.Q<UnityEngine.UIElements.Label>("Message").text = "No camera selected.";
                return;

            }

            //Debug.Log(toggles.Count);
            //Debug.Log(cameras.Count);

            for (int i = 0; i < cameras.Count; i++)
            {
                if (toggles[i].value == true)
                {
                    cameras[i].gameObject.SetActive(true);
                    if (cameras[i].transform.parent != null)
                    {
                        if (cameras[i].transform.parent.gameObject.name.Contains("Str"))
                        {
                            cameras[i].transform.parent.gameObject.SetActive(true);
                        }
                    }
                }
                else
                {
                    cameras[i].gameObject.SetActive(false);
                    if (cameras[i].transform.parent != null)
                    {
                        if (cameras[i].transform.parent.gameObject.name.Contains("Str"))
                        {
                            cameras[i].transform.parent.gameObject.SetActive(false);
                        }
                    }
                }

                cameras[i].targetTexture = null;

            }


            _uiDocument.gameObject.SetActive(true);
            _uiCameraSelectDocument.gameObject.SetActive(false);
            UnityEngine.Object.Destroy(_uiCameraSelectDocument.gameObject);

            setupMenuUI();

            GlobalVariables.ForceCameraChabge = true;
        }


        void CamerasInvisible()
        {

            List<Camera> cameras = new List<Camera>();
            //cameras.AddRange(FindObjectsOfType<Camera>(true));
            cameras.AddRange(CameraChanger.FindGameCameras(true));

            for (int i = 0; i < cameras.Count; i++)
            {

                if (cameras[i].gameObject.transform.root.gameObject.name.Contains("Camera_"))
                {
                    Renderer[] renders = cameras[i].gameObject.transform.root.gameObject.GetComponentsInChildren<Renderer>();
                    foreach (Renderer render in renders)
                    {
                        Material material = render.material;
                        material.SetFloat("_Mode", 2);
                        Color color = material.color;
                        color.a = 0.0f;
                        material.color = color;
                    }
                }
            }

        }

        void setupMenuUI()
        {
            var root = _uiDocument.rootVisualElement;
            root.Q<Button>("DunpTruckPositionSetting").clicked += () => OnDunpTruckPositionSettingClicked();
            root.Q<Button>("CameraPositionSetting").clicked += () => OnCameraPositionSettingClicked();
            root.Q<Button>("SelectCamera").clicked += () => OnSelectCameraClicked();
            root.Q<Button>("SimulationStart").clicked += () => OnSimulationStartClicked();
        }


        public void GenerateCheckboxes(UIDocument uiDocument, int numberOfCheckboxes, List<Camera> cameras)
        {
            if (uiDocument == null || uiDocument.rootVisualElement == null)
            {
                Debug.LogError("UIDocument");
                return;
            }

            VisualElement root = uiDocument.rootVisualElement;



            for (int i = 0; i < numberOfCheckboxes; i++)
            {
                VisualElement checkboxContainer = new VisualElement();
                //checkboxContainer.style.flexDirection = FlexDirection.Row;
                checkboxContainer.style.flexDirection = FlexDirection.Column;
                checkboxContainer.style.alignItems = Align.FlexStart;

                Toggle checkbox;

                string camName = cameras[i].transform.root.gameObject.name;
                if (camName.Contains("Str")) camName = cameras[i].transform.gameObject.name;

                //UnityEngine.UIElements.Label label = new UnityEngine.UIElements.Label(cameras[i].transform.root.gameObject.name);
                checkbox = new Toggle();

                //checkbox.label = "";
                checkbox.label = camName;
                checkbox.name = $"checkbox_{i + 1}";
                checkbox.value = false;
                checkbox.style.position = Position.Relative;
                checkbox.style.marginBottom = 4;

                checkbox.RegisterValueChangedCallback(evt =>
                {

                });

                checkboxContainer.Add(checkbox);
                //checkboxContainer.Add(label);
                root.Q<VisualElement>("CameraList").Add(checkboxContainer);
            }
        }


        void SaveClicked()
        {
            Debug.Log("Save!");

            // 時間とスコアを保持
            var myTime = CountdownTimer.timeRemaining;
            var myScore = GlobalVariables.score;


            if (terrain == null)
            {
                terrain = FindObjectOfType<DeformableTerrain>();
            }
            var soilSim = terrain.Native.getSoilSimulationInterface();


            // 掘削時のモデル生成数確認
            //var num = soilSim.getNumSoilParticles();
            //Debug.Log("getNumSoilParticles: " + num);

            // agx::Physics::GranularBodyPtrArray を取得
            var soilParticles = soilSim.getSoilParticles();


            if ((int)soilParticles.size() > 0)
            {
                SaveParticles save = new SaveParticles();
                save.data = new Particles[soilParticles.size()];

                for (uint i = 0; i < soilParticles.size(); i++)
                {
                    //Debug.Log("Index: " + i);
                    save.data[i] = new Particles();

                    // agx::Physics::GranularBodyPtr から位置や径を取得
                    var pos = soilParticles.at(i).getPosition();
                    //Debug.Log("Position: " + pos[0] + ", " + pos[1] + ", " + pos[2]);
                    save.data[i].position = new agxVec3();
                    save.data[i].position.x = pos[0];
                    save.data[i].position.y = pos[1];
                    save.data[i].position.z = pos[2];

                    var vel = soilParticles.at(i).getVelocity();
                    //Debug.Log("Velocity: " + vel[0] + ", " + vel[1] + ", " + vel[2]);
                    save.data[i].velocity = new agxVec3();
                    save.data[i].velocity.x = vel[0];
                    save.data[i].velocity.y = vel[1];
                    save.data[i].velocity.z = vel[2];

                    var rad = soilParticles.at(i).getRadius();
                    //Debug.Log("Radius: " + rad);
                    save.data[i].radius = rad;

                    var mas = soilParticles.at(i).getMass();
                    //Debug.Log("Mass: " + mas);
                    save.data[i].mass = mas;
                }


                if (save.data != null)
                {
                    string json = JsonUtility.ToJson(save, true);
                    StreamWriter sw = new StreamWriter(Path.Combine(GlobalVariables.BACKUP_FOLDER, "SoilParticles"));
                    sw.Write(json);
                    sw.Flush();
                    sw.Close();
                }
            }

            // 地形を保存
            var savescript = SaveLoadUIobj.GetComponent<saveScript>();
            savescript.OnClick();


            // 重機の姿勢保持
            SaveMachines sm = new SaveMachines();
            int num = (int)GlobalVariables.Dump_ObjList.Count + 1;
            sm.data = new objProperties[num];
            sm.data[0] = new objProperties();


            // 時間とスコア
            sm.time = myTime;
            sm.score = myScore;

                        
            // ショベルカー
            GameObject shovelObj = GameObject.Find(SpawnObject.zx200_objName);

            var shovelInput = shovelObj.GetComponent<ExcavatorInput>();
            var shovelJoint = shovelInput.joints;

            sm.data[0].name = shovelObj.name;
            sm.data[0].id = shovelObj.name;
            sm.data[0].p = shovelObj.transform.position;
            sm.data[0].q = shovelObj.transform.rotation;

            sm.data[0].joint = new objJoint();

            sm.data[0].joint.swing_joint = shovelJoint.swing.JointCurrentPosition;
            sm.data[0].joint.boom_joint = shovelJoint.boomTilt.JointCurrentPosition;
            sm.data[0].joint.arm_joint = shovelJoint.armTilt.JointCurrentPosition;
            sm.data[0].joint.bucket_joint = shovelJoint.bucketTilt.JointCurrentPosition;
            sm.data[0].joint.right_track = shovelJoint.rightSprocket.JointCurrentPosition;
            sm.data[0].joint.left_track = shovelJoint.leftSprocket.JointCurrentPosition;


            if ((int)GlobalVariables.Dump_ObjList.Count > 0)
            {
                SaveDumpSoil sd = new SaveDumpSoil();
                sd.data = new TransDumpSoil[GlobalVariables.Dump_ObjList.Count];

                for (int i = 0; i < GlobalVariables.Dump_ObjList.Count; i++)
                {
                    sd.data[i] = new TransDumpSoil();

                    var id = GlobalVariables.Dump_IDList[i];
                    GameObject dumpObj = GlobalVariables.Dump_ObjList[i];

                    var ds = dumpObj.GetComponentInChildren<DumpSoil>();
                    Debug.Log("id: " + id + ", mass: " + ds.soilMass);

                    sd.data[i].id = id;
                    sd.data[i].mass = ds.soilMass;
                
                    // -----------------
                    var dumpInput = dumpObj.GetComponent<DumpTruckInput>();
                    var dumpJoint = dumpInput.joints;

                    sm.data[i + 1] = new objProperties();
                    sm.data[i + 1].joint = new objJoint();

                    sm.data[i + 1].name = dumpObj.name;
                    sm.data[i + 1].id = id;
                    sm.data[i + 1].p = dumpObj.transform.Find("base_link/body_link").position;
                    sm.data[i + 1].q = dumpObj.transform.Find("base_link/body_link").rotation;

                    sm.data[i + 1].joint.right_track = dumpJoint.rightSprocket.CurrentPosition;
                    sm.data[i + 1].joint.left_track = dumpJoint.leftSprocket.CurrentPosition;
                    sm.data[i + 1].joint.dump_joint = dumpJoint.dump_joint.CurrentPosition;
                }

                // ダンプトラックの積載保存
                if (sd.data != null)
                {
                    string json_sd = JsonUtility.ToJson(sd, true);
                    using (StreamWriter sw = new StreamWriter(Path.Combine(GlobalVariables.BACKUP_FOLDER, "DumpSoil")))
                    {
                        sw.Write(json_sd);
                    }
                }
            }


            // 重機の姿勢を保存
            if (sm.data != null)
            {
                string json_sm = JsonUtility.ToJson(sm, true);
                using (StreamWriter sw = new StreamWriter(Path.Combine(GlobalVariables.BACKUP_FOLDER, "MachinesJoints")))
                {
                    sw.Write(json_sm);
                }
            }

            // 泥濘エリアのカウント行列保存
            using (StreamWriter sw = new StreamWriter(Path.Combine(GlobalVariables.BACKUP_FOLDER, "MudAreaMatrix")))
            {
                //sw.Write(GlobalVariables.countMat);

                int rows = GlobalVariables.countMat.RowCount;
                int cols = GlobalVariables.countMat.ColumnCount;

                Debug.Log("rows: " + rows + ", cols: " + cols);

                for (int i = 0; i < rows; i++)
                {
                    string line = "";
                    for (int j = 0; j < cols; j++)
                    {
                        line = line + GlobalVariables.countMat[i, j].ToString() + ",";
                    }
                    sw.WriteLine(line);
                }
            }


            MessageDaialogUIobj = Instantiate(MessageDaialogUI);
            _uiMessageDaialogDocument = MessageDaialogUIobj.GetComponent<UIDocument>();

            var root = _uiMessageDaialogDocument.rootVisualElement;
            root.Q<UnityEngine.UIElements.Label>("Title").text = "Information";
            root.Q<UnityEngine.UIElements.Label>("Message").text = "Save completed.";
        }

        void LoadClicked()
        {
            Debug.Log("***** Load *****");

            // リセット
            if (terrain == null)
            {
                terrain = FindObjectOfType<DeformableTerrain>();
            }
            terrain.ResetHeights();

            // 泥濘エリアのカウントリセット
            GlobalVariables.countMat.Clear();


            // 掘削で生成されたモデルを削除
            var soilSim = terrain.Native?.getSoilSimulationInterface();
            var soilParticles = soilSim.getSoilParticles();

            for (uint i = 0; i < soilParticles.size(); i++)
            {
                // 残留モデルの削除
                soilSim.removeSoilParticle(soilParticles.at(i));
            }

            // 地形を読込
            var loadscript = SaveLoadUIobj.GetComponent<loadScript>();
            loadscript.OnClick();


            // ファイル読み込み（姿勢）
            StreamReader rd_ms = new StreamReader(Path.Combine(GlobalVariables.BACKUP_FOLDER, "MachinesJoints"));
            string str_ms = rd_ms.ReadToEnd();
            rd_ms.Close();

            SaveMachines json_ms = JsonUtility.FromJson<SaveMachines>(str_ms);

            GlobalVariables.saveMachines = json_ms;


            // ファイル読み込み（積載）
            StreamReader rd_ds = new StreamReader(Path.Combine(GlobalVariables.BACKUP_FOLDER, "DumpSoil"));
            string str_ds = rd_ds.ReadToEnd();
            rd_ds.Close();

            SaveDumpSoil json_ds = JsonUtility.FromJson<SaveDumpSoil>(str_ds);

            GlobalVariables.saveDumpSoil = json_ds;


            // ファイル読み込み（掘削で生成されたモデル）
            StreamReader rd = new StreamReader(Path.Combine(GlobalVariables.BACKUP_FOLDER, "SoilParticles"));
            string str_p = rd.ReadToEnd();
            rd.Close();

            SaveParticles json = JsonUtility.FromJson<SaveParticles>(str_p);

            GlobalVariables.saveParticles = json;



            // ショベルカー削除
            GameObject _shovelObj = GameObject.Find(SpawnObject.zx200_objName);
            if (_shovelObj != null)
            {
                UnityEngine.Object.Destroy(_shovelObj);
            }

            // 位置
            Vector3 _pos = new Vector3((float)json_ms.data[0].p.x, (float)json_ms.data[0].p.y, (float)json_ms.data[0].p.z);
            Debug.Log(_pos);

            // 回転
            Quaternion _qut = new Quaternion((float)json_ms.data[0].q.x, (float)json_ms.data[0].q.y, (float)json_ms.data[0].q.z, (float)json_ms.data[0].q.w);
            Debug.Log(_qut);

            // ショベルカー再配置
            GameObject zx200_prefab = Resources.Load<GameObject>(SpawnObject.zx200_path);
            GameObject shovelObj = (GameObject)UnityEngine.Object.Instantiate(zx200_prefab, _pos, _qut);
            shovelObj.name = SpawnObject.zx200_objName;


            // ダンプトラック削除
            for (int i = 0; i < GlobalVariables.Dump_ObjList.Count; i++)
            {
                GameObject dumpObj = GlobalVariables.Dump_ObjList[i];

                if (dumpObj != null)
                {
                    // 削除
                    Destroy(dumpObj);
                    GameObject objMassBody = GameObject.Find(dumpObj.name + "_SoilMassBody");
                    if (objMassBody != null) Destroy(objMassBody);
                    GameObject objMassJoint = GameObject.Find(dumpObj.name + "_SoilMassJoint");
                    if (objMassJoint != null) Destroy(objMassJoint);
                }
            }

            // 保持しているダンプトラックオブジェクトリストのクリア
            GlobalVariables.Dump_IDList.Clear();
            GlobalVariables.Dump_ObjList.Clear();

            GlobalVariables.ic120Counter = 0;


            // ダンプトラックに関する情報読み込み
            for (int i = 1; i < json_ms.data.Length; i++)
            {
                // 再配置
                GameObject ic120_prefab = Resources.Load<GameObject>("Prefabs/ic120_prefVar");
                GameObject ic120obj = (GameObject)UnityEngine.Object.Instantiate(ic120_prefab, json_ms.data[i].p, json_ms.data[i].q);
                ic120obj.name = json_ms.data[i].name;

                // ダンプトラックオブジェクトリストの更新
                GlobalVariables.Dump_IDList.Add(json_ms.data[i].id);
                GlobalVariables.Dump_ObjList.Add(ic120obj);

                GlobalVariables.ic120Counter += 1;
            }


            // 泥濘エリアのカウント行列読込
            using (StreamReader sr = new StreamReader(Path.Combine(GlobalVariables.BACKUP_FOLDER, "MudAreaMatrix")))
            {
                string content = sr.ReadToEnd();
                string[] strAry = content.Split(',');

                int rows = GlobalVariables.countMat.RowCount;
                int cols = GlobalVariables.countMat.ColumnCount;

                Debug.Log("rows: " + rows + ", cols: " + cols);

                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        GlobalVariables.countMat[i, j] = double.Parse(strAry[(rows * i - 1) + (j + 1)]);
                    }
                }
            }


            //// デバック
            //using (StreamWriter sw = new StreamWriter(Path.Combine(GlobalVariables.BACKUP_FOLDER, "MudAreaMatrix_set")))
            //{
            //    //sw.Write(GlobalVariables.countMat);

            //    int rows = GlobalVariables.countMat.RowCount;
            //    int cols = GlobalVariables.countMat.ColumnCount;

            //    Debug.Log("rows: " + rows + ", cols: " + cols);

            //    for (int i = 0; i < rows; i++)
            //    {
            //        string line = "";
            //        for (int j = 0; j < cols; j++)
            //        {
            //            line = line + GlobalVariables.countMat[i, j].ToString() + ",";
            //        }
            //        sw.WriteLine(line);
            //    }
            //}


            // カウントリセット
            GlobalVariables.SetupJointDumpCount = 0;

            // フラグを切り替えて動作開始
            GlobalVariables.SetupJointFlag = true;
            GlobalVariables.SetupJointDumpFlag = true;

        }

        void ResetClicked()
        {
            Debug.Log("Reset!");

            // 別のスクリプトで地形と重機の読み込みを実行するためにフラグ切り替え
            GlobalVariables.SelectMode = 2;


            List<Camera> cameras = new List<Camera>();
            cameras.AddRange(FindObjectsOfType<Camera>(true));

            for (int i = 0; i < cameras.Count; i++)
            {
                Debug.Log(cameras[i].gameObject.transform.root.gameObject.name.Contains("Camera_"));

                // 追加したカメラのみ削除
                if (cameras[i].gameObject.transform.root.gameObject.name.Contains("Camera_"))
                {
                    UnityEngine.Object.Destroy(cameras[i].gameObject.transform.root.gameObject);
                }
            }

            // カメラを削除
            GlobalVariables.CameraCounter = 0;



            // タイマー停止
            CountdownTimer.isRunning = false;

            // スコアリセット
            GlobalVariables.score = 0;


            // 設定画面に戻る
            StatusBoardUIobj = Instantiate(StatusBoardUI);
            sBoard = StatusBoardUIobj.GetComponent<StatusBoard>();
            sBoard.SetStatusMessage("Start!!");
            sBoard.SetMessageColor(Color.blue);

            munuUIobj = Instantiate(MenuUI);
            _uiDocument = munuUIobj.GetComponent<UIDocument>();
            var root = _uiDocument.rootVisualElement;

            setupMenuUI();

            ObjectTransRotUIobj = null;


            // タイマーやスコアを非表示にする
            CountdownTimerUIobj.SetActive(false);
            ScoreUIobj.SetActive(false);
            _uiSaveLoadDocument.gameObject.SetActive(false);

            // モードをセット
            GlobalVariables.ActionMode = -1;

            SubdisplayUIobj = null;
        }


        void MainCameraSettingsClicked() {
            GlobalVariables.SetMoveType = 3;

            _uiObjectTransRotDocument = ObjectTransRotUIobj.GetComponent<UIDocument>();
            var trans_root = _uiObjectTransRotDocument.rootVisualElement;

            SetButtonColorSelect(trans_root.Q<Button>("MainCameraSettings"));
            SetButtonColorDefault(trans_root.Q<Button>("Translation"));
            SetButtonColorDefault(trans_root.Q<Button>("Rotation"));
            SetButtonColorDefault(trans_root.Q<Button>("Delete"));
        }

        void ObjTranslationClicked() {
            GlobalVariables.SetMoveType = 0;

            _uiObjectTransRotDocument = ObjectTransRotUIobj.GetComponent<UIDocument>();
            var trans_root = _uiObjectTransRotDocument.rootVisualElement;

            SetButtonColorDefault(trans_root.Q<Button>("MainCameraSettings"));
            SetButtonColorSelect(trans_root.Q<Button>("Translation"));
            SetButtonColorDefault(trans_root.Q<Button>("Rotation"));
            SetButtonColorDefault(trans_root.Q<Button>("Delete"));
        }

        void ObjRotationClicked() {
            GlobalVariables.SetMoveType = 1;

            _uiObjectTransRotDocument = ObjectTransRotUIobj.GetComponent<UIDocument>();
            var trans_root = _uiObjectTransRotDocument.rootVisualElement;

            SetButtonColorDefault(trans_root.Q<Button>("MainCameraSettings"));
            SetButtonColorDefault(trans_root.Q<Button>("Translation"));
            SetButtonColorSelect(trans_root.Q<Button>("Rotation"));
            SetButtonColorDefault(trans_root.Q<Button>("Delete"));
        }

        void ObjDeleteClicked()
        {
            GlobalVariables.SetMoveType = 2;

            _uiObjectTransRotDocument = ObjectTransRotUIobj.GetComponent<UIDocument>();
            var trans_root = _uiObjectTransRotDocument.rootVisualElement;

            SetButtonColorDefault(trans_root.Q<Button>("MainCameraSettings"));
            SetButtonColorDefault(trans_root.Q<Button>("Translation"));
            SetButtonColorDefault(trans_root.Q<Button>("Rotation"));
            SetButtonColorSelect(trans_root.Q<Button>("Delete"));
        }


        void SetButtonColorSelect(Button btn)
        {
            // 文字色
            btn.style.color = Color.white;
            // 背景色
            btn.style.backgroundColor = new StyleColor(new Color32(0, 58, 118, 255));
        }

        void SetButtonColorDefault(Button btn)
        {
            // 文字色
            btn.style.color = Color.black;
            // 背景色
            btn.style.backgroundColor = new StyleColor(new Color32(188, 188, 188, 255));
        }

        void SetButtonAddFunction(UIDocument doc)
        {
            var trans_root = doc.rootVisualElement;

            trans_root.Q<Button>("MainCameraSettings").clicked += () => MainCameraSettingsClicked();
            trans_root.Q<Button>("Translation").clicked += () => ObjTranslationClicked();
            trans_root.Q<Button>("Rotation").clicked += () => ObjRotationClicked();
            trans_root.Q<Button>("Delete").clicked += () => ObjDeleteClicked();


            if (GlobalVariables.SetMoveType == 0)
            {
                ObjTranslationClicked();
            }
            else if (GlobalVariables.SetMoveType == 1)
            {
                ObjRotationClicked();
            }
            else if (GlobalVariables.SetMoveType == 2)
            {
                ObjDeleteClicked();
            }
            else if (GlobalVariables.SetMoveType == 3)
            {
                MainCameraSettingsClicked();
            }
        }
    }
}
