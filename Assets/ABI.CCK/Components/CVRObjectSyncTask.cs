﻿using UnityEngine;

namespace ABI.CCK.Components
{
    [System.Serializable]
    public class CVRObjectSyncTask
    {
        public enum TaskType
        {
            Position = 1,
            Rotation = 2,
            ActivityState = 3,
            PickupOwner = 4,
            AnimatorParameter = 5,
            AnimatorAnimationProgress = 6,
            VariableBufferValue = 7,
        }

        public TaskType type = TaskType.Position;

        public Component component;

        public int intVal;

        public CVRSerializableObjectSyncTask getDefaultValues()
        {
            if (component == null) return null;

            switch (type)
            {
                case TaskType.Position:
                    var retPos = new CVRSerializableObjectSyncTask();
                    retPos.type = TaskType.Position;
                    retPos.value = ((Transform) component).position.ToString("F6");
                    retPos.intVal = intVal;
                    return retPos;
                case TaskType.Rotation:
                    var retRot = new CVRSerializableObjectSyncTask();
                    retRot.type = TaskType.Rotation;
                    retRot.value = ((Transform) component).eulerAngles.ToString("F3");
                    retRot.intVal = intVal;
                    return retRot;
                case TaskType.ActivityState:
                    var retAct = new CVRSerializableObjectSyncTask();
                    retAct.type = TaskType.ActivityState;
                    retAct.value = ((Transform) component).gameObject.activeSelf ? "1" : "0";
                    retAct.intVal = intVal;
                    return retAct;
                case TaskType.PickupOwner:
                    var retPick = new CVRSerializableObjectSyncTask();
                    retPick.type = TaskType.PickupOwner;
                    retPick.value = "";
                    retPick.intVal = intVal;
                    return retPick;
                case TaskType.AnimatorParameter:
                    var retAniParam = new CVRSerializableObjectSyncTask();
                    retAniParam.type = TaskType.AnimatorParameter;
                    var animator = (Animator) component;
                    if (animator == null) return null;
                    if (animator.runtimeAnimatorController == null) return null;
                    if (animator.parameters.Length <= intVal) return null; 
                    switch (animator.parameters[intVal].type)
                    {
                        case AnimatorControllerParameterType.Bool:
                            retAniParam.value = animator.parameters[intVal].defaultBool ? "1" : "0";
                            break;
                        case AnimatorControllerParameterType.Float:
                            retAniParam.value = animator.parameters[intVal].defaultFloat.ToString("F8");
                            break;
                        case AnimatorControllerParameterType.Int:
                            retAniParam.value = animator.parameters[intVal].defaultInt.ToString();
                            break; 
                        case AnimatorControllerParameterType.Trigger:
                            retAniParam.value = "0";
                            break;
                    }
                    retAniParam.intVal = intVal;
                    return retAniParam;
                case TaskType.AnimatorAnimationProgress:
                    var retAniProg = new CVRSerializableObjectSyncTask();
                    retAniProg.type = TaskType.AnimatorAnimationProgress;
                    retAniProg.value = (0f).ToString("F8");
                    retAniProg.intVal = intVal;
                    return retAniProg;
                case TaskType.VariableBufferValue:
                    var retVar = new CVRSerializableObjectSyncTask();
                    retVar.type = TaskType.VariableBufferValue;
                    retVar.value = ((CVRVariableBuffer) component).defaultValue.ToString("F8");
                    retVar.intVal = intVal;
                    return retVar;
            }
            
            return null;
        }
    }

    [System.Serializable]
    public class CVRSerializableObjectSyncTask
    {
        public CVRObjectSyncTask.TaskType type = CVRObjectSyncTask.TaskType.Position;
        public string value;
        public int intVal;
    }
}