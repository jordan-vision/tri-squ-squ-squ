using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Ending : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_creditsText, m_finalScore, m_retryOption, m_quitOption;
    private bool m_gameEnded = false;
    private bool m_retrySelected;

    private void Update()
    {
        if (!m_gameEnded)
            return;

        if (Controls.GetLeftPress() || Controls.GetRightPress())
        {
            m_retrySelected = !m_retrySelected;

            if (m_retrySelected)
            {
                m_retryOption.color = new Color(1.0f, 192.0f / 255.0f, 192.0f / 255.0f);
                m_quitOption.color = new Color(219.0f / 255.0f, 90.0f / 255.0f, 140.0f / 255.0f);
            } else
            {
                m_retryOption.color = new Color(219.0f / 255.0f, 90.0f / 255.0f, 140.0f / 255.0f);
                m_quitOption.color = new Color(1.0f, 192.0f / 255.0f, 192.0f / 255.0f);
            }
        }

        if (Controls.GetShootPress())
        {
            if (m_retrySelected)
                GameManager.Instance.RestartGame();

            else
                GameManager.Instance.ExitGame();
        }
    }

    public IEnumerator StartEnding()
    {

        while (!(LevelManager.Instance.RhythmTimer.IsStartOfMeasure() && LevelManager.Instance.RhythmTimer.BeatCounter % 2 == 0))
            yield return new WaitForEndOfFrame();

        yield return new WaitForSeconds(2.0f);
        m_creditsText.gameObject.SetActive(true);

        yield return new WaitForSeconds(2.0f);
        m_creditsText.text = "For the <color=#FFC0C0>CGD Game Jam</color>, Fall 2023";

        yield return new WaitForSeconds(2.0f);
        m_creditsText.text = "Thank you for playing!";

        yield return new WaitForSeconds(2.0f);
        m_finalScore.text = $"Final Score: <color=#ffc0c0>{LevelManager.Instance.Player.Score.ToString("00000")}</color>";
        m_retryOption.gameObject.SetActive(true);
        m_quitOption.gameObject.SetActive(true);

        m_gameEnded = true;
    }
}
