using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ABI.CCK.Scripts.Runtime
{
    [AddComponentMenu("")]
    public class CCK_TamperCheck : MonoBehaviour
    {
        private bool _hasErrorOccured;
        
        public void DummyTargetMethod()
        {
            // ignored
        }

        private void FixedUpdate()
        {
            if (_hasErrorOccured)
                return;
            
            Button button = GetComponent<Button>();
            if (button == null)
                return;

            UnityEvent onClickEvent = button.onClick;
            if (onClickEvent == null || onClickEvent.GetPersistentEventCount() != 0)
                return;

            _hasErrorOccured = true;
            
            Debug.LogError(
                "[CCK:TamperCheck] No OnClick events detected on dummy button. A third-party script may have removed them on entering playmode!");
#if UNITY_EDITOR
            Debug.LogError("[CCK:TamperCheck] Exiting playmode & canceling upload. The CCK is unusable in this state.");
            if (UnityEditor.EditorUtility.DisplayDialog("CCK :: TamperCheck",
                    "An error has occured which has rendered the CCK unusable. Please check console for errors.", "Okay"))
                UnityEditor.EditorApplication.ExitPlaymode();
#endif
        }
    }
}