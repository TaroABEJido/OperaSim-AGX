using UnityEngine;

namespace PWRISimulator
{

    [System.Serializable]
    public class SettingData
    {
        public int MaxDumpTracks;   //最大設置可能ダンプトラック数
        public int MaxCameras;      //最大設置可能カメラ数
        public int MinScore;        //スコア下限

        public float MiningCoef;    //採掘スコア係数
        public float LoadSoilCoef;  //土砂積込みスコア係数
        public float UnloadSoilCoef;//土砂積み降ろしスコア係数
        public float CollisionCoef; //重機衝突スコア係数
        public float OffTruckCoef;  //コースアウトスコア係数
        public float OverlappCoef;  //コース重複スコア係数

        public float GameTime;
        public float TimeBarRedThreshold;
        public float TimeBarYellowThreshold;

        public string datapath;     //データ保存パス
        public string RosIP;        //ROS接続先IP
    }
}
