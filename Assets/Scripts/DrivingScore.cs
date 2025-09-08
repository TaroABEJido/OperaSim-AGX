using AGXUnity;
using MathNet.Numerics;
using MathNet.Numerics.Providers.LinearAlgebra;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

using Debug = UnityEngine.Debug;

namespace PWRISimulator
{
    public class DrivingScore : MonoBehaviour
    {
        // 位置を保持
        private int prevPosX;
        private int prevPosY;

        // 進入不可エリアに滞在した時間
        private float stayTime;

        // 積載量を保持
        private double prevVolume;
        private double volScore;


        // メッシュサイズの都合によるスコア積算

        // 泥濘エリア
        private double mudScore;


        private void scoringDumpSoil()
        {
            // 積載量取得
            var obj = this.transform.parent.parent.gameObject;
            var ds = obj.GetComponentInChildren<DumpSoil>();
            double volume = ds.soilVolume;

            if (volume >= prevVolume)
            {
                // 積載量の増加分
                var diff = volume - prevVolume;

                // スコア積算
                //volScore += GlobalVariables.LoadSoilCoef * diff / 0.1;
                volScore += GlobalVariables.LoadSoilCoef * diff;

                Debug.Log("***** scoringDumpSoil: " + volScore + " *****");
                Debug.Log("volume: " + volume + ", prevVolume: " + prevVolume);

                if (Math.Abs(volScore) >= 1.0)
                {
                    // スコア反映
                    GlobalVariables.incrementScore((int)volScore);

                    // スコア積算リセット
                    volScore = volScore - (int)volScore;
                    
                    // 値を保持
                    prevVolume = volume;
                }
            }
            else {
                // 積載量が減少した場合は値の保持のみ
                prevVolume = volume;
            }
        }

        private void scoringMuddyAreas(double x, double y)
        {
            // メッシュを0.5mで計算
            int _x = (int)(x / 0.5);
            int _y = (int)(y / 0.5);

            // 泥濘エリアで停止している場合は二重カウントしないようにする
            if (_x != prevPosX || _y != prevPosY)
            {
                // 移動したらカウントアップ
                GlobalVariables.setCountMat(_x, _y);

                Debug.Log("***** Mad Count: " + GlobalVariables.countMat[_x, _y] + ", index: " + _x + ", " + _y + " *****");

                // スコア計算
                if (GlobalVariables.getCountMat(_x, _y) > 1.0)
                {
                    // カウントが2以上になったら重畳
                    mudScore += GlobalVariables.OverlappCoef * 0.5;

                    Debug.Log("***** mudScore: " + mudScore + " *****");

                    if (Math.Abs(mudScore) > 0.5)
                    {
                        // スコア反映
                        GlobalVariables.incrementScore((int)mudScore);

                        // スコア積算リセット
                        mudScore = mudScore - (int)mudScore;
                    }
                }

                prevPosX = _x;
                prevPosY = _y;
            }
        }

        private void scoringRestrictedAreas()
        {
            // 経過時間の加算
            stayTime += Time.deltaTime;

            Debug.Log("stayTime: " + stayTime);

            // スコア計算
            if ((int)stayTime >= 1)
            {
                // 1秒以上経過で減算
                GlobalVariables.incrementScore((int)(GlobalVariables.OffTruckCoef * (int)stayTime));
                // スコア計算した分は経過時間から引いておく
                stayTime = stayTime - (int)stayTime;
            }
        }

        private void OnSeparation(SeparationData data)
        {
            //UnityEngine.Debug.Log("OnSeparation: " + data);
            Debug.Log("Component1: " + data.Component1.transform.root.gameObject.name);
            Debug.Log("Component2: " + data.Component2.transform.root.gameObject.name);

            var com_1 = data.Component1.transform.root.gameObject.name;
            var com_2 = data.Component2.transform.root.gameObject.name;

            // 重機名確認
            if ((com_1.Contains("ic120") || com_1.Contains("zx200")) &&
                (com_2.Contains("ic120") || com_2.Contains("zx200")))
            {
                Debug.Log("OnSeparation!!!");

                // スコア計算
                if (com_1 != com_2)
                {
                    // 他の重機との接触
                    GlobalVariables.incrementScore((int)GlobalVariables.CollisionCoef);
                }
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            // 初期化
            volScore = 0.0;
            stayTime = 0.0f;
            mudScore = 0.0;

            // 接触判定に使用するbody_linkを取得
            var parent = this.transform.parent.gameObject;

            //Debug.Log(parent);
            //Debug.Log(parent.transform.parent.gameObject);

            var body_link = parent.transform.Find("body_link").gameObject;

            //Debug.Log(body_link);
            //Debug.Log(body_link.transform.parent.parent.gameObject);


            // 他の重機との接触判定
            var rb = body_link.GetComponent<RigidBody>();
            if (rb == null)
            {
                Debug.LogWarning("MyContactListener: Expecting a RigidBody component.", this);
                return;
            }
            Debug.Log("Modifying surface velocity of " + rb.name + ".");

            // コールバックを設定
            Simulation.Instance.ContactCallbacks.OnSeparation(OnSeparation, rb);
        }

        // Update is called once per frame
        void Update()
        {
            if (GlobalVariables.ActionMode == 3)
            {
                Debug.Log("prevPosX: " + prevPosX + ", prevPosY: " + prevPosY + ", Object: " + this.gameObject);

                // 現在地を取得
                double Xpos = this.gameObject.transform.position.x;
                double Ypos = this.gameObject.transform.position.z;

                // エリア確認
                int x_idx = (int)(Xpos / GlobalVariables.step_x);
                int z_idx = (int)(Ypos / GlobalVariables.step_z);

                int curtArea = (int)GlobalVariables.getAreaMat(x_idx, z_idx);

                //Debug.Log("curtArea: " + curtArea + ", Position: (" + Xpos + ", " + Ypos + ")");
                Debug.Log("curtArea: " + curtArea + ", " + this.gameObject.transform.parent.parent.gameObject);

                //--------------------
                // エリアごとの処理
                //--------------------
                if (curtArea == 2)
                {
                    // 進入不可エリア
                    scoringRestrictedAreas();
                }
                else if (curtArea == 5)
                {
                    // 泥濘エリア
                    scoringMuddyAreas(Xpos, Ypos);
                }

                // 進入不可エリアでない場合は経過時間をリセット
                if (curtArea != 2) {
                    stayTime = 0.0f;
                }

                //--------------------
                // 積載量のスコアリング
                //--------------------
                scoringDumpSoil();
            }
        }
    }
}
