using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using static System.Net.Mime.MediaTypeNames;




namespace PWRISimulator
{
    public class SettingSaveLoadManager : MonoBehaviour
    {

        [SerializeField]
        public string filePath;

        const string DefaultRosIP = "192.168.0.74";


        void Awake()
        {
            filePath = UnityEngine.Application.persistentDataPath + "/SettingData.json";
            UnityEngine.Debug.Log(filePath);

            LoadSetting();

        }

        // Start is called before the first frame update
        //void Start()
        //{
        //    
        //}

        // Update is called once per frame
        //void Update()
        //{
        //
        //}


        public void SaveSetting()
        {
            SettingData sdata = new SettingData();

            sdata.MaxDumpTracks = GlobalVariables.MaxDunpTracks;
            sdata.MaxCameras = GlobalVariables.MaxCameras;
            sdata.MinScore = GlobalVariables.MinScore;

            sdata.MiningCoef = GlobalVariables.MiningCoef;
            sdata.LoadSoilCoef = GlobalVariables.LoadSoilCoef;
            sdata.UnloadSoilCoef = GlobalVariables.UnloadSoilCoef;
            sdata.CollisionCoef = GlobalVariables.CollisionCoef;
            sdata.OffTruckCoef = GlobalVariables.OffTruckCoef;
            sdata.OverlappCoef = GlobalVariables.OverlappCoef;

            sdata.GameTime = GlobalVariables.GameTime;
            sdata.TimeBarRedThreshold = GlobalVariables.TimeBarRedThreshold;
            sdata.TimeBarYellowThreshold = GlobalVariables.TimeBarYellowThreshold;

            sdata.datapath = GlobalVariables.datapath;
            sdata.RosIP = GlobalVariables.RosIP;

            string json = JsonUtility.ToJson(sdata, true);
            File.WriteAllText(filePath, json);

            UnityEngine.Debug.Log("�f�[�^��ۑ����܂���: " + json);

        }

        public void LoadSetting()
        {

            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                SettingData sdata = JsonUtility.FromJson<SettingData>(json);

                UnityEngine.Debug.Log("�f�[�^��ǂݍ��݂܂���: ");

                GlobalVariables.MaxDunpTracks = sdata.MaxDumpTracks;
                GlobalVariables.MaxCameras = sdata.MaxCameras;
                GlobalVariables.MinScore = sdata.MinScore;

                GlobalVariables.MiningCoef = sdata.MiningCoef;
                GlobalVariables.LoadSoilCoef = sdata.LoadSoilCoef;
                GlobalVariables.UnloadSoilCoef = sdata.UnloadSoilCoef;
                GlobalVariables.CollisionCoef = sdata.CollisionCoef;
                GlobalVariables.OffTruckCoef = sdata.OffTruckCoef;
                GlobalVariables.OverlappCoef = sdata.OverlappCoef;

                GlobalVariables.GameTime = sdata.GameTime;
                GlobalVariables.TimeBarRedThreshold = sdata.TimeBarRedThreshold;
                GlobalVariables.TimeBarYellowThreshold = sdata.TimeBarYellowThreshold;

                GlobalVariables.datapath = sdata.datapath;
                GlobalVariables.RosIP = sdata.RosIP;

            }
            else
            {
                UnityEngine.Debug.Log("�ۑ��ݒ�f�[�^��������܂���B�����ݒ�l�����[�h���܂��B");

                LoadDefaultSetting();
                SaveSetting();

            }

        }



        public void LoadDefaultSetting()
        {

            GlobalVariables.MaxDunpTracks = 4;
            GlobalVariables.MaxCameras = 3;
            GlobalVariables.MinScore = -100;

            GlobalVariables.MiningCoef = 1.0f / 0.5f;
            GlobalVariables.LoadSoilCoef = 1.0f / 0.1f;
            GlobalVariables.UnloadSoilCoef = 10.0f / 0.1f;
            GlobalVariables.CollisionCoef = -5.0f / 1.0f;
            GlobalVariables.OffTruckCoef = -1.0f / 1.0f;
            GlobalVariables.OverlappCoef = -1.0f / 0.5f;

            GlobalVariables.GameTime = 60.0f*15.0f;
            GlobalVariables.TimeBarRedThreshold = 33.33333f;
            GlobalVariables.TimeBarYellowThreshold = 66.66666f;


            GlobalVariables.datapath = UnityEngine.Application.persistentDataPath;
            GlobalVariables.RosIP = DefaultRosIP;

        }



    }
}
