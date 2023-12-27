using System;
using ABI.CCK.Components;
using UnityEngine;

// https://web.archive.org/web/20210507003436/http://wiki.unity3d.com/index.php/MirrorReflection4

[AddComponentMenu("ChilloutVR/CVR Mirror")]
[HelpURL("https://developers.abinteractive.net/cck/components/mirror/")]
[ExecuteInEditMode]
public class CVRMirror : MonoBehaviour, ICCK_Component
{
    public enum MirrorClearFlags {Skybox = 1, Color = 2}
    
    // General
    public bool m_DisablePixelLights = true;
    public int m_TextureSize = 4096;
    public LayerMask m_ReflectLayers = -1;

    // Optimization
    public bool m_UseOcclusionCulling;
    
    // Advanced
    public MirrorClearFlags m_ClearFlags = MirrorClearFlags.Skybox;
    public Material m_CustomSkybox;
    public Color m_CustomColor = new Color(19, 30, 47);

    // Advanced / Why ??
    public float m_ClipPlaneOffset = 0.001f;
    public int m_framesNeededToUpdate;

    // Legacy behaviour forces player layers on + UI off
    public bool m_ignoreLegacyBehaviour;

    private Camera m_ReflectionCamera;
    private RenderTexture m_ReflectionTextureLeft;
    private RenderTexture m_ReflectionTextureRight;
    private MaterialPropertyBlock m_PropertyBlock;
    
    private Renderer m_MirrorRenderer;

    private int m_frameCounter;
    private static bool s_InsideRendering;

    // mirror mesh normal in local coordinates
    private Vector3 mirrorNormal = Vector3.zero;

    // configurable by player in-game
    private int usedTextureSize = 4096;
    private int usedMsaa = 0;

    private static Shader mirrorShader;
    private static readonly int _propertyLeft = Shader.PropertyToID("_ReflectionTexLeft");
    private static readonly int _propertyRight = Shader.PropertyToID("_ReflectionTexRight");
    
#if UNITY_EDITOR
    private void Reset()
    {
        // Ensure new content is not "legacy".
        m_ignoreLegacyBehaviour = true;
    }
    
    public void OnValidate()
    {
        // prevent against infinite reimport when viewing prefabs
        if (!gameObject.scene.IsValid()) 
            return;
        
        CleanupMirrorObjects();
        
        m_MirrorRenderer = GetComponent<Renderer>();
        if (!m_MirrorRenderer)
        {
            enabled = false;
            return;
        }
        
        if (mirrorShader == null)
            mirrorShader = Shader.Find("FX/MirrorReflection");
        
        m_PropertyBlock ??= new MaterialPropertyBlock();
        
        var materials = m_MirrorRenderer.sharedMaterials;
        foreach (Material mat in materials)
        {
            if (mat == null) continue;
            if (mat.shader.name is "FX/MirrorReflection" or "Alpha Blend Interactive/MirrorReflection")
                mat.shader = mirrorShader;
        }
        m_MirrorRenderer.sharedMaterials = materials;
    }
#endif
    
    private void Start()
    {
        LegacyBehaviourIfNeeded();
        
        // Prevent mirrors from reflecting others
        // This is a reserved layer, no prior content should be using it
        gameObject.layer = 14;
        m_ReflectLayers &= ~(1 << 14);

        mirrorShader = Shader.Find("FX/MirrorReflection");
        m_PropertyBlock ??= new MaterialPropertyBlock();

        m_MirrorRenderer = GetComponent<Renderer>();
        if (!m_MirrorRenderer)
        {
            enabled = false;
            return;
        }
        
        var materials = m_MirrorRenderer.sharedMaterials;
        foreach (Material mat in materials)
        {
            if (mat == null) continue;
            if (mat.shader.name is "FX/MirrorReflection" or "Alpha Blend Interactive/MirrorReflection")
                mat.shader = mirrorShader;
        }
        m_MirrorRenderer.sharedMaterials = materials;
    }

    private void OnDisable()
    {
        CleanupMirrorObjects();
    }
    
    private void OnDestroy()
    {
        if (m_ReflectionCamera == null)
            return;

        if (Application.isEditor)
            DestroyImmediate(m_ReflectionCamera.gameObject);
        else
            Destroy(m_ReflectionCamera.gameObject);
    }
    
    private void LegacyBehaviourIfNeeded()
    {
        if (m_ignoreLegacyBehaviour)
            return;

        // Older worlds should still force player-layers on for compatability.
        // CCK Mirror prefab didn't reflect local player, so user content only worked cause of this!

        m_UseOcclusionCulling = false;
        
        m_ClipPlaneOffset = 0.001f; // exposed in CCK
        m_ReflectLayers &= ~(1 << 5);
        m_ReflectLayers &= ~(1 << 15);
        m_ReflectLayers |= 1 << 8;
        m_ReflectLayers |= 1 << 9;
        m_ReflectLayers |= 1 << 10;
    }

