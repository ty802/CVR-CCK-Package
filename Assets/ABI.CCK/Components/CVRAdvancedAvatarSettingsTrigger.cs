using System;
using System.Collections.Generic;
using UnityEngine;

namespace ABI.CCK.Components
{
    [AddComponentMenu("ChilloutVR/CVR Advanced Avatar Trigger")]
    [HelpURL("https://developers.abinteractive.net/cck/components/aas-trigger/")]
    [DisallowMultipleComponent]
    public class CVRAdvancedAvatarSettingsTrigger : MonoBehaviour, ICCK_Component
    {
        public Vector3 areaSize = new Vector3(0.05f, 0.05f, 0.05f);
        public Vector3 areaOffset = Vector3.zero;
        public string settingName;
        public float settingValue;

        public bool useAdvancedTrigger;
        public bool isLocalInteractable = true;
        public bool isNetworkInteractable = true;
        public bool allowParticleInteraction;

        [SerializeField] public List<CVRPointer> allowedPointer = new List<CVRPointer>();
        public string[] allowedTypes = Array.Empty<string>();

        public List<CVRAdvancedAvatarSettingsTriggerTask> enterTasks = new List<CVRAdvancedAvatarSettingsTriggerTask>();
        public List<CVRAdvancedAvatarSettingsTriggerTask> exitTasks = new List<CVRAdvancedAvatarSettingsTriggerTask>();
        public List<CVRAdvancedAvatarSettingsTriggerTaskStay> stayTasks = new List<CVRAdvancedAvatarSettingsTriggerTaskStay>();

        public enum SampleDirection
        {
            XPositive,
            XNegative,
            YPositive,
            YNegative,
            ZPositive,
            ZNegative
        }

        public SampleDirection sampleDirection = SampleDirection.XPositive;

        public void Trigger()
        {
        }

        public void EnterTrigger()
        {
        }

        public void ExitTrigger()
        {
        }

        public void StayTrigger(float percent = 0f)
        {
        }

