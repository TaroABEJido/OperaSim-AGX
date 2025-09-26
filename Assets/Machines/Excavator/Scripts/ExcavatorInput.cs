using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Drawing.Text;

namespace PWRISimulator.ROS
{
    public class ExcavatorInput : MonoBehaviour
    {
        public ExcavatorFrontSubscriber frontSubscriber;
        public TrackMessageSubscriber trackSubscriber;
        public ExcavatorSettingSubscriber settingSubscriber;
        [SerializeField] ConstractionMovementControlType movementControlType;
        [SerializeField] ControlType controlType = ControlType.Position;

        private ExcavatorJoints joints;

        // for Joint Command
        public BoomAngleToCylinderLengthConvertor boomCylConv;
        public ArmAngleToCylinderLengthConvertor armCylConv;
        public BucketAngleToCylinderLengthConvertor bucketCylConv;

        // for Twist Command
        public TrackTwistCommandConvertor twistCommandConvertor;

        public float swingKappa;
        public float boomKappa;
        public float armKappa;
        public float bucketKappa;

        // Mapping for joint_name → index num 
        private readonly Dictionary<string, int> _frontIndexMap = new Dictionary<string, int>(System.StringComparer.Ordinal);
        private const string JOINT_BUCKET = "bucket_joint";
        private const string JOINT_ARM    = "arm_joint";
        private const string JOINT_BOOM   = "boom_joint";
        private const string JOINT_SWING  = "swing_joint";

        void Start()
        {
            joints = gameObject.GetComponent<ExcavatorJoints>();
        }
        void FixedUpdate()
        {
            // 受信
            double currentTime = Time.fixedTimeAsDouble - Time.fixedDeltaTime;

            frontSubscriber.ExecuteSubscriptionHandlerActions(currentTime);
            trackSubscriber.ExecuteSubscriptionHandlerActions(currentTime);
            settingSubscriber.ExecuteSubscriptionHandlerActions(currentTime);

            BuildFrontJointIndexMap();
        }

        // Making a map (Dictionary) of Front_cmd's joint_name <-> position/velocity/effort
        private void BuildFrontJointIndexMap()
        {
            var msg = frontSubscriber?.FrontCmd;
            if (msg == null || msg.joint_name == null) return;

            _frontIndexMap.Clear();
            for (int i = 0; i < msg.joint_name.Length; i++)
            {
                var name = msg.joint_name[i];
                if (string.IsNullOrEmpty(name)) continue;
                if (!_frontIndexMap.ContainsKey(name))
                    _frontIndexMap.Add(name, i);
            }
        }

        // Get the position/velocity/effort corresponding to joint_name in front_cmd
        private bool TryGetJointValue(double[] arr, string jointName, out double value)
        {
            value = 0.0;
            if (arr == null) return false;
            if (_frontIndexMap.TryGetValue(jointName, out int idx))
            {
                if (idx >= 0 && idx < arr.Length)
                {
                    value = arr[idx];
                    return true;
                }
            }
            return false;
        }