    // This is called when it's known that the object will be rendered by some
    // camera. We render reflections and do other updates here.
    // Because the script executes in edit mode, reflections for the scene view
    // camera will just work!
    public void OnWillRenderObject()
    {
        if (!enabled || !m_MirrorRenderer || !m_MirrorRenderer.sharedMaterial || !m_MirrorRenderer.enabled)
            return;
        
        // Previously was RootLogic.Instance.activeCamera;
        // Camera.current produces correct reflection with *any* camera, be it photo camera, in-world camera, camera on an avatar, or anything else
        // TODO: consider a marker or settings component (i.e. CVRCameraSettings) that would allow excluding mirrors from camera render (useful on both avatars and worlds)
        Camera cam = Camera.current;
        if (!cam)
            return;
        
        // Safeguard from recursive reflections.    
        if (s_InsideRendering) return;
        s_InsideRendering = true;

        if (m_frameCounter > 0)
        {
            m_frameCounter--;
            return;
        }
        m_frameCounter = m_framesNeededToUpdate;
        
        mirrorNormal = Vector3.up;
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = meshFilter != null ? meshFilter.sharedMesh : null;
        if (mesh != null && mesh.normals.Length > 0)
            mirrorNormal = mesh.normals[0];
        
        // Optionally disable pixel lights for reflection
        int oldPixelLightCount = QualitySettings.pixelLightCount;
        if (m_DisablePixelLights)
            QualitySettings.pixelLightCount = 0;

        try
        {
            RenderCamera(cam, m_MirrorRenderer, Camera.StereoscopicEye.Left, ref m_ReflectionTextureLeft);
            m_PropertyBlock.SetTexture(_propertyLeft, m_ReflectionTextureLeft);

            if (!cam.stereoEnabled) return;
            RenderCamera(cam, m_MirrorRenderer, Camera.StereoscopicEye.Right, ref m_ReflectionTextureRight);
            m_PropertyBlock.SetTexture(_propertyRight, m_ReflectionTextureRight);
        }
        finally
        {
            s_InsideRendering = false;
            m_MirrorRenderer.SetPropertyBlock(m_PropertyBlock);
            if (m_DisablePixelLights) // Restore pixel light count
                QualitySettings.pixelLightCount = oldPixelLightCount;
        }
    }

    private void RenderCamera(Camera cam, Renderer rend, Camera.StereoscopicEye eye,
        ref RenderTexture reflectionTexture)
    {
        // find out the reflection plane: position and normal in world space
        Vector3 pos = transform.position;
        Vector3 normal = transform.TransformDirection(mirrorNormal);
        
        CreateMirrorObjects(cam, eye, ref reflectionTexture);

        CopyCameraProperties(cam, m_ReflectionCamera);

        m_ReflectionCamera.useOcclusionCulling = m_UseOcclusionCulling;
        m_ReflectionCamera.depthTextureMode = cam.depthTextureMode | DepthTextureMode.Depth;
        m_ReflectionCamera.stereoTargetEye = StereoTargetEyeMask.None;
        m_ReflectionCamera.cullingMask = m_ReflectLayers.value;

        // Render reflection
        // Reflect camera around reflection plane
        float d = -Vector3.Dot(normal, pos) - m_ClipPlaneOffset;
        Vector4 reflectionPlane = new Vector4(normal.x, normal.y, normal.z, d);

        Matrix4x4 reflection = Matrix4x4.zero;
        CalculateReflectionMatrix(ref reflection, reflectionPlane);

        Matrix4x4 worldToCameraMatrix;
        if (cam.stereoEnabled)
            worldToCameraMatrix = cam.GetStereoViewMatrix(eye) * reflection;
        else
            worldToCameraMatrix = cam.worldToCameraMatrix * reflection;

        m_ReflectionCamera.targetTexture = reflectionTexture;

        Matrix4x4 cameraSideReflection = Matrix4x4.zero;
        CalculateReflectionMatrix(ref cameraSideReflection, new Vector4(1, 0, 0, 0));
        worldToCameraMatrix = cameraSideReflection * worldToCameraMatrix;

        m_ReflectionCamera.worldToCameraMatrix = worldToCameraMatrix;

        // Setup oblique projection matrix so that near plane is our reflection
        // plane. This way we clip everything below/above it for free.
        Vector4 clipPlane = CameraSpacePlane(worldToCameraMatrix, pos, normal, 1.0f);

        m_ReflectionCamera.projectionMatrix = cameraSideReflection *
                                              (cam.stereoEnabled
                                                  ? cam.GetStereoProjectionMatrix(eye)
                                                  : cam.projectionMatrix) * cameraSideReflection.inverse;
        
        m_ReflectionCamera.projectionMatrix = m_ReflectionCamera.CalculateObliqueMatrix(clipPlane);

        m_ReflectionCamera.Render();
    }

    // Cleanup all the objects we possibly have created
    private void CleanupMirrorObjects()
    {
        if (m_ReflectionTextureLeft)
        {
            RenderTexture.ReleaseTemporary(m_ReflectionTextureLeft);
            m_ReflectionTextureLeft = null;
        }
        if (m_ReflectionTextureRight)
        {
            RenderTexture.ReleaseTemporary(m_ReflectionTextureRight);
            m_ReflectionTextureRight = null;
        }
    }

