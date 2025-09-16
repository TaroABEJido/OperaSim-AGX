using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.RenderStreaming;
using Unity.WebRTC;
using System.Linq;

namespace PWRISimulator
{
    public class CameraSignalingHandler : SignalingHandlerBase, IOfferHandler, IDisconnectHandler, IDeletedConnectionHandler
    {
        public List<VideoStreamSender> CameraStreamList = new List<VideoStreamSender>();
        //public GameObject CameraGroup;

        private List<string> connectionIds = new List<string>();

        //rtiStreamId�Ŏw�肵��index�ɑΉ�����VideoStreamSender��Ԃ�
        private GameObject GetSelectedVideoStreamSender(string rtiStreamId)
        {
            var items = CameraStreamList;
            UnityEngine.Debug.Log(items.Count);

            int streamIndex = int.Parse(rtiStreamId);

            //�w�肵��Index������VideoStreamSender��Ԃ�
            if (streamIndex < items.Count)
            {
                UnityEngine.Debug.Log("VideoStreamSender Obj (GetSelectedVideoStreamSender) : " + items[streamIndex].gameObject.name);
                return items[streamIndex].gameObject;
            }

            //�Ή�����index�����݂��Ȃ��ꍇ�ɂ̓��X�g�̍ŏ���VideoStreamSender��Ԃ�
            UnityEngine.Debug.Log("Error: Selected Stream Index is Out Of Number Of VideoStremaSenders.");
            return items[0].gameObject;

        }


        //Offer����
        public void OnOffer(SignalingEventData data)
        {
            //VideoStreamSender�I�u�W�F�N�g�����������X�g�ɓo�^
            var items = FindObjectsOfType<VideoStreamSender>();
            foreach (var item in items)
            {
                UnityEngine.Debug.Log("VideoStreamSender Obj (OnOffer) : " + item.gameObject.name);
                if (!CameraStreamList.Contains(item)) CameraStreamList.Add(item);
            }

            //Connection ID�̊Ǘ�
            if (connectionIds.Contains(data.connectionId))
            {
                UnityEngine.Debug.Log("Already answered this connectionId : " + data.connectionId);
                return;
            }
            connectionIds.Add(data.connectionId);

            //sdp�f�[�^��v�f���ɕ������Asdp�f�[�^�Ɋ܂܂�Ă���rid���擾
            var sdpList = data.sdp.Split("\r\n");
            var results = sdpList.Where(line => line.Contains("a=rid"));
            var res = "";
            foreach (var line in results)
            {
                res = line;
                break;
            }

            GameObject gameObject = null;

            //rid���擾�\�ł����rid�ɑΉ�����VideoStreamSender���擾
            if (res != "")
            {
                UnityEngine.Debug.Log("test2 : " + res.Split("=")[1].Split(":")[1]);
                gameObject = GetSelectedVideoStreamSender(res.Split("=")[1].Split(":")[1]);
                if (gameObject == null)
                {
                    UnityEngine.Debug.Log($"Sorry, no more streams available to assign to connectionId : {data.connectionId}");
                    return;
                }

            }
            else
            { //rid���擾�ł��Ȃ����List�̐擪��VideoStreamSender���擾
                gameObject = GetSelectedVideoStreamSender("0"); ;
                if (gameObject == null)
                {
                    UnityEngine.Debug.Log($"Sorry, no more streams available to assign to connectionId : {data.connectionId}");
                    return;
                }
            }

            UnityEngine.Debug.Log($"Found an available stream, adding to connectionId : {data.connectionId}");

            gameObject.GetComponent<VideoStreamSenderProp>().connectionId = data.connectionId;

            //gameObject.name = data.connectionId;
            //gameObject.SetActive(true);

            AddSender(data.connectionId, gameObject.GetComponent<VideoStreamSender>());
            SendAnswer(data.connectionId);
        }




        //connectionId�Ŏw�肵��ConnectionID�����A�N�e�B�u��VideoStreamSender��Ԃ�
        private GameObject FindActiveVideoStreamSender(string connectionId)
        {
            //var items = CameraGroup.GetComponentsInChildren<VideoStreamSender>();
            var items = FindObjectsOfType<VideoStreamSender>();

            foreach (var item in items)
            {
                UnityEngine.Debug.Log("VideoStreamSender Obj (FindActiveVideoStreamSender) : " + item.gameObject.name);
                UnityEngine.Debug.Log("VideoStreamSender Obj (FindActiveVideoStreamSender) : " + item.gameObject.GetComponent<VideoStreamSenderProp>().connectionId);
                //if (item.gameObject.name == connectionId) return item.gameObject;
                if (item.gameObject.GetComponent<VideoStreamSenderProp>().connectionId == connectionId) return item.gameObject;
            }

            return null;
        }



        //�ڑ��ؒf����
        private void Disconnect(string connectionId)
        {
            if (!connectionIds.Contains(connectionId))
            {
                return;
            }

            // Find and disable the game object
            //FindActiveVideoStreamSender(connectionId)?.SetActive(false);

            connectionIds.Remove(connectionId);
        }

        //�ڑ��ؒf
        public void OnDeletedConnection(SignalingEventData eventData)
        {
            Disconnect(eventData.connectionId);
        }

        //�N���C�A���g������ڑ��ؒf
        public void OnDisconnect(SignalingEventData eventData)
        {
            Disconnect(eventData.connectionId);
        }
    }
}
