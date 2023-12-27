using UnityEngine;
using UnityEngine.AI;

namespace ABI.CCK.Components
{
    [AddComponentMenu("ChilloutVR/CVR Nav Controller")]
    [HelpURL("https://developers.abinteractive.net/cck/components/nav-controller/")]
    public class CVRNavController : MonoBehaviour, ICCK_Component
    {
        public NavMeshAgent navMeshAgent;
        public Transform[] navTargetList;
        public int navTargetIndex = 0;
        public Transform[] patrolPoints;
        public int patrolPointIndex = 0;
        public float patrolPointCheckDistance = 0.5f;
        public bool patrolEnabled = false;
    }
}