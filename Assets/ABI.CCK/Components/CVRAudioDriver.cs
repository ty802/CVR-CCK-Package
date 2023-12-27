using System.Collections.Generic;
using UnityEngine;

namespace ABI.CCK.Components
{
    [HelpURL("https://developers.abinteractive.net/cck/components/audio-driver/")]
    public class CVRAudioDriver : MonoBehaviour, ICCK_Component
    {
        public AudioSource audioSource;
        
        [SerializeField] 
        public List<AudioClip> audioClips = new List<AudioClip>();
        public int selectedAudioClip;
        public bool playOnSwitch = true;

        private int _selectedAudioClip;

        #region Unity Events

        // Editor testing
        private void OnValidate() =>  CheckAndUpdateAudioClip();
        private void OnDidApplyAnimationProperties() => CheckAndUpdateAudioClip();

        #endregion

        #region Public Methods

        public void PlaySound(int index)
        {
            SetAndMaybePlay(index);
        }

        public void PlaySound()
        {
            if (audioSource != null && audioSource.enabled) 
                audioSource.Play();
        }

        public void PlayNext()
        {
            if (_selectedAudioClip + 1 >= audioClips.Count)
                PlaySound(0);
            else
                PlaySound(_selectedAudioClip + 1);
        }

        public void PlayPrev()
        {
            if (_selectedAudioClip == 0)
                PlaySound(audioClips.Count - 1);
            else
                PlaySound(_selectedAudioClip - 1);
        }

        public void SelectRandomSound()
        {
            SetAndMaybePlay(Random.Range(0, audioClips.Count));
        }

        #endregion

        #region Private Methods

        private void CheckAndUpdateAudioClip()
        {
            if (selectedAudioClip != _selectedAudioClip)
                SetAndMaybePlay(selectedAudioClip);
        }

        private void SetAndMaybePlay(int index)
        {
            if (SetAudioClip(index) && playOnSwitch) 
                PlaySound();
        }
        
        private bool SetAudioClip(int index)
        {
            if (index >= audioClips.Count) 
                return false;

            if (audioClips[index] == null || audioSource == null) 
                return false;
            
            audioSource.clip = audioClips[index];
            _selectedAudioClip = selectedAudioClip;
            return true;
        }

        #endregion
    }
}