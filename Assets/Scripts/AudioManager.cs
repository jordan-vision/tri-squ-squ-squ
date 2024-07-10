using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private List<AudioSource> m_allAudioSources = new();
    [SerializeField] private AudioClipProperties[] m_allMusicTracks;

    private void Awake()
    {
        foreach (AudioClipProperties clip in m_allMusicTracks)
        {
            AudioSource source = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;

            source.clip = clip.Clip;
            source.volume = clip.Volume;
            source.loop = clip.Loops;

            m_allAudioSources.Add(source);
            clip.Source = source;
        }
    }

    public void Fade(int trackNumber, float newVolume) {
        StartCoroutine(Fade(m_allMusicTracks[trackNumber].Source, newVolume));
    }

    public IEnumerator Fade(AudioSource source, float newVolume)
    {
        while (Mathf.Abs(source.volume - newVolume) > 0.05f)
        {
            if (source.volume < newVolume)
                source.volume += 0.05f;

            else
                source.volume -= 0.05f;

            yield return new WaitForSeconds(0.1f);
        }

        source.volume = newVolume;
    }

    public void PlayAllMusic()
    {
        foreach (AudioSource source in m_allAudioSources)
        {
            source.Play();
        }
    }

    public void RestructureTracks(int phase)
    {
        foreach (AudioClipProperties clip in m_allMusicTracks)
        {
            float newVolume = clip.ActivePhases.Contains(phase) ? 1.0f : 0.0f;
            StartCoroutine(Fade(clip.Source, newVolume));
        }
    }

    public void SetVolume(int trackNumber, float newVolume)
    {
        m_allMusicTracks[trackNumber].Source.volume = newVolume;
    }

    public void StopAllMusic()
    {
        foreach (AudioSource source in m_allAudioSources)
        {
            source.Stop();
        }
    }
}

[Serializable]
public class AudioClipProperties
{
    [SerializeField] List<int> m_activePhases = new();
    [SerializeField] AudioClip m_clip;
    [SerializeField] float m_initialVolume;
    [SerializeField] bool m_loops = true;
    AudioSource m_source;

    public List<int> ActivePhases { get { return m_activePhases; } }
    public AudioClip Clip { get { return m_clip; } }
    public float Volume { get { return m_initialVolume; } }
    public bool Loops { get { return m_loops; } }
    public AudioSource Source { get { return m_source; } set { m_source = value; } }
}