    private void CopyCameraProperties(Camera src, Camera dest)
    {
        if (dest == null)
            return;

        dest.CopyFrom(src);

        if (m_ClearFlags == MirrorClearFlags.Skybox)
        {
            dest.clearFlags = CameraClearFlags.Skybox;
            Skybox mysky = dest.GetComponent<Skybox>();
            if (!mysky || !m_CustomSkybox)
            {
                mysky.enabled = false;
            }
            else
            {
                mysky.enabled = true;
                mysky.material = m_CustomSkybox;
            }
        }
        else if (m_ClearFlags == MirrorClearFlags.Color)
        {
            dest.clearFlags = CameraClearFlags.Color;
            dest.backgroundColor = m_CustomColor;
        }
    }

    private void CreateMirrorObjects(Camera currentCamera, Camera.StereoscopicEye eye,
        ref RenderTexture reflectionTexture)
    {
        // Calculate target resolution
        int currentTextureWidth = Mathf.RoundToInt(Math.Min(usedTextureSize, currentCamera.pixelWidth));
        int currentTextureHeight = Mathf.RoundToInt(Math.Min(usedTextureSize, currentCamera.pixelHeight));

        var targetMsaa = usedMsaa;
        if (targetMsaa == 0)
        {
            RenderTexture targetTexture = currentCamera.targetTexture;
            if (targetTexture != null)
                targetMsaa = targetTexture.antiAliasing;
            else
                targetMsaa = QualitySettings.antiAliasing == 0 ? 1 : QualitySettings.antiAliasing;
        }

        // Unity is good at caching rendertextures, so releasing it here and then immediately re-allocating a texture with the same resolution is fast
        // If the resolution is different, a new texture will be allocated and the old one will be freed
        if (reflectionTexture)
            RenderTexture.ReleaseTemporary(reflectionTexture);

        // Additionally, releasing it here (instead of after mirror rendering is done) allows it to survive turning away from the mirror,
        // so that turning away from and back towards  a mirror does not lead to lag spikes

        // Reflection render texture
        reflectionTexture = RenderTexture.GetTemporary(currentTextureWidth, currentTextureHeight, 24,
            RenderTextureFormat.ARGBHalf,
            RenderTextureReadWrite.Default, targetMsaa, RenderTextureMemoryless.None, VRTextureUsage.None);
        reflectionTexture.name = "__MirrorReflection" + eye.ToString() + GetInstanceID();

        // Camera for reflection
        if (m_ReflectionCamera == null)
        {
            GameObject go = new GameObject("Mirror Reflection Camera id" + GetInstanceID(),
                typeof(Camera), typeof(Skybox), typeof(FlareLayer));
            // Parent it to the mirror for easy cleanup in case of destroyed mirror
            go.transform.SetParent(transform);
            m_ReflectionCamera = go.GetComponent<Camera>();
            // Reflection camera transform is irrelevant because it has matrices set explicitly
            m_ReflectionCamera.enabled = false;
            go.hideFlags = HideFlags.DontSave | HideFlags.HideInHierarchy;
        }
    }

    // Given position/normal of the plane, calculates plane in camera space.
    private Vector4 CameraSpacePlane(Matrix4x4 worldToCameraMatrix, Vector3 pos, Vector3 normal, float sideSign)
    {
        Vector3 offsetPos = pos + normal * m_ClipPlaneOffset;
        Vector3 cpos = worldToCameraMatrix.MultiplyPoint(offsetPos);
        Vector3 cnormal = worldToCameraMatrix.MultiplyVector(normal).normalized * sideSign;
        return new Vector4(cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos, cnormal));
    }

    // Calculates reflection matrix around the given plane
    private static void CalculateReflectionMatrix(ref Matrix4x4 reflectionMat, Vector4 plane)
    {
        reflectionMat.m00 = (1F - 2F * plane[0] * plane[0]);
        reflectionMat.m01 = (-2F * plane[0] * plane[1]);
        reflectionMat.m02 = (-2F * plane[0] * plane[2]);
        reflectionMat.m03 = (-2F * plane[3] * plane[0]);

        reflectionMat.m10 = (-2F * plane[1] * plane[0]);
        reflectionMat.m11 = (1F - 2F * plane[1] * plane[1]);
        reflectionMat.m12 = (-2F * plane[1] * plane[2]);
        reflectionMat.m13 = (-2F * plane[3] * plane[1]);

        reflectionMat.m20 = (-2F * plane[2] * plane[0]);
        reflectionMat.m21 = (-2F * plane[2] * plane[1]);
        reflectionMat.m22 = (1F - 2F * plane[2] * plane[2]);
        reflectionMat.m23 = (-2F * plane[3] * plane[2]);

        reflectionMat.m30 = 0F;
        reflectionMat.m31 = 0F;
        reflectionMat.m32 = 0F;
        reflectionMat.m33 = 1F;
    }
}