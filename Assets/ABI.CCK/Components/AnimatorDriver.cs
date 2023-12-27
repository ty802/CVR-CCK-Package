using System.Collections.Generic;
using UnityEngine;

namespace ABI.CCK.Components
{
    [AddComponentMenu("ChilloutVR/Animator Driver")]
    [HelpURL("https://developers.abinteractive.net/cck/")]
    public class AnimatorDriver : StateMachineBehaviour, ICCK_Component
    {
        public List<AnimatorDriverTask> EnterTasks = new List<AnimatorDriverTask>();
        public List<AnimatorDriverTask> ExitTasks = new List<AnimatorDriverTask>();
        
        public bool localOnly = false;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            foreach (var task in EnterTasks)
            {
                task.Execute(animator);
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            foreach (var task in ExitTasks)
            {
                task.Execute(animator);
            }
        }
    }

    [System.Serializable]
    public class AnimatorDriverTask
    {
        public enum Operator
        {
            Set,
            Addition,
            Subtraction,
            Multiplication,
            Division,
            Modulo,
            Power,
            Log,
            Equal,
            NotEqual,
            LessThen,
            LessEqual,
            MoreThen,
            MoreEqual
        }

        public enum SourceType
        {
            Static,
            Parameter,
            Random
        }

        public enum ParameterType
        {
            None,
            Float,
            Int,
            Bool,
            Trigger
        }

        public ParameterType targetType = ParameterType.None;
        public string targetName = "";

        public Operator op = Operator.Set;

        public SourceType aType = SourceType.Static;
        public float aValue = 0f;
        public float aMax = 1f;
        public ParameterType aParamType;
        public string aName = "";
        
        public SourceType bType = SourceType.Static;
        public float bValue = 0f;
        public float bMax = 1f;
        public ParameterType bParamType;
        public string bName = "";

        public void Execute(Animator animator)
        {
            float valA = 0f;
            float valB = 0f;
            float res = 0f;

            switch (aType)
            {
                case SourceType.Static:
                    valA = aValue;
                    break;
                case SourceType.Random:
                    valA = Random.Range(aValue, aMax);
                    break;
                case SourceType.Parameter:
                    switch (aParamType)
                    {
                        default:
                            valA = 0f;
                            break;
                        case ParameterType.Bool:
                            valA = animator.GetBool(aName) ? 1f : 0f;
                            break;
                        case ParameterType.Float:
                            valA = animator.GetFloat(aName);
                            break;
                        case ParameterType.Int:
                            valA = animator.GetInteger(aName);
                            break;
                    }
                    break;
            }

            if (op == Operator.Set)
            {
                res = valA;
            }
            else
            {
                switch (bType)
                {
                    case SourceType.Static:
                        valB = bValue;
                        break;
                    case SourceType.Random:
                        valB = Random.Range(bValue, aMax);
                        break;
                    case SourceType.Parameter:
                        switch (bParamType)
                        {
                            default:
                                valB = 0f;
                                break;
                            case ParameterType.Bool:
                                valB = animator.GetBool(bName) ? 1f : 0f;
                                break;
                            case ParameterType.Float:
                                valB = animator.GetFloat(bName);
                                break;
                            case ParameterType.Int:
                                valB = animator.GetInteger(bName);
                                break;
                        }
                        break;
                }

                switch (op)
                {
                    case Operator.Addition:
                        res = valA + valB;
                        break;
                    case Operator.Subtraction:
                        res = valA - valB;
                        break;
                    case Operator.Multiplication:
                        res = valA * valB;
                        break;
                    case Operator.Division:
                        res = valA / valB;
                        break;
                    case Operator.Modulo:
                        res = valA % valB;
                        break;
                    case Operator.Power:
                        res = Mathf.Pow(valA, valB);
                        break;
                    case Operator.Log:
                        res = Mathf.Log(valA, valB);
                        break;
                    case Operator.Equal:
                        res = valA == valB ? 1f : 0f;
                        break;
                    case Operator.NotEqual:
                        res = valA != valB ? 1f : 0f;
                        break;
                    case Operator.LessThen:
                        res = valA < valB ? 1f : 0f;
                        break;
                    case Operator.LessEqual:
                        res = valA <= valB ? 1f : 0f;
                        break;
                    case Operator.MoreThen:
                        res = valA > valB ? 1f : 0f;
                        break;
                    case Operator.MoreEqual:
                        res = valA >= valB ? 1f : 0f;
                        break;
                }
            }

            switch (targetType)
            {
                case ParameterType.Bool:
                    animator.SetBool(targetName, res >= 0.5f);
                    break;
                case ParameterType.Trigger:
                    if (res >= 0.5f) animator.SetTrigger(targetName);
                    break;
                case ParameterType.Float:
                    animator.SetFloat(targetName, res);
                    break;
                case ParameterType.Int:
                    animator.SetInteger(targetName, (int) res);
                    break;
            }
        }
    }
}