using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Calibrator : MonoBehaviour
{
    private bool m_audioCalibrationReady, m_audioCalibrationStarted, m_inputCalibrationStarted, m_inputCalibrationDone;
    [SerializeField] private AudioManager m_audioManager;
    private float m_audioOffset = 0.1f, m_inputOffset;
    private float m_averageInputError;
    [SerializeField] private GameObject[] m_beats;
    private int m_index = 0;
    bool indicating = false;
    private float[] m_inputErrors = new float[8];
    [SerializeField] private TextMeshProUGUI m_instructions;
    [SerializeField] private TextMeshProUGUI m_offset;
    [SerializeField] private RhythmTimer m_rhythmTimer;

    public static Calibrator Instance { get; private set; }

    private void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (Controls.GetShootPress() && !m_audioCalibrationStarted)
            StartCoroutine(StartAudioCalibration());

        if (!m_audioCalibrationStarted || m_inputCalibrationDone)
            return;

        if (m_audioCalibrationReady && m_rhythmTimer.IsBeat() && !indicating)
            StartCoroutine(Indicate(m_beats[(int)m_rhythmTimer.TimerValue]));

        if (m_audioCalibrationReady && !m_inputCalibrationStarted)
            CalibrateAudio();

        if (m_inputCalibrationStarted)
            CalibrateInput();
    }

    private void CalibrateAudio()
    {
        if (Controls.GetLeftPress())
            m_audioOffset -= 0.01f;

        else if (Controls.GetRightPress())
            m_audioOffset += 0.01f;

        else if (Controls.GetShootPress())
        {
            StartInputCalibration();
            return;
        }

        else
            return;

        m_audioManager.StopAllMusic();
        m_rhythmTimer.StopTimer();
        StartCoroutine(StartAudioCalibration());
    }

    private void CalibrateInput()
    {
        if (Controls.GetShootPress())
        {
            float thisOffset = m_rhythmTimer.GetOffsetFromClosestTimestamp();
            m_inputErrors[m_index] = thisOffset;
            m_index++;
            m_offset.text = thisOffset.ToString("+0.00;-0.00");
            m_instructions.text = "Calibrate input and audio. Hit <color=#DB5A8C>SPACE</color> in time with the metronome.\n" +
                $"Do this {8 - m_index} more time(s).";

            m_audioManager.Fade(1, (m_index + 1) / 9.0f);

            if (m_index >= 8)
            {
                float total = 0;

                for (int i = 1; i < 8; i++)
                    total += m_inputErrors[i];

                m_averageInputError = Mathf.Round(100.0f * total / 7.0f) / 100.0f;
                StartCoroutine(CalibrationComplete());
            }
        }
    }

    private IEnumerator CalibrationComplete()
    {
        m_inputCalibrationDone = true;
        SetPlayerSettings();
        m_instructions.text = "CALIBRATION COMPLETE";

        foreach (GameObject beat in m_beats)
            beat.SetActive(false);

        m_offset.gameObject.SetActive(false);

        m_audioManager.Fade(0, 0);
        m_audioManager.Fade(1, 1);
        yield return new WaitForSeconds(2.0f);
        m_audioManager.Fade(1, 0);
        yield return new WaitForSeconds(2.0f);

        GameManager.Instance.Play();
    }

    private IEnumerator Indicate(GameObject beat)
    {
        indicating = true;
        beat.GetComponent<Image>().color = new Color(1.0f, 192.0f / 255.0f, 192.0f / 255.0f);
        yield return new WaitForSeconds(0.25f);
        beat.GetComponent<Image>().color = new Color(219.0f / 255.0f, 90.0f / 255.0f, 140.0f / 255.0f);
        indicating = false;
    }

    private void SetPlayerSettings()
    {
        Settings.AudioLatency = m_audioOffset;
        Settings.InputLatency = m_averageInputError;
    }

    private IEnumerator StartAudioCalibration()
    {
        m_audioCalibrationStarted = true;
        m_audioManager.PlayAllMusic();

        foreach (GameObject beat in m_beats)
            beat.SetActive(true);

        m_offset.gameObject.SetActive(true);

        m_instructions.text = "Calibrate audio and display by using the <color=#DB5A8C>A</color> and <color=#DB5A8C>D</color> keys.\n" +
            "Hit <color=#DB5A8C>SPACE</color> when they are in sync.";
        m_offset.text = m_audioOffset.ToString("+0.00;-0.00");
        yield return new WaitForSeconds(m_audioOffset);

        m_rhythmTimer.StartTimer();
        m_audioCalibrationReady = true;
    }

    private void StartInputCalibration()
    {
        m_inputCalibrationStarted = true;
        m_instructions.text = "Calibrate input and audio. Hit <color=#DB5A8C>SPACE</color> in time with the metronome.";
        m_audioManager.Fade(1, 1.0f / 9.0f);
    }
}
