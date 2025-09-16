using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;


namespace PWRISimulator
{
    /// <summary>
    /// �O���[�o���ϐ��Ǘ�
    /// </summary>
    public class GlobalVariables
    {
        // add 202507
        // �z�u�����_���v�g���b�N�I�u�W�F�N�g�̕ێ�
        public static List<string> Dump_IDList = new List<string>();
        public static List<GameObject> Dump_ObjList = new List<GameObject>();

        // �Z�[�u�E���[�h�@�\�ł̎p���ǂݍ��݃t���O
        public static bool SetupJointFlag = false;
        public static bool SetupJointDumpFlag = false;
        public static int SetupJointDumpCount = 0;

        // �Z�[�u�E���[�h�@�\�ł̎p���ǂݍ��݊����t���O
        public static bool SetupJointCompletedFlag = false;
        public static bool SetupJointDumpCompletedFlag = false;

        public static saveScript.SaveMachines saveMachines = new saveScript.SaveMachines();
        public static saveScript.SaveDumpSoil saveDumpSoil = new saveScript.SaveDumpSoil();
        public static saveScript.SaveParticles saveParticles = new saveScript.SaveParticles();

        //public static bool ObjectRemoveFlag = false;
        public static int ConfirmWaitFlag = 0; // 0: �����l�A1�F�m�F��ʕ\�����A2�F�m�F���OK�N���b�N


        // �t�@�C���o�͎��̃t�H���_�p�X
        public static string BACKUP_FOLDER = "Assets/SaveData/";


        // �G���A����s��
        public static MathNet.Numerics.LinearAlgebra.Matrix<double> areaMat;

        // �s�N�Z���Ԃ̋���
        public static double step_x = 0.0;
        public static double step_z = 0.0;

        // �D�^�G���A�̃J�E���g�s��
        public static MathNet.Numerics.LinearAlgebra.Matrix<double> countMat;

        private static Mutex _mutexAreaMat = new Mutex();
        private static Mutex _mutexCountMat = new Mutex();


        public static double getAreaMat(int x, int y)
        {
            double val = -1.0;

            if (_mutexAreaMat.WaitOne(TimeOutSpan))
            {
                try
                {
                    val = areaMat[x, y];
                }
                finally
                {
                    _mutexAreaMat.ReleaseMutex();
                }
            }
            else
            {
                UnityEngine.Debug.Log("Error : Could not get _mutexAreaMat.");
            }

            return val;
        }


        public static void setCountMat(int x, int y)
        {
            if (_mutexCountMat.WaitOne(TimeOutSpan))
            {
                try
                {
                    countMat[x, y] += 1.0;
                }
                finally
                {
                    _mutexCountMat.ReleaseMutex();
                }
            }
            else
            {
                UnityEngine.Debug.Log("Error : Could not get _mutexCountMat.");
            }
        }

        public static double getCountMat(int x, int y)
        {
            double val = -1.0;

            if (_mutexCountMat.WaitOne(TimeOutSpan))
            {
                try
                {
                    val = countMat[x, y];
                }
                finally
                {
                    _mutexCountMat.ReleaseMutex();
                }
            }
            else
            {
                UnityEngine.Debug.Log("Error : Could not get _mutexCountMat.");
            }

            return val;
        }




        //�ݒ�f�[�^
        public static int MaxDunpTracks;   //�ő�ݒu�\�_���v�g���b�N��
        public static int MaxCameras;      //�ő�ݒu�\�J������
        public static int MinScore;      //�ő�ݒu�\�J������
        public static float MiningCoef;    //�̌@�X�R�A�W��
        public static float LoadSoilCoef;  //�y���ύ��݃X�R�A�W��
        public static float UnloadSoilCoef;//�y���ςݍ~�낵�X�R�A�W��
        public static float CollisionCoef; //�d�@�Փ˃X�R�A�W��
        public static float OffTruckCoef;  //�R�[�X�A�E�g�X�R�A�W��
        public static float OverlappCoef;  //�R�[�X�d���X�R�A�W��
        public static string datapath;     //�f�[�^�ۑ��p�X
        public static string RosIP;        //ROS�ڑ���IP
        public static float GameTime;               //�Q�[�����ԁi�b�j      
        public static float TimeBarRedThreshold;      //�^�C���o�[�̐ԐF�ւ̐؂�ւ������i���j
        public static float TimeBarYellowThreshold;   //�^�C���o�[�̉��F�ւ̐؂�ւ������i���j


