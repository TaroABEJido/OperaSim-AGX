using UnityEngine;

namespace PWRISimulator
{

    [System.Serializable]
    public class SettingData
    {
        public int MaxDumpTracks;   //�ő�ݒu�\�_���v�g���b�N��
        public int MaxCameras;      //�ő�ݒu�\�J������
        public int MinScore;        //�X�R�A����

        public float MiningCoef;    //�̌@�X�R�A�W��
        public float LoadSoilCoef;  //�y���ύ��݃X�R�A�W��
        public float UnloadSoilCoef;//�y���ςݍ~�낵�X�R�A�W��
        public float CollisionCoef; //�d�@�Փ˃X�R�A�W��
        public float OffTruckCoef;  //�R�[�X�A�E�g�X�R�A�W��
        public float OverlappCoef;  //�R�[�X�d���X�R�A�W��

        public float GameTime;
        public float TimeBarRedThreshold;
        public float TimeBarYellowThreshold;

        public string datapath;     //�f�[�^�ۑ��p�X
        public string RosIP;        //ROS�ڑ���IP
    }
}
