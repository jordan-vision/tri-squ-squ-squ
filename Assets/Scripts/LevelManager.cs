using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] bool m_wavesActive = false;
    [SerializeField] private AudioManager m_audioManager;
    [SerializeField] private BeatGuide m_beatGuide;
    [SerializeField] private TextMeshProUGUI m_bigText;
    private List<GameObject> m_currentWaveEnemyInstances = new(), m_currentWaveEnemyPrefabs;
    [SerializeField] private Ending m_ending;
    private int m_i = 0, m_j = 0;
    private int m_musicPhase = 0;
    [SerializeField] private Player m_player;
    [SerializeField] private RhythmTimer m_rhythmTimer;
    [SerializeField] private WaveList m_waves;

    public static LevelManager Instance { get; private set; }
    public AudioManager AudioManager { get { return m_audioManager; } }
    public BeatGuide BeatGuide { get { return m_beatGuide; } }

    public bool IsStarted { get; private set; }

    public Player Player { get { return m_player; } }
    public RhythmTimer RhythmTimer { get { return m_rhythmTimer; } }

    private void Awake()
    {
        Instance = this;
        m_bigText.text = "HIT\n<color=#DB5A8C>SPACE</color>";
    }

    void Update()
    {
        if (Controls.GetShootPress() && !IsStarted)
            StartCoroutine(StartLevel());

        if (m_wavesActive && IsStarted && m_currentWaveEnemyInstances.Count > 0 && AreAllEnemiesDefeated())
            StartCoroutine(EndSubWave());
    }

    public IEnumerator KillPlayer()
    {
        m_player.GetComponent<SpriteRenderer>().enabled = false;
        m_audioManager.StopAllMusic();
        yield return new WaitForSeconds(1);
        GameManager.Instance.RestartGame();
    }

    private void EndLevel()
    {
        m_ending.gameObject.SetActive(true);
        StartCoroutine(m_ending.StartEnding());
    }

    private bool AreAllEnemiesDefeated()
    {
        foreach (GameObject enemy in m_currentWaveEnemyInstances)
        {
            if (enemy != null)
                return false;
        }
        return true;
    }

    private IEnumerator EndSubWave()
    {
        m_j++;
        m_musicPhase++;
        AudioManager.RestructureTracks(m_musicPhase);

        m_currentWaveEnemyInstances = new();
        if (m_j >= m_waves.Waves[m_i].SubWaves.SubWaves.Count)
        {
            StartCoroutine(EndWave());
            yield return null;
        } else
        {
            yield return new WaitForSeconds(2);
            StartSubWave();
        }
    }

    private IEnumerator EndWave()
    {
        m_i++;
        m_j = 0;

        if (m_i >= m_waves.Waves.Count)
        {
            EndLevel();
            yield return null;
        }
        else
        {
            yield return new WaitForSeconds(4);
            StartWave();
        }
    }

    private IEnumerator ShowText(string message)
    {
        m_bigText.text = message;
        yield return new WaitForSeconds(2.0f);
        m_bigText.text = "";
    }

    private IEnumerator StartLevel()
    {
        AudioManager.PlayAllMusic();
        yield return new WaitForSeconds(Settings.AudioLatency);
        RhythmTimer.StartTimer();
        if (m_wavesActive) StartWave();
        BeatGuide.gameObject.SetActive(true);
        StartCoroutine(BeatGuide.Show(16.0f));
        m_bigText.text = "";
        IsStarted = true;
    }

    private void StartSubWave()
    {
        m_currentWaveEnemyPrefabs = m_waves.Waves[m_i].SubWaves.SubWaves[m_j].Enemies;
        foreach (GameObject enemy in m_currentWaveEnemyPrefabs)
        {
            m_currentWaveEnemyInstances.Add(Instantiate(enemy));
        }
    }

    private void StartWave()
    {
        if (m_musicPhase == 6)
        {
            m_musicPhase++;
            AudioManager.RestructureTracks(m_musicPhase);
        }

        if (m_i == 2)
            StartCoroutine(ShowText("BOSS: <color=#DB5A8C>□²</color>"));
        else
            StartCoroutine(ShowText("WAVE " + (m_i + 1)));
        StartSubWave();
    }
}

[Serializable]
public class Wave {

    [SerializeField] public SubWaveList SubWaves;
}

[Serializable]
public class SubWave
{
    [SerializeField] public List<GameObject> Enemies;
}

[Serializable]
public class WaveList
{
    [SerializeField] public List<Wave> Waves;
}

[Serializable]
public class SubWaveList
{
    [SerializeField] public List<SubWave> SubWaves;
}
