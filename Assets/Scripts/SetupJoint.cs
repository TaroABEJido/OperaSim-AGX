using PWRISimulator.ROS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace PWRISimulator
{
    public class SetupJoint : MonoBehaviour
    {
        private ExcavatorJoints joints;
        private ExcavatorInput input;

        // 全軸動作完了フラグ
        private bool completedFlag = false;

        // 各軸の動作完了フラグ
        private bool bucketTiltFlag = false;
        private bool armTiltFlag = false;
        private bool boomTiltFlag = false;
        private bool swingFlag = false;
        //private bool leftSprocketFlag = false;
        //private bool rightSprocketFlag = false;


        // Start is called before the first frame update
        void Start()
        {
            UnityEngine.Debug.Log("SetupJoint!!!");

            var shovelObj = this.gameObject;

            // クラス読み込み
            input = shovelObj.GetComponent<ExcavatorInput>();
            joints = input.joints;
        }

        // Update is called once per frame
        void Update()
        {
            //UnityEngine.Debug.Log("bucketTilt: " + joints.bucketTilt.actuator.CurrentPosition
            //        + ", armTilt   : " + joints.armTilt.actuator.CurrentPosition
            //        + ", boomTilt  : " + joints.boomTilt.actuator.CurrentPosition
            //        + ", swing     : " + joints.swing.actuator.CurrentPosition
            //        + ", leftSprocket : " + joints.leftSprocket.actuator.CurrentPosition
            //        + ", rightSprocket: " + joints.rightSprocket.actuator.CurrentPosition);


            if (GlobalVariables.SetupJointFlag && !completedFlag)
            {
                // 重機のゲームオブジェクト
                var shovelObj = this.gameObject;

                // JSON形式で保存したデータを読込
                var json_ms = GlobalVariables.saveMachines;

                // ControlTypeはSpeed（関節が移動する角速度を入力する）
                input.controlType = ControlType.Speed;
                input.movementControlType = ConstractionMovementControlType.ActuatorCommand;
                
                joints.bucketTilt.actuator.controlType = ControlType.Speed;
                joints.armTilt.actuator.controlType = ControlType.Speed;
                joints.boomTilt.actuator.controlType = ControlType.Speed;
                joints.swing.actuator.controlType = ControlType.Speed;
                joints.leftSprocket.actuator.controlType = ControlType.Speed;
                joints.rightSprocket.actuator.controlType = ControlType.Speed;


                // 許容誤差
                const double error = 0.0174533f; // プラス・マイナス1度くらい許容
                // 動作速度
                const float speed = 0.523599f; // 30度くらいに設定


                // 差分格納
                float diff = 0.0f;


                // 動作完了確認
                if (input.bucketCylConv.CalculateCylinderRodTelescoping((float)json_ms.data[0].joint.bucket_joint) - error < joints.bucketTilt.actuator.CurrentPosition &&
                    input.bucketCylConv.CalculateCylinderRodTelescoping((float)json_ms.data[0].joint.bucket_joint) + error > joints.bucketTilt.actuator.CurrentPosition)
                {
                    // 速度ゼロで停止させる
                    joints.bucketTilt.actuator.controlValue = 0.0f;
                    // フラグ切替
                    bucketTiltFlag = true;
                }

                if (input.armCylConv.CalculateCylinderRodTelescoping((float)json_ms.data[0].joint.arm_joint) - error < joints.armTilt.actuator.CurrentPosition &&
                    input.armCylConv.CalculateCylinderRodTelescoping((float)json_ms.data[0].joint.arm_joint) + error > joints.armTilt.actuator.CurrentPosition)
                {
                    // 速度ゼロで停止させる
                    joints.armTilt.actuator.controlValue = 0.0f;
                    // フラグ切替
                    armTiltFlag = true;
                }

                if (input.boomCylConv.CalculateCylinderRodTelescoping((float)json_ms.data[0].joint.boom_joint) - error < joints.boomTilt.actuator.CurrentPosition &&
                    input.boomCylConv.CalculateCylinderRodTelescoping((float)json_ms.data[0].joint.boom_joint) + error > joints.boomTilt.actuator.CurrentPosition)
                {
                    // 速度ゼロで停止させる
                    joints.boomTilt.actuator.controlValue = 0.0f;
                    // フラグ切替
                    boomTiltFlag = true;
                }

                if (json_ms.data[0].joint.swing_joint - error < joints.swing.actuator.CurrentPosition &&
                    json_ms.data[0].joint.swing_joint + error > joints.swing.actuator.CurrentPosition)
                {
                    // 速度ゼロで停止させる
                    joints.swing.actuator.controlValue = 0.0f;
                    // フラグ切替
                    swingFlag = true;
                }


                // 全軸動作完了確認
                if (bucketTiltFlag && armTiltFlag && boomTiltFlag && swingFlag)
                {
                    // 直接制御を解除
                    input.movementControlType = ConstractionMovementControlType.TwistCommand;

                    // 動作完了フラグ
                    completedFlag = true;
                    GlobalVariables.SetupJointCompletedFlag = true;

                    UnityEngine.Debug.Log("Completed!!: " + GlobalVariables.SetupJointCompletedFlag);


                    // ダンプトラックが存在しない場合にフラグを切り替える
                    if ((int)GlobalVariables.Dump_ObjList.Count == 0 && GlobalVariables.SetupJointDumpCount == 0)
                    {
                        GlobalVariables.SetupJointDumpCompletedFlag = true;
                    }
                }


                UnityEngine.Debug.Log("Flag: " + bucketTiltFlag + ", " + armTiltFlag + ", " + boomTiltFlag + ", " + swingFlag);


                // 軸に値をセット
                if (!bucketTiltFlag && armTiltFlag)
                {
                    diff = (float)(joints.bucketTilt.actuator.CurrentPosition - input.bucketCylConv.CalculateCylinderRodTelescoping((float)json_ms.data[0].joint.bucket_joint));
                    joints.bucketTilt.actuator.controlValue = setValue(diff, speed);
                }

                if (!armTiltFlag && boomTiltFlag)
                {
                    diff = (float)(joints.armTilt.actuator.CurrentPosition - input.armCylConv.CalculateCylinderRodTelescoping((float)json_ms.data[0].joint.arm_joint));
                    joints.armTilt.actuator.controlValue = setValue(diff, speed);
                }

                if (!boomTiltFlag && swingFlag)
                {
                    diff =(float)(joints.boomTilt.actuator.CurrentPosition - input.boomCylConv.CalculateCylinderRodTelescoping((float)json_ms.data[0].joint.boom_joint));
                    //joints.boomTilt.actuator.controlValue = input.boomCylConv.CalculateCylinderRodTelescopingVelocity(setValue(diff, speed));
                    joints.boomTilt.actuator.controlValue = setValue(diff, speed);

                    //UnityEngine.Debug.Log("CurrentPosition: " + joints.boomTilt.actuator.CurrentPosition);
                    //UnityEngine.Debug.Log("diff: " + diff);
                    //UnityEngine.Debug.Log("controlValue: " + joints.boomTilt.actuator.controlValue);
                }

                if (!swingFlag)
                {
                    diff = (float)(joints.swing.actuator.CurrentPosition - json_ms.data[0].joint.swing_joint);
                    joints.swing.actuator.controlValue = setValue(diff, speed);
                }

                joints.UpdateConstraintControls();
            }


            if (!GlobalVariables.SetupJointFlag && completedFlag)
            {
                completedFlag = false;
                bucketTiltFlag = false;
                armTiltFlag = false;
                boomTiltFlag = false;
                swingFlag = false;
            }
        }

        float setValue(float diff, float speed) {
            
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
