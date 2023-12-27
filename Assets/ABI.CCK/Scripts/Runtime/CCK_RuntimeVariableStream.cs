using System;
using System.Collections;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Abi.Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace ABI.CCK.Scripts.Runtime
{
    [AddComponentMenu("")]
    public class CCK_RuntimeVariableStream : MonoBehaviour
    {
        private void Start()
        {
            StartCoroutine(StreamVars());
        }

        private IEnumerator StreamVars()
        {
            OnGuiUpdater updater = gameObject.GetComponent<OnGuiUpdater>();
            string type = updater.asset.type.ToString();
            string uploadRegion = "0";
#if UNITY_EDITOR
            APIConnection.Initialize(EditorPrefs.GetString("m_ABI_Username"), EditorPrefs.GetString("m_ABI_Key"));
            EditorPrefs.GetInt("ABI_PREF_UPLOAD_REGION").ToString();
#endif
            
            Task<APIConnection.BaseResponse<APIConnection.ContentInfoResponse>> task = Task.Run(() => APIConnection.MakeRequest<APIConnection.ContentInfoResponse>(
                $"cck/contentInfo/{type}/{updater.asset.objectId}?platform=pc_standalone&region={uploadRegion}"));
            
            while (!task.IsCompleted) yield return null;

            APIConnection.BaseResponse<APIConnection.ContentInfoResponse> response = task.Result;

            if (response != null)
            {
                if (response.Data != null)
                {
                    updater.UploadLocation = response.Data.UploadLocation;

                    Debug.Log($"Upload Location: {updater.UploadLocation}");

                    updater.assetName.text = response.Data.ContentData.Name;
                    updater.assetDesc.text = response.Data.ContentData.Description;

                    updater.LoudAudio.isOn = response.Data.ContentData.Tags.LoudAudio;
                    updater.LongRangeAudio.isOn = response.Data.ContentData.Tags.LongRangeAudio;
                    updater.SpawnAudio.isOn = response.Data.ContentData.Tags.SpawnAudio;
                    updater.ContainsMusic.isOn = response.Data.ContentData.Tags.ContainsMusic;

                    updater.ScreenEffects.isOn = response.Data.ContentData.Tags.ScreenEffects;
                    updater.FlashingColors.isOn = response.Data.ContentData.Tags.FlashingColors;
                    updater.FlashingLights.isOn = response.Data.ContentData.Tags.FlashingLights;
                    updater.ExtremelyBright.isOn = response.Data.ContentData.Tags.ExtremelyBright;
                    updater.ParticleSystems.isOn = response.Data.ContentData.Tags.ParticleSystems;

                    updater.Violence.isOn = response.Data.ContentData.Tags.Violence;
                    updater.Gore.isOn = response.Data.ContentData.Tags.Gore;
                    updater.Horror.isOn = response.Data.ContentData.Tags.Horror;
                    updater.Jumpscare.isOn = response.Data.ContentData.Tags.Jumpscare;

                    updater.ExcessivelySmall.isOn = response.Data.ContentData.Tags.ExtremelySmall;
                    updater.ExcessivelyHuge.isOn = response.Data.ContentData.Tags.ExtremelyHuge;

                    updater.Suggestive.isOn = response.Data.ContentData.Tags.Suggestive;
                    updater.Nudity.isOn = response.Data.ContentData.Tags.Nudity;
                }
                else
                {
#if UNITY_EDITOR
                    EditorUtility.ClearProgressBar();
                    if (UnityEditor.EditorUtility.DisplayDialog("Alpha Blend Interactive CCK",
                        response.Message, "Okay"))
                    {
                        EditorApplication.isPlaying = false;
                    }
#endif
                    yield break;
                }
            }
            else
            {
#if UNITY_EDITOR
                EditorUtility.ClearProgressBar();
                if (UnityEditor.EditorUtility.DisplayDialog("Alpha Blend Interactive CCK",
                    "An Error occured while uploading. Please try again later.", "Okay"))
                {
                    EditorApplication.isPlaying = false;
                }
#endif
                yield break;
            }
        }
    }
}