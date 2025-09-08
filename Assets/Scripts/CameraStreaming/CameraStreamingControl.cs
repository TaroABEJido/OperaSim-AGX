using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.RenderStreaming;
using Unity.WebRTC;

namespace PWRISimulator
{
    public class CameraStreamingControl : MonoBehaviour
    {

        public GameObject CameraGroup;

        void Awake()
        {

            var items = CameraGroup.GetComponentsInChildren<VideoStreamSender>(true);
            foreach (var item in items)
            {
                UnityEngine.Debug.Log("VideoStreamSender Obj (CSCont Awake) : " + item.gameObject.name);
                UnityEngine.Debug.Log("VideoStreamSender Track ID (CSCont Awake) : " + item.name);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            var items = CameraGroup.GetComponentsInChildren<VideoStreamSender>(true);
            foreach (var item in items)
            {
                UnityEngine.Debug.Log("VideoStreamSender Obj (CSCont Start) : " + item.gameObject.name);
            }

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
