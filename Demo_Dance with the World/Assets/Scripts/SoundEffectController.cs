using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectController : MonoBehaviour {
    public AudioClip getN;
    public AudioClip getS;
    public AudioClip attract;
    public AudioClip repulse;
    public AudioClip giveMag;
    public AudioClip resetMag;

    public void PlayGetN() {
        AudioSource.PlayClipAtPoint(getN, Camera.main!.transform.position);
    }

    public void PlayGetS() {
        AudioSource.PlayClipAtPoint(getS, Camera.main!.transform.position);
    }

    public void PlayAttract() {
        AudioSource.PlayClipAtPoint(attract, Camera.main!.transform.position);
    }

    public void PlayRepulse() {
        AudioSource.PlayClipAtPoint(repulse, Camera.main!.transform.position);
    }

    public void PlayGiveMag() {
        AudioSource.PlayClipAtPoint(giveMag, Camera.main!.transform.position);
    }

    public void PlayResetMag() {
        AudioSource.PlayClipAtPoint(resetMag, Camera.main!.transform.position);
    }
}