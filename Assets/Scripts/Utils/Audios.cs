using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;
using MyExtensions;
using UnityEngine.Audio;


namespace AudioController
{
    public class Audios : SingletonDontDestroy<Audios>
    {
        [Header("AUDIO SOURCE")]
        [SerializeField] private AudioSource ambienceAudioSource;
        [SerializeField] private AudioSource effectsAudioSource;

        [Header("AUDIO MIXER")]
        [field: SerializeField] public AudioMixer audioMixer { get; private set; }

        [Header("CLIPS")]
        [SerializeField] private AudioClip clickButton;



        public readonly string masterVolumeParam = "masterVolume";
        public readonly string ambientVolumeParam = "ambientVolume";
        public readonly string effectsVolumeParam = "effectsVolume";


        private CancellationTokenSource ctx;

        void OnDestroy()
        {
            ctx?.Cancel();
        }

        public void PlayClickButton()
        {
            if (clickButton == null) return;
            effectsAudioSource.PlayOneShot(clickButton);
        }

        public void PlayEffect(AudioClip effect)
        {
            if (effect == null) return;
            effectsAudioSource.PlayOneShot(effect);
        }






        public async UniTask BlendOff(float duration = 1f)
        {
            ctx?.Cancel();
            ctx = new CancellationTokenSource();
            try
            {
                await audioMixer.LerpAudio(masterVolumeParam, -80f, duration, ctx.Token);   //80 es el minimo
            }
            catch (OperationCanceledException)
            {
                // La operación fue cancelada, no hacemos nada
                return;
            }
        }


        public async UniTask BlendOn(float duration, CancellationToken token)
        {
            ctx?.Cancel();
            ctx = new CancellationTokenSource();
            try
            {
                await audioMixer.LerpAudio(masterVolumeParam, 0f, duration, ctx.Token);   //0 es el maximo
            }
            catch (OperationCanceledException)
            {
                // La operación fue cancelada, no hacemos nada
                return;
            }
        }




    }
}
