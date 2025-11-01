using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioController : MonoBehaviour {
    private new AudioSource audio;
    public AudioClip source;
    private float curTime = 0.8f;

    void Start() {
        audio = GetComponent<AudioSource>();
        audio.clip = source;
        audio.Play();
    }

    void Update() {
        float[] spectrumData = new float[128];
        audio.GetSpectrumData(spectrumData, 0, FFTWindow.Rectangular);
        curTime += Time.deltaTime;
        if (spectrumData[0] >= 0.48f && curTime >= 0.4f) {
            Messager.Send(new SparkMessage());
            curTime = 0;
        }

        // Debug用
        // Messager.Send(new VolumnChangedMessage(spectrumData));
    }
}