        public void SetCommands()
        {
            // 制御値の反映
            if (settingSubscriber.EmergencyStopCmd)
            {
                // 緊急停止
                joints.bucketTilt.actuator.controlType = ControlType.Position;
                joints.bucketTilt.actuator.controlValue = joints.bucketTilt.actuator.CurrentPosition;

                joints.armTilt.actuator.controlType = ControlType.Position;
                joints.armTilt.actuator.controlValue = joints.armTilt.actuator.CurrentPosition;

                joints.boomTilt.actuator.controlType = ControlType.Position;
                joints.boomTilt.actuator.controlValue = joints.boomTilt.actuator.CurrentPosition;

                joints.swing.actuator.controlType = ControlType.Position;
                joints.swing.actuator.controlValue = joints.swing.actuator.CurrentPosition;

                joints.leftSprocket.actuator.controlType = ControlType.Position;
                joints.leftSprocket.actuator.controlValue = joints.leftSprocket.actuator.CurrentPosition;

                joints.rightSprocket.actuator.controlType = ControlType.Position;
                joints.rightSprocket.actuator.controlValue = joints.rightSprocket.actuator.CurrentPosition;
            }
            else
            {
                var cmd = frontSubscriber.FrontCmd;
                // 上部旋回体
                switch (controlType)
                {
                    case ControlType.Position:
                        if (TryGetJointValue(cmd.position, JOINT_BUCKET, out double bucketPos))
                        {
                            joints.bucketTilt.actuator.controlType = ControlType.Position;
                            joints.bucketTilt.actuator.controlValue = bucketCylConv.CalculateCylinderRodTelescoping((float)bucketPos);
                        }

                        if (TryGetJointValue(cmd.position, JOINT_ARM, out double armPos))
                        {
                            joints.armTilt.actuator.controlType = ControlType.Position;
                            joints.armTilt.actuator.controlValue = armCylConv.CalculateCylinderRodTelescoping((float)armPos);
                        }

                        if (TryGetJointValue(cmd.position, JOINT_BOOM, out double boomPos))
                        {
                            joints.boomTilt.actuator.controlType = ControlType.Position;
                            joints.boomTilt.actuator.controlValue = boomCylConv.CalculateCylinderRodTelescoping((float)boomPos);
                        }

                        if (TryGetJointValue(cmd.position, JOINT_SWING, out double swingPos))
                        {
                            joints.swing.actuator.controlType = ControlType.Position;
                            joints.swing.actuator.controlValue = swingPos;
                        }
                        break;
                    case ControlType.Speed:
                        if (TryGetJointValue(cmd.velocity, JOINT_BUCKET, out double bucketVel))
                        {
                            joints.bucketTilt.actuator.controlType = ControlType.Speed;
                            joints.bucketTilt.actuator.controlValue = bucketCylConv.CalculateCylinderRodTelescopingVelocity((float)bucketVel);
                        }

                        if (TryGetJointValue(cmd.velocity, JOINT_ARM, out double armVel))
                        {
                            joints.armTilt.actuator.controlType = ControlType.Speed;
                            joints.armTilt.actuator.controlValue = armCylConv.CalculateCylinderRodTelescopingVelocity((float)armVel);
                        }

                        if (TryGetJointValue(cmd.velocity, JOINT_BOOM, out double boomVel))
                        {
                            joints.boomTilt.actuator.controlType = ControlType.Speed;
                            joints.boomTilt.actuator.controlValue = boomCylConv.CalculateCylinderRodTelescopingVelocity((float)boomVel);
                        }

                        if (TryGetJointValue(cmd.velocity, JOINT_SWING, out double swingVel))
                        {
                            joints.swing.actuator.controlType = ControlType.Speed;
                            // 既存ロジック踏襲: 正方向のみ0.52倍
                            joints.swing.actuator.controlValue = (swingVel > 0.0) ? (swingVel * 0.52) : swingVel;
                        }
                        break;
                    case ControlType.Force:
                        if (TryGetJointValue(cmd.effort, JOINT_BUCKET, out double bucketEff))
                        {
                            // bucketEff = 356270.3514f * (float)bucketEff;
                            bucketEff = bucketKappa * (float)bucketEff;
                            joints.bucketTilt.actuator.controlType = ControlType.Force;
                            joints.bucketTilt.actuator.controlValue = bucketCylConv.CalculateCylinderRodTelescopingForce((float)bucketEff);
                        }

                        if (TryGetJointValue(cmd.effort, JOINT_ARM, out double armEff))
                        {
                            // armEff = 490966.1364f * (float)armEff;
                            armEff = armKappa * (float)armEff;
                            joints.armTilt.actuator.controlType = ControlType.Force;
                            joints.armTilt.actuator.controlValue = armCylConv.CalculateCylinderRodTelescopingForce((float)armEff);
                        }

                        if (TryGetJointValue(cmd.effort, JOINT_BOOM, out double boomEff))
                        {
                            // boomEff = 775847.7217f * (float)boomEff;
                            boomEff = -boomKappa * (float)boomEff;
                            joints.boomTilt.actuator.controlType = ControlType.Force;
                            joints.boomTilt.actuator.controlValue = boomCylConv.CalculateCylinderRodTelescopingForce((float)boomEff);
                        }

                        if (TryGetJointValue(cmd.effort, JOINT_SWING, out double swingEff))
                        {
                            // swingEff = 68000.0f * (float)swingEff;
                            swingEff = swingKappa * (float)swingEff;
                            joints.swing.actuator.controlType = ControlType.Force;
                            joints.swing.actuator.controlValue = swingEff;
                        }
                        break;
                    default:
                        break;
                }

                // 下部走行体（従来処理のまま）
                switch (movementControlType)
                {
                    case ConstractionMovementControlType.ActuatorCommand:
                        switch (controlType)
                        {
                            case ControlType.Position:
                                joints.leftSprocket.actuator.controlType = ControlType.Position;
                                joints.leftSprocket.actuator.controlValue = trackSubscriber.TrackCmd.position[0];

                                joints.rightSprocket.actuator.controlType = ControlType.Position;
                                joints.rightSprocket.actuator.controlValue = trackSubscriber.TrackCmd.position[1];
                                break;
                            case ControlType.Speed:
                                joints.leftSprocket.actuator.controlType = ControlType.Speed;
                                joints.leftSprocket.actuator.controlValue = trackSubscriber.TrackCmd.velocity[0];

                                joints.rightSprocket.actuator.controlType = ControlType.Speed;
                                joints.rightSprocket.actuator.controlValue = trackSubscriber.TrackCmd.velocity[1];
                                break;
                            case ControlType.Force:
                                joints.leftSprocket.actuator.controlType = ControlType.Force;
                                joints.leftSprocket.actuator.controlValue = trackSubscriber.TrackCmd.effort[0];

                                joints.rightSprocket.actuator.controlType = ControlType.Force;
                                joints.rightSprocket.actuator.controlValue = trackSubscriber.TrackCmd.effort[1];
                                break;
                            default:
                                break;
                        }
                        break;
                    case ConstractionMovementControlType.TwistCommand:
                        twistCommandConvertor.SetCommand(trackSubscriber.VelocityCmd.linear, trackSubscriber.VelocityCmd.angular);

                        joints.leftSprocket.actuator.controlType = ControlType.Speed;
                        joints.leftSprocket.actuator.controlValue = twistCommandConvertor.sprocketSpeed_L;

                        joints.rightSprocket.actuator.controlType = ControlType.Speed;
                        joints.rightSprocket.actuator.controlValue = twistCommandConvertor.sprocketSpeed_R;
                        break;
                    case ConstractionMovementControlType.VolumeCommand:
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
