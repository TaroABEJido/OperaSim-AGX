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

        // �S�����슮���t���O
        private bool completedFlag = false;

        // �e���̓��슮���t���O
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

            // �N���X�ǂݍ���
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
                // �d�@�̃Q�[���I�u�W�F�N�g
                var shovelObj = this.gameObject;

                // JSON�`���ŕۑ������f�[�^��Ǎ�
                var json_ms = GlobalVariables.saveMachines;

                // ControlType��Speed�i�֐߂��ړ�����p���x����͂���j
                input.controlType = ControlType.Speed;
                input.movementControlType = ConstractionMovementControlType.ActuatorCommand;
                
                joints.bucketTilt.actuator.controlType = ControlType.Speed;
                joints.armTilt.actuator.controlType = ControlType.Speed;
                joints.boomTilt.actuator.controlType = ControlType.Speed;
                joints.swing.actuator.controlType = ControlType.Speed;
                joints.leftSprocket.actuator.controlType = ControlType.Speed;
                joints.rightSprocket.actuator.controlType = ControlType.Speed;


                // ���e�덷
                const double error = 0.0174533f; // �v���X�E�}�C�i�X1�x���炢���e
                // ���쑬�x
                const float speed = 0.523599f; // 30�x���炢�ɐݒ�


                // �����i�[
                float diff = 0.0f;


                // ���슮���m�F
                if (input.bucketCylConv.CalculateCylinderRodTelescoping((float)json_ms.data[0].joint.bucket_joint) - error < joints.bucketTilt.actuator.CurrentPosition &&
                    input.bucketCylConv.CalculateCylinderRodTelescoping((float)json_ms.data[0].joint.bucket_joint) + error > joints.bucketTilt.actuator.CurrentPosition)
                {
                    // ���x�[���Œ�~������
                    joints.bucketTilt.actuator.controlValue = 0.0f;
                    // �t���O�ؑ�
                    bucketTiltFlag = true;
                }

                if (input.armCylConv.CalculateCylinderRodTelescoping((float)json_ms.data[0].joint.arm_joint) - error < joints.armTilt.actuator.CurrentPosition &&
                    input.armCylConv.CalculateCylinderRodTelescoping((float)json_ms.data[0].joint.arm_joint) + error > joints.armTilt.actuator.CurrentPosition)
                {
                    // ���x�[���Œ�~������
                    joints.armTilt.actuator.controlValue = 0.0f;
                    // �t���O�ؑ�
                    armTiltFlag = true;
                }

                if (input.boomCylConv.CalculateCylinderRodTelescoping((float)json_ms.data[0].joint.boom_joint) - error < joints.boomTilt.actuator.CurrentPosition &&
                    input.boomCylConv.CalculateCylinderRodTelescoping((float)json_ms.data[0].joint.boom_joint) + error > joints.boomTilt.actuator.CurrentPosition)
                {
                    // ���x�[���Œ�~������
                    joints.boomTilt.actuator.controlValue = 0.0f;
                    // �t���O�ؑ�
                    boomTiltFlag = true;
                }

                if (json_ms.data[0].joint.swing_joint - error < joints.swing.actuator.CurrentPosition &&
                    json_ms.data[0].joint.swing_joint + error > joints.swing.actuator.CurrentPosition)
                {
                    // ���x�[���Œ�~������
                    joints.swing.actuator.controlValue = 0.0f;
                    // �t���O�ؑ�
                    swingFlag = true;
                }


                // �S�����슮���m�F
                if (bucketTiltFlag && armTiltFlag && boomTiltFlag && swingFlag)
                {
                    // ���ڐ��������
                    input.movementControlType = ConstractionMovementControlType.TwistCommand;

                    // ���슮���t���O
                    completedFlag = true;
                    GlobalVariables.SetupJointCompletedFlag = true;

                    UnityEngine.Debug.Log("Completed!!: " + GlobalVariables.SetupJointCompletedFlag);


                    // �_���v�g���b�N�����݂��Ȃ��ꍇ�Ƀt���O��؂�ւ���
                    if ((int)GlobalVariables.Dump_ObjList.Count == 0 && GlobalVariables.SetupJointDumpCount == 0)
                    {
                        GlobalVariables.SetupJointDumpCompletedFlag = true;
                    }
                }


                UnityEngine.Debug.Log("Flag: " + bucketTiltFlag + ", " + armTiltFlag + ", " + boomTiltFlag + ", " + swingFlag);


                // ���ɒl���Z�b�g
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
