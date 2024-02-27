using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class RhythmTimer : MonoBehaviour
{
    [SerializeField] private float m_beatLength;
    private DateTime m_lastShot;
    [SerializeField] private float m_measureLength;
    private bool m_isStarted = false;
    private float m_previousTimerValue = 0;
    private float m_timerValue = 0;
    [SerializeField] private List<float> m_timeStamps;

    public int BeatCounter = 0;

    public float PreviousTimerValue { 
        get { 
            float value = m_previousTimerValue;
            StartCoroutine(SetPreviousTimerValue());
            return value;
        } 
    }
    public float TimerValue { get { return m_timerValue; } }

    // Update is called once per frame
    void Update()
    {
        if (!m_isStarted)
            return;

        m_timerValue += Time.deltaTime;
        m_timerValue %= m_measureLength;

        if (IsStartOfMeasure())
            BeatCounter++;
    }


    public float ComputeBulletPower()
    {
        float basePower = 1;
        basePower *= TimingAccuracy();

        float timeTillPreviousShot = (float)(DateTime.Now - m_lastShot).TotalSeconds;
        float spamPunish = Mathf.Min(1, Math.Max(0, (timeTillPreviousShot / m_beatLength) * 2 - 1));
        basePower *= spamPunish;
        m_lastShot = DateTime.Now;

        Debug.Log(basePower);
        return basePower;
    }

    public bool IsBeat()
    {
        return GetOffsetFromClosestTimestamp(PreviousTimerValue) < 0.0f && GetOffsetFromClosestTimestamp() >= 0.0f;
    }

    public bool IsStartOfMeasure()
    {
        return PreviousTimerValue >= m_timerValue;
    }

    public float GetOffsetFromClosestTimestamp()
    {
        return GetOffsetFromClosestTimestamp(m_timerValue);
    }

    public float GetOffsetFromClosestTimestamp(float time)
    {
        float offset = m_beatLength;

        foreach (float timeStamp in m_timeStamps)
        {
            if (Mathf.Abs(time - timeStamp) < Mathf.Abs(offset))
                offset = time - timeStamp;
        }

        return offset;
    }

    private IEnumerator SetPreviousTimerValue()
    {
        yield return new WaitForEndOfFrame();
        m_previousTimerValue = m_timerValue;
    }

    public void StartTimer()
    {
        m_lastShot = DateTime.Now;
        m_isStarted = true;
    }

    public void StopTimer()
    {
        m_isStarted = false;
        m_timerValue = 0.0f;
    }

    public float TimingAccuracy()
    {
        return (1 - (Mathf.Abs(GetOffsetFromClosestTimestamp(m_timerValue - Settings.InputLatency)) / m_beatLength)) * 2 - 1;
    }
}