        public static int score = 0;

        public static double OutOfFieldAreaTime = 0.0;

        public static double AmountOfPickupSoil = 0.0;

        public static double AmountOfDropedSoil = 0.0;

        public static double AmountOfTransportedSoil = 0.0;

        public static double AmountOfLoadedSoil = 0.0;

        public static int ActionMode = -1; //0:�g���b�N�z�u,�@1:�J�����z�u,�@2:�g�p�J�����I��,�@3:�V�~�����[�V����
        public static int SelectMode = -1; //0:�Z�[�u,�@1:���[�h,�@2:���Z�b�g
        public static int SetMoveType = 0; //0:���i,�@1:��],�@2:�폜,�@3:���C���J����

        public static int TimeOutSpan = 100;

        public static bool CameraSelected = false;

        public static int ic120Counter = 0;
        public static int CameraCounter = 0;

        public static bool ForceCameraChange = false;

        private static Mutex _mutexScore = new Mutex();
        private static Mutex _mutexOOFAT = new Mutex();
        private static Mutex _mutexAOPS = new Mutex();
        private static Mutex _mutexAODS = new Mutex();
        private static Mutex _mutexAOTS = new Mutex();
        private static Mutex _mutexAOLS = new Mutex();
        private static Mutex _mutexActionMode = new Mutex();

        public static void incrementScore(int point)
        {
            if (_mutexScore.WaitOne(TimeOutSpan))
            {
                try
                {
                    // �X�R�A�����l�̏ꍇ�͌��Z���Ȃ�
                    if (score <= MinScore && point < 0) {
                        score = MinScore;
                        return;
                    }

                    score = score + point;
                }
                finally
                {
                    _mutexScore.ReleaseMutex();
                }
            }
            else
            {
                UnityEngine.Debug.Log("Error : Could not get _mutexScore.");
            }
        }

        public static void decrementScore(int point)
        {
            if (_mutexScore.WaitOne(TimeOutSpan))
            {
                try
                {
                    score = score - point;
                }
                finally
                {
                    _mutexScore.ReleaseMutex();
                }
            }
            else
            {
                UnityEngine.Debug.Log("Error : Could not get _mutexScore.");
            }
        }


        public static void incrementOutOfFieldAreaTime(double point)
        {
            if (_mutexOOFAT.WaitOne(TimeOutSpan))
            {
                try
                {
                    OutOfFieldAreaTime = OutOfFieldAreaTime + point;
                }
                finally
                {
                    _mutexOOFAT.ReleaseMutex();
                }
            }
            else
            {
                UnityEngine.Debug.Log("Error : Could not get _mutexOOFAT.");
            }
        }


        public static void incrementAmountOfPickupSoil(double point)
        {
            if (_mutexAOPS.WaitOne(TimeOutSpan))
            {
                try
                {
                    AmountOfPickupSoil = AmountOfPickupSoil + point;
                }
                finally
                {
                    _mutexAOPS.ReleaseMutex();
                }
            }
            else
            {
                UnityEngine.Debug.Log("Error : Could not get _mutexAOPS.");
            }
        }

        public static void decrementAmountOfPickupSoil(double point)
        {
            if (_mutexAOPS.WaitOne(TimeOutSpan))
            {
                try
                {
                    AmountOfPickupSoil = AmountOfPickupSoil - point;
                }
                finally
                {
                    _mutexAOPS.ReleaseMutex();
                }
            }
            else
            {
                UnityEngine.Debug.Log("Error : Could not get _mutexAOPS.");
            }
        }

        public static void changeActionMode(int mode)
        {
            if (_mutexActionMode.WaitOne(TimeOutSpan))
            {
                try
                {
                    ActionMode = mode;
                }
                finally
                {
                    _mutexActionMode.ReleaseMutex();
                }
            }
            else
            {
                UnityEngine.Debug.Log("Error : Could not get _mutexActionMode.");
            }

        }



    }
}
