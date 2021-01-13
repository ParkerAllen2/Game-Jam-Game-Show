using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundController : MonoBehaviour
{
    public static SoundController sc;
    [SerializeField] Slider slider;

    public AudioSource battleMusic, menuMusic;
    bool battleOn;

    private void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
        if(sc == null)
        {
            UpdateVolumes();
            sc = GetComponent<SoundController>();
            StartCoroutine(LerpBetweenMusic(battleMusic, menuMusic));
        }
    }

    public void PlayBattleMusic()
    {
        if (!battleOn)
        {
            battleOn = true;
            StartCoroutine(LerpBetweenMusic(menuMusic, battleMusic));
        }
    }

    public void PlayMenuMusic()
    {
        if(battleOn)
        {
            battleOn = false;
            StartCoroutine(LerpBetweenMusic(battleMusic, menuMusic));
        }
    }

    IEnumerator LerpBetweenMusic(AudioSource from, AudioSource to)
    {
        to.Play();
        float step = 1f / 1000;
        for(float i = 0; i < .1f; i += step)
        {
            from.volume = .1f - i;
            to.volume = i;
            yield return new WaitForSeconds(.05f);
        }

        from.Stop();
        from.volume = 0;
        to.volume = .1f;
    }


    public void UpdateVolumes()
    {
        AudioListener.volume = slider.value;
    }
}
