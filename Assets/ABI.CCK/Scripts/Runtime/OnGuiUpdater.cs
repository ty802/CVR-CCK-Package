using System;
using System.IO;
using ABI.CCK.Components;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace ABI.CCK.Scripts.Runtime
{
    [AddComponentMenu("")]
    public class OnGuiUpdater : MonoBehaviour
    {
        [Space] [Header("Object details")] [Space]
        public Text uiTitle;
        public InputField assetName;
        public InputField assetDesc;
        public InputField assetChangelog;
        
        [Space] [Header("Object tags")] [Space]
        public Toggle LoudAudio;
        public Toggle LongRangeAudio;
        public Toggle SpawnAudio;
        public Toggle ContainsMusic;
        public Toggle ScreenEffects;
        public Toggle FlashingColors;
        public Toggle FlashingLights;
        public Toggle ExtremelyBright;
        public Toggle ParticleSystems;
        public Toggle Violence;
        public Toggle Gore;
        public Toggle Horror;
        public Toggle Jumpscare;
        public Toggle ExcessivelyHuge;
        public Toggle ExcessivelySmall;
        public Toggle Suggestive;
        public Toggle Nudity;

        public Toggle dontOverridePicture;
        public Toggle SetAsActive;
        
        //Regulatory
        public Toggle contentOwnership;
        public Toggle tagsCorrect;
        
        [Space] [Header("Object reference")] [Space]
        public CVRAssetInfo asset;

        [Space] [Header("UIHelper objects")] [Space]
        public GameObject camObj;
        public RenderTexture tex;
        public RawImage texView;
        public RawImage texViewBig;
        public GameObject tagsObject;
        public GameObject detailsObject;
        public GameObject legalObject;
        public GameObject uploadObject;
        public Image stepOne;
        public Image stepTwo;
        public Image stepThree;
        public Image tagsImage;
        public Image detailsImage;
        public Image legalImage;
        public Image uploadImage;
        public Text tagsText;
        public Text detailsText;
        public Text legalText;
        public Text uploadText;
        public Image uploadProgress;
        public Text uploadProgressText;
        public Image processingProgress;
        public Text processingProgressText;
        public Text assetFileSizeText;
        public Text assetImageFileSizeText;
        public Text assetFileManifestSizeText;
        public Text assetFilePano1SizeText;
        public Text assetFilePano4SizeText;
        public Text cckVersion;

        [HideInInspector] public string UploadLocation;

        public CCK_RuntimeUploaderMaster uploader;

        private void Start()
        {
            SwitchPage(0);

            CVRAssetInfo.AssetType type = asset.GetComponent<CVRAssetInfo>().type;
            string basePath = Application.persistentDataPath;
            string content_id = asset.objectId;
            string commonPath = $"{basePath}/cvr{type.ToString().ToLower()}_{content_id}_{asset.randomNum}.cvr";
            
            if (type == CVRAssetInfo.AssetType.Avatar || type == CVRAssetInfo.AssetType.World)
                commonPath += type.ToString().ToLower();
            else if (type == CVRAssetInfo.AssetType.Spawnable) commonPath += "prop";

            SetupCCKShotCam();
            texView.texture = tex;
            texViewBig.texture = tex;

            if (type == CVRAssetInfo.AssetType.World)
            {
                Editor.CCK_WorldPreviewCapture.CreatePanoImages();
                assetFileSizeText.text = ToFileSizeString(new FileInfo($"{basePath}/bundle.cvrworld").Length);
                assetImageFileSizeText.text = "N/A";
                assetFileManifestSizeText.text = ToFileSizeString(new FileInfo($"{basePath}/bundle.cvrworld.manifest").Length);
                assetFilePano1SizeText.text = ToFileSizeString(new FileInfo($"{basePath}/bundle_pano_1024.png").Length);
                assetFilePano4SizeText.text = ToFileSizeString(new FileInfo($"{basePath}/bundle_pano_4096.png").Length);
            }
            else
            {
                assetImageFileSizeText.text = type == CVRAssetInfo.AssetType.Avatar ? "N/A" : "";
                assetFilePano1SizeText.text = "N/A";
                assetFilePano4SizeText.text = "N/A";
                assetFileSizeText.text = ToFileSizeString(new FileInfo($"{commonPath}").Length);
                assetFileManifestSizeText.text = ToFileSizeString(new FileInfo($"{commonPath}.manifest").Length);
            }

            cckVersion.text = "ABI Platform Content Creation Kit v3.7 RELEASE (Build 104)";
        }

        public void SwitchPage(int index)
        {
            switch (index)
            {
                case 0:
                    tagsObject.SetActive(true);
                    detailsObject.SetActive(false);
                    legalObject.SetActive(false);
                    uploadObject.SetActive(false);
                    stepOne.color = Color.white;
                    stepTwo.color = Color.white;
                    stepThree.color = Color.white;
                    tagsImage.color = Color.yellow;
                    detailsImage.color = Color.white;
                    legalImage.color = Color.white;
                    uploadImage.color = Color.white;
                    tagsText.color = Color.yellow;
                    detailsText.color = Color.white;
                    legalText.color = Color.white;
                    uploadText.color = Color.white;
                    break;
                case 1:
                    tagsObject.SetActive(false);
                    detailsObject.SetActive(true);
                    legalObject.SetActive(false);
                    uploadObject.SetActive(false);
                    stepOne.color = Color.green;
                    stepTwo.color = Color.white;
                    stepThree.color = Color.white;
                    tagsImage.color = Color.green;
                    detailsImage.color = Color.yellow;
                    legalImage.color = Color.white;
                    uploadImage.color = Color.white;
                    tagsText.color = Color.green;
                    detailsText.color = Color.yellow;
                    legalText.color = Color.white;
                    uploadText.color = Color.white;
                    break;
                case 2:
                    tagsObject.SetActive(false);
                    detailsObject.SetActive(false);
                    legalObject.SetActive(true);
                    uploadObject.SetActive(false);
                    stepOne.color = Color.green;
                    stepTwo.color = Color.green;
                    stepThree.color = Color.white;
                    tagsImage.color = Color.green;
                    detailsImage.color = Color.green;
                    legalImage.color = Color.yellow;
                    uploadImage.color = Color.white;
                    tagsText.color = Color.green;
                    detailsText.color = Color.green;
                    legalText.color = Color.yellow;
                    uploadText.color = Color.white;
                    break;
                case 3:
                    tagsObject.SetActive(false);
                    detailsObject.SetActive(false);
                    legalObject.SetActive(false);
                    uploadObject.SetActive(true);
                    stepOne.color = Color.green;
                    stepTwo.color = Color.green;
                    stepThree.color = Color.green;
                    tagsImage.color = Color.green;
                    detailsImage.color = Color.green;
                    legalImage.color = Color.green;
                    uploadImage.color = Color.yellow;
                    tagsText.color = Color.green;
                    detailsText.color = Color.green;
                    legalText.color = Color.green;
                    uploadText.color = Color.yellow;
                    if (string.IsNullOrEmpty(assetName.text))
                    {
#if UNITY_EDITOR
                        EditorUtility.DisplayDialog("Alpha Blend Interactive CCK", CCKLocalizationProvider.GetLocalizedText("ABI_UI_BUILDSTEP_UPLOAD_DETAILS_MISSING"), "Okay");
#endif
                        SwitchPage(1);
                        return;
                    }

                    if (!contentOwnership.isOn || !tagsCorrect.isOn)
                    {
#if UNITY_EDITOR
                        EditorUtility.DisplayDialog("Alpha Blend Interactive CCK", CCKLocalizationProvider.GetLocalizedText("ABI_UI_BUILDSTEP_UPLOAD_LEGAL_MISSING"), "Okay");
#endif
                        SwitchPage(2);
                        return;
                    }
                    uploader.StartUpload();
                    break;
            }
        }
        
        public void ToggleObject(GameObject obj) => obj.SetActive(!obj.activeInHierarchy);

        #region Private Methods

        private static string ToFileSizeString(long fileSize)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            while (fileSize >= 1024 && order < sizes.Length - 1) {
                order++;
                fileSize = fileSize/1024;
            }
            
            return $"{fileSize:0.##} {sizes[order]}";
        }

        #endregion

        #region Camera Handling

        public void CaptureInSceneImage()
        {
            Camera c = GetOrEnableCamera();
            c.Render();
            c.targetTexture = tex;
            RenderTexture.active = tex;
            SaveAndDisplayTexture();
        }

        public void CaptureImageFromFile()
        {
            if (TryLoadImageFromFile())
                SaveAndDisplayTexture();
        }

        public void ClearTakenImage()
        {
            Texture2D clearTexture = new Texture2D(2, 2);
            clearTexture.SetPixels(new Color[] { Color.white, Color.white, Color.white, Color.white });
            clearTexture.Apply();
            Graphics.Blit(clearTexture, tex);
        }

        private Camera GetOrEnableCamera()
        {
            Camera c = camObj.GetComponent<Camera>();
            if (!c.enabled)
            {
                c.targetTexture = tex;
                c.enabled = true;
            }
            return c;
        }

        private bool TryLoadImageFromFile()
        {
            string path = OpenFileBrowserAndGetFilePathInEditor();
            if (string.IsNullOrEmpty(path))
                return false;

            byte[] imageBytes = File.ReadAllBytes(path);
            Texture2D loadedImage = new Texture2D(2, 2);
            if (!loadedImage.LoadImage(imageBytes)) return false;
            Graphics.Blit(loadedImage, tex);
            return true;
        }

        private void SaveAndDisplayTexture()
        {
            gameObject.GetComponent<CCK_TexImageCreation>().SaveTexture(tex);
            assetImageFileSizeText.text = ToFileSizeString(new FileInfo(Application.persistentDataPath + "/bundle.png").Length);
            camObj.GetComponent<Camera>().enabled = false;
        }

        private string OpenFileBrowserAndGetFilePathInEditor()
        {
        #if UNITY_EDITOR
            return EditorUtility.OpenFilePanel("Select an Image", "", "png,jpg,jpeg");
        #else
            return null;
        #endif
        }

        private void SetupCCKShotCam()
        {
            tex = new RenderTexture(512, 512, 1, RenderTextureFormat.ARGB32);
            tex.Create();

            camObj = new GameObject
            {
                name = "ShotCam for CVR CCK",
                transform =
                {
                    rotation = new Quaternion(0, 180, 0, 0)
                }
            };

            CVRAvatar avatar = asset.GetComponent<CVRAvatar>();
            if (avatar != null && asset.type == CVRAssetInfo.AssetType.Avatar)
                camObj.transform.position = new Vector3(avatar.viewPosition.x, avatar.viewPosition.y, avatar.viewPosition.z *= 5f);

            Camera cam = camObj.AddComponent<Camera>();
            cam.aspect = 1f;
            cam.nearClipPlane = 0.01f;
            cam.targetTexture = tex;
        }

        #endregion

    }
}
