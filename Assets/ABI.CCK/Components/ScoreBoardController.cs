using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ABI.CCK.Components
{
    [AddComponentMenu("ChilloutVR/Score Board Controller")]
    [HelpURL("https://developers.abinteractive.net/cck/")]
    public class ScoreBoardController : MonoBehaviour, ICCK_Component
    {
        public GameInstanceController gameInstanceController;

        public List<Text> roundTimers = new List<Text>();
        public List<Text> roundStatus = new List<Text>();

        public List<ScoreBoardDisplayElementsTeam> teamElements = new List<ScoreBoardDisplayElementsTeam>();
        
        private void Start()
        {
            
        }
    }

    [System.Serializable]
    public class ScoreBoardDisplayElementsTeam
    {
        public List<Text> playerLists = new List<Text>();
        public List<Text> teamScore = new List<Text>();
    }
}