        private void OnDrawGizmosSelected()
        {
            if (!isActiveAndEnabled) 
                return;
            
            Gizmos.color = Color.cyan;
            Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            Gizmos.matrix = rotationMatrix;

            Collider collider = gameObject.GetComponent<Collider>();
            if (collider == null)
            {
                if (OnlyHasDistanceTask())
                {
                    Gizmos.DrawSphere(Vector3.zero, areaSize.x);
                    Gizmos.DrawWireSphere(Vector3.zero, areaSize.x);
                }
                else
                {
                    Gizmos.DrawCube(areaOffset, areaSize);
                }
            }

            Vector3 bounds = new Vector3(areaSize.x * 0.5f, areaSize.y * 0.5f, areaSize.z * 0.5f);

            if (stayTasks.Count > 0 && !OnlyHasDistanceTask())
            {
                Gizmos.DrawWireCube(areaOffset, areaSize);

                switch (sampleDirection)
                {
                    case SampleDirection.XPositive:
                        Gizmos.DrawLine(
                            new Vector3(-bounds.x, bounds.y, bounds.z) + areaOffset,
                            new Vector3(bounds.x, 0f, bounds.z) + areaOffset
                        );
                        Gizmos.DrawLine(
                            new Vector3(-bounds.x, -bounds.y, bounds.z) + areaOffset,
                            new Vector3(bounds.x, 0f, bounds.z) + areaOffset
                        );
                        Gizmos.DrawLine(
                            new Vector3(-bounds.x, bounds.y, bounds.z) + areaOffset,
                            new Vector3(bounds.x, bounds.y, 0f) + areaOffset
                        );
                        Gizmos.DrawLine(
                            new Vector3(-bounds.x, bounds.y, -bounds.z) + areaOffset,
                            new Vector3(bounds.x, bounds.y, 0f) + areaOffset
                        );
                        break;
                    case SampleDirection.XNegative:
                        Gizmos.DrawLine(
                            new Vector3(bounds.x, bounds.y, bounds.z) + areaOffset,
                            new Vector3(-bounds.x, 0f, bounds.z) + areaOffset
                        );
                        Gizmos.DrawLine(
                            new Vector3(bounds.x, -bounds.y, bounds.z) + areaOffset,
                            new Vector3(-bounds.x, 0f, bounds.z) + areaOffset
                        );
                        Gizmos.DrawLine(
                            new Vector3(bounds.x, bounds.y, bounds.z) + areaOffset,
                            new Vector3(-bounds.x, bounds.y, 0f) + areaOffset
                        );
                        Gizmos.DrawLine(
                            new Vector3(bounds.x, bounds.y, -bounds.z) + areaOffset,
                            new Vector3(-bounds.x, bounds.y, 0f) + areaOffset
                        );
                        break;
                    case SampleDirection.YPositive:
                        Gizmos.DrawLine(
                            new Vector3(-bounds.x, -bounds.y, bounds.z) + areaOffset,
                            new Vector3(0f, bounds.y, bounds.z) + areaOffset
                        );
                        Gizmos.DrawLine(
                            new Vector3(bounds.x, -bounds.y, bounds.z) + areaOffset,
                            new Vector3(0f, bounds.y, bounds.z) + areaOffset
                        );
                        Gizmos.DrawLine(
                            new Vector3(-bounds.x, -bounds.y, -bounds.z) + areaOffset,
                            new Vector3(-bounds.x, bounds.y, 0f) + areaOffset
                        );
                        Gizmos.DrawLine(
                            new Vector3(-bounds.x, -bounds.y, bounds.z) + areaOffset,
                            new Vector3(-bounds.x, bounds.y, 0f) + areaOffset
                        );
                        break;
                    case SampleDirection.YNegative:
                        Gizmos.DrawLine(
                            new Vector3(-bounds.x, bounds.y, bounds.z) + areaOffset,
                            new Vector3(0f, -bounds.y, bounds.z) + areaOffset
                        );
                        Gizmos.DrawLine(
                            new Vector3(bounds.x, bounds.y, bounds.z) + areaOffset,
                            new Vector3(0f, -bounds.y, bounds.z) + areaOffset
                        );
                        Gizmos.DrawLine(
                            new Vector3(-bounds.x, bounds.y, -bounds.z) + areaOffset,
                            new Vector3(-bounds.x, -bounds.y, 0f) + areaOffset
                        );
                        Gizmos.DrawLine(
                            new Vector3(-bounds.x, bounds.y, bounds.z) + areaOffset,
                            new Vector3(-bounds.x, -bounds.y, 0f) + areaOffset
                        );
                        break;
                    case SampleDirection.ZPositive:
                        Gizmos.DrawLine(
                            new Vector3(-bounds.x, bounds.y, -bounds.z) + areaOffset,
                            new Vector3(0f, bounds.y, bounds.z) + areaOffset
                        );
                        Gizmos.DrawLine(
                            new Vector3(bounds.x, bounds.y, -bounds.z) + areaOffset,
                            new Vector3(0f, bounds.y, bounds.z) + areaOffset
                        );
                        Gizmos.DrawLine(
                            new Vector3(-bounds.x, bounds.y, -bounds.z) + areaOffset,
                            new Vector3(-bounds.x, 0f, bounds.z) + areaOffset
                        );
                        Gizmos.DrawLine(
                            new Vector3(-bounds.x, -bounds.y, -bounds.z) + areaOffset,
                            new Vector3(-bounds.x, 0f, bounds.z) + areaOffset
                        );
                        break;
                    case SampleDirection.ZNegative:
                        Gizmos.DrawLine(
                            new Vector3(-bounds.x, bounds.y, bounds.z) + areaOffset,
                            new Vector3(0f, bounds.y, -bounds.z) + areaOffset
                        );
                        Gizmos.DrawLine(
                            new Vector3(bounds.x, bounds.y, bounds.z) + areaOffset,
                            new Vector3(0f, bounds.y, -bounds.z) + areaOffset
                        );
                        Gizmos.DrawLine(
                            new Vector3(-bounds.x, bounds.y, bounds.z) + areaOffset,
                            new Vector3(-bounds.x, 0f, -bounds.z) + areaOffset
                        );
                        Gizmos.DrawLine(
                            new Vector3(-bounds.x, -bounds.y, bounds.z) + areaOffset,
                            new Vector3(-bounds.x, 0f, -bounds.z) + areaOffset
                        );
                        break;
                }
            }
        }

        private bool OnlyHasDistanceTask()
        {
            return enterTasks.Count == 0 && exitTasks.Count == 0 && stayTasks.Count > 0 && stayTasks.FindAll(x =>
                x.updateMethod != CVRAdvancedAvatarSettingsTriggerTaskStay.UpdateMethod.SetFromDistance).Count == 0;
        }
    }

    [Serializable]
    public class CVRAdvancedAvatarSettingsTriggerTask
    {
        public string settingName;
        public float settingValue = 0f;
        public float delay = 0f;
        public float holdTime = 0f;

        public enum UpdateMethod
        {
            Override = 1,
            Add = 2,
            Subtract = 3,
            Toggle = 4
        }

        public UpdateMethod updateMethod = UpdateMethod.Override;
    }

    [Serializable]
    public class CVRAdvancedAvatarSettingsTriggerTaskStay
    {
        public string settingName;
        public float minValue = 0f;
        public float maxValue = 1f;

        public enum UpdateMethod
        {
            SetFromPosition = 1,
            Add = 2,
            Subtract = 3,
            SetFromDistance = 4
        }

        public UpdateMethod updateMethod = UpdateMethod.SetFromPosition;
    }
}