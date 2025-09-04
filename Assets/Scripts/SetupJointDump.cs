using PWRISimulator.ROS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace PWRISimulator
{
    public class SetupJointDump : MonoBehaviour
    {
        private DumpTruckJoint joints;
        private DumpTruckInput input;

        // 全軸動作完了フラグ
        private bool completedFlag = false;

        // 各軸の動作完了フラグ
        //private bool leftSprocketFlag = false;
        //private bool rightSprocketFlag = false;
        private bool dump_jointFlag = false;

        // Start is called before the first frame update
        void Start()
        {
            UnityEngine.Debug.Log("SetupJointDump!!!");

            var dumpObj = this.gameObject;

            // クラス読み込み
            input = dumpObj.GetComponent<DumpTruckInput>();
            joints = input.joints;
        }

        // Update is called once per frame
        void Update()
        {
            if (GlobalVariables.SetupJointDumpFlag && !completedFlag)
            {
                // 重機のゲームオブジェクト
                var dumpObj = this.gameObject;

                // JSON形式で保存したデータを読込
                var json_ms = GlobalVariables.saveMachines;

                // オブジェクトと同名のデータを利用
                int myidx = -1;
                for (int i = 1; i < json_ms.data.Length; i++)
                {
                    UnityEngine.Debug.Log("Obj_name: " + dumpObj.name + ", json: " + json_ms.data[i].name);

                    if (json_ms.data[i].name == dumpObj.name)
                    {
                        myidx = i;
                    }
                }


                UnityEngine.Debug.Log("myidx: " + myidx);

                if (myidx > 0)
                {
                    // ControlTypeはSpeed（関節が移動する角速度を入力する）
                    input.controlType = ControlType.Speed;
                    input.movementControlType = ConstractionMovementControlType.ActuatorCommand;

                    joints.leftSprocket.controlType = ControlType.Speed;
                    joints.rightSprocket.controlType = ControlType.Speed;
                    joints.dump_joint.controlType = ControlType.Speed;


                    // 許容誤差
                    const double error = 0.0174533f; // プラス・マイナス1度くらい許容
                    // 動作速度
                    const float speed = 0.523599f; // 30度くらいに設定


                    // 差分格納
                    float diff = 0.0f;


                    // 動作完了確認
                    if (json_ms.data[myidx].joint.dump_joint - error < joints.dump_joint.CurrentPosition &&
                        json_ms.data[myidx].joint.dump_joint + error > joints.dump_joint.CurrentPosition)
                    {
                        // 速度ゼロで停止させる
                        joints.dump_joint.controlValue = 0.0f;
                        // フラグ切替
                        dump_jointFlag = true;
                    }


                    // 全軸動作完了確認
                    if (dump_jointFlag)
                    {
                        // 直接制御を解除
                        input.movementControlType = ConstractionMovementControlType.TwistCommand;

                        // 動作完了フラグ
                        completedFlag = true;
                        GlobalVariables.SetupJointDumpCount += 1;

                        UnityEngine.Debug.Log(GlobalVariables.Dump_ObjList.Count + ", " + GlobalVariables.SetupJointDumpCount);

                        if ((int)GlobalVariables.Dump_ObjList.Count == GlobalVariables.SetupJointDumpCount)
                        {
                            GlobalVariables.SetupJointDumpCompletedFlag = true;
                        }

                        UnityEngine.Debug.Log("Completed!!: " + GlobalVariables.SetupJointDumpCompletedFlag);
                    }


                    //UnityEngine.Debug.Log("Flag: " + dump_jointFlag);


                    // 軸に値をセット
                    if (!dump_jointFlag)
                    {
                        diff = (float)(joints.dump_joint.CurrentPosition - json_ms.data[myidx].joint.dump_joint);
                        joints.dump_joint.controlValue = setValue(diff, speed);

                        //UnityEngine.Debug.Log("CurrentPosition: " + joints.dump_joint.CurrentPosition);
                        //UnityEngine.Debug.Log("diff: " + diff);
                        //UnityEngine.Debug.Log("controlValue: " + joints.dump_joint.controlValue);
                    }

                    joints.UpdateConstraintControls();

                }
                else
                {
                    // 直接制御を解除
                    input.movementControlType = ConstractionMovementControlType.TwistCommand;

                    // 動作完了フラグ
                    completedFlag = true;
                    GlobalVariables.SetupJointDumpCount += 1;
                    if ((int)GlobalVariables.Dump_ObjList.Count >= GlobalVariables.SetupJointDumpCount)
                    {
                        GlobalVariables.SetupJointDumpCompletedFlag = true;
                    }
                }
            }


            if (!GlobalVariables.SetupJointDumpFlag && completedFlag)
            {
                completedFlag = false;
                dump_jointFlag = false;
            }
        }



        float setValue(float diff, float speed)
        {

            float result = 0.0f;

            if (Math.Abs(diff) > speed)
            {
                if (diff < 0.0f)
                {
                    result = speed;
                }
                else
                {
                    result = -1.0f * speed;
                }
            }
            else
            {
                if (diff > 0.0)
                {
                    result = -1.0f * speed * 0.5f;
                }
                else
                {
                    result = speed * 0.5f;
                }
            }
            return result;
        }
        
    }
}

