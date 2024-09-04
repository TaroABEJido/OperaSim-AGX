using System.Collections;
using System.Collections.Generic;
using RosMessageTypes.Geometry;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using UnityEngine;

namespace PWRISimulator.ROS
{
    /// <summary>
    /// ブルドーザのglobal_pose
    /// グローバル座標系における車両の中心位置および速度
    /// </summary>
    public class BulldozerGlobalPosePublisher : OdometryPublisher
    {
        [SerializeField] BulldozerJoints bulldozerJoint;
        [SerializeField] uint frequency = 60;
        private double previousTime = 0;
        protected override void DoUpdate()
        {
            double time = Time.fixedTimeAsDouble;
            double deltaTime = time - previousTime;

            if (time > 0 && deltaTime > 0)
            {

                GameObject trackLink = bulldozerJoint.gameObject.GetComponentInChildren<AGXUnity.Model.Track>().gameObject;
                MessageUtil.UpdateTimeMsg(odometryMsg.header.stamp, time);

                odometryMsg.header.frame_id="world";
                odometryMsg.child_frame_id="d37pxi_tf/base_link";
                odometryMsg.pose.pose.position = trackLink.transform.position.To<FLU>();
                odometryMsg.pose.pose.orientation = trackLink.transform.rotation.To<FLU>();
                previousTime = time;
            }
        }

        protected override string MachineName()
        {
            return this.gameObject.name;
        }
        protected override string TopicPhrase()
        {
            return "/global_pose";
        }
        protected override uint Frequency()
        {
            return frequency;
        }
    }
}
