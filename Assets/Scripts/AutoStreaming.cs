using System;
using System.Linq;
using Unity.Collections;
using Unity.RenderStreaming.InputSystem;
using Unity.WebRTC;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.SceneManagement;
using Unity.RenderStreaming;

namespace PWRISimulator
{
    public class AutoStreaming : MonoBehaviour
    {

        private VideoStreamSender videoStreamSender;

        private void Awake()
        {
            gameObject.hideFlags = HideFlags.HideInHierarchy;

            videoStreamSender = gameObject.AddComponent<VideoStreamSender>();
            videoStreamSender.source = VideoStreamSource.Screen;
            videoStreamSender.SetTextureSize(new Vector2Int(Screen.width, Screen.height));

            SceneManager.activeSceneChanged += (scene1, scene2) =>
            {
                var audioListener = FindObjectOfType<AudioListener>();
                if (audioListener == null || audioListener.gameObject.GetComponent<AutoAudioFilter>() != null)
                {
                    return;
                }

                var autoFilter = audioListener.gameObject.AddComponent<AutoAudioFilter>();
            };
        }


        private void Start()
        {

            var CSH  = FindObjectsOfType<CameraSignalingHandler>();
            if (CSH != null)
            {
                //UnityEngine.Debug.LogError("CSH Not NULL");
                CSH[0].CameraStreamList.Add(videoStreamSender);
            }
            else
            {
                UnityEngine.Debug.LogError("CSH NULL");
            }

        }

        private void Update()
        {
            
            


        }

        private void OnDestroy()
        {
            //renderstreaming.Stop();
            //renderstreaming = null;
            //broadcast = null;
            videoStreamSender = null;
            //audioStreamSender = null;
            //inputReceiver = null;
        }

        class AutoAudioFilter : MonoBehaviour
        {
            private AudioStreamSender sender;

            public void SetSender(AudioStreamSender sender)
            {
                this.sender = sender;
            }

            private void Awake()
            {
                this.hideFlags = HideFlags.HideInInspector;
            }

            private void OnAudioFilterRead(float[] data, int channels)
            {
                if (sender == null || sender.source != AudioStreamSource.APIOnly)
                {
                    return;
                }

                var nativeArray = new NativeArray<float>(data, Allocator.Temp);
                sender.SetData(nativeArray.AsReadOnly(), channels);
                nativeArray.Dispose();
            }

            private void OnDestroy()
            {
                sender = null;
            }
        }

 
    }
}
