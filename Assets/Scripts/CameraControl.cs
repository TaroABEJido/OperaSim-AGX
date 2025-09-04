using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PWRISimulator
{
    public class CameraControl : MonoBehaviour
    {
        [SerializeField, Range(0.1f, 20f)]
        private float wheelSpeed = 15.0f;

        [SerializeField, Range(0.1f, 100f)]
        private float moveSpeed = 3.0f;

        [SerializeField, Range(0.1f, 5f)]
        private float rotateSpeed = 0.1f;

        private Vector3 preMousePos;

        private void Update()
        {
            if (GlobalVariables.CameraSelected == true) return;
            if (GlobalVariables.SetMoveType == 3)
            {
                MouseUpdate();
            }
            return;
        }

        private void MouseUpdate()
        {

            if (GlobalVariables.CameraSelected == true) return;

            float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
            if (scrollWheel != 0.0f)
                MouseWheel(scrollWheel);

            if (Input.GetMouseButtonDown(0) ||
               Input.GetMouseButtonDown(1) ||
               Input.GetMouseButtonDown(2)) {
                preMousePos = Input.mousePosition;
            }

            if (Input.GetMouseButtonUp(0) ||
               Input.GetMouseButtonUp(1) ||
               Input.GetMouseButtonUp(2)){
                preMousePos = Input.mousePosition;
                return;
            }

            MouseDrag(Input.mousePosition);
        }

        private void MouseWheel(float delta)
        {
            transform.position += transform.forward * delta * wheelSpeed;
            return;
        }

        private void MouseDrag(Vector3 mousePos)
        {
            Vector3 diff = mousePos - preMousePos;

            if (diff.magnitude < Vector3.kEpsilon)
                return;

            if (Input.GetMouseButton(0))
                transform.Translate(-diff * Time.deltaTime * moveSpeed);
            else if (Input.GetMouseButton(1))
                CameraRotate(new Vector2(-diff.y, diff.x) * rotateSpeed);

            preMousePos = mousePos;
        }

        public void CameraRotate(Vector2 angle)
        {
            transform.RotateAround(transform.position, transform.right, angle.x);
            transform.RotateAround(transform.position, Vector3.up, angle.y);
        }
    }
}