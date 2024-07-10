using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    float m_countDown = float.MaxValue;
    bool m_isNextStanceBarricade;
    [SerializeField] GameObject[] m_squares;
    private enum Stance { NORMAL, BARRICADE, DIAMOND };
    private Stance m_stance = Stance.NORMAL;

    float m_timer;

    private void Awake()
    {
        m_isNextStanceBarricade = Random.value > 0.5f;
        m_timer = LevelManager.Instance.RhythmTimer.TimerValue;
        StartCoroutine(MoveTowards(gameObject, new Vector2(7, 2.0f * Mathf.Sin(Mathf.PI * 0.25f * 0.2f))));
    }

    private void Update()
    {
        m_timer += Time.deltaTime;

        if (m_stance != Stance.NORMAL)
            return;

        if (m_countDown <= 4.0f)
            m_countDown -= Time.deltaTime;

        if (IsBossDefeated())
            Destroy(gameObject);

        if (LevelManager.Instance.RhythmTimer.IsStartOfMeasure() && LevelManager.Instance.RhythmTimer.BeatCounter % 4 == 0)
        {
            if (m_countDown > 5)
                m_countDown = 4.0f;
        }

        if (m_countDown <= 0.2f)
        {
            if (m_isNextStanceBarricade)
                StartCoroutine(BarricadeStance());
            else
                StartCoroutine(DiamondStance());

            m_isNextStanceBarricade = !m_isNextStanceBarricade;
            return;
        }

        transform.position = new Vector2(transform.position.x, 2.0f * Mathf.Sin(Mathf.PI * 0.25f * m_timer));

        float angle = Mathf.PI * 0.5f * m_timer;
        if (m_squares[0] != null) m_squares[0].transform.localPosition = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        if (m_squares[1] != null) m_squares[1].transform.localPosition = new Vector2(-Mathf.Sin(angle), Mathf.Cos(angle));
        if (m_squares[2] != null) m_squares[2].transform.localPosition = new Vector2(-Mathf.Cos(angle), -Mathf.Sin(angle));
        if (m_squares[3] != null) m_squares[3].transform.localPosition = new Vector2(Mathf.Sin(angle), -Mathf.Cos(angle));

        float totalHP = 0;
        foreach (GameObject square in m_squares)
        {
            if (square != null)
                totalHP += square.GetComponent<EnemyHP>().HP;
        }

        LevelManager.Instance.AudioManager.SetVolume(8, 1.0f - totalHP / 24.0f);
    }

    private IEnumerator BarricadeStance()
    {
        m_stance = Stance.BARRICADE;
        m_countDown = float.MaxValue;

        if (m_squares[0] != null)
            StartCoroutine(MoveTowards(m_squares[0], new Vector2(7, 3)));
        
        if (m_squares[1] != null)
            StartCoroutine(MoveTowards(m_squares[1], new Vector2(7,-1)));

        if (m_squares[2] != null)
            StartCoroutine(MoveTowards(m_squares[2], new Vector2(7, 1)));

        if (m_squares[3] != null)
            StartCoroutine(MoveTowards(m_squares[3], new Vector2(7, -3)));

        foreach (GameObject square in m_squares)
        {
            if (square == null)
                continue;

            square.GetComponent<BasicEnemy>().ShootingBeats = new List<int>() { 0 };
            square.GetComponent<BasicEnemy>().TargetPlayer = true;
        }

        yield return new WaitForSeconds(4.0f);

        StartCoroutine(ResetEnemyValues(false));
        foreach (GameObject square in m_squares)
        {
            if (square == null)
                continue;

            square.GetComponent<BasicEnemy>().SpreadShot = true;
            square.GetComponent<BasicEnemy>().TargetPlayer = false;
        }

        yield return new WaitForSeconds(3.8f);

        StartCoroutine(ResetEnemyValues(true));
        m_stance = Stance.NORMAL;
    }

    private IEnumerator DiamondStance()
    {
        m_stance = Stance.DIAMOND;
        m_countDown = float.MaxValue;

        foreach (GameObject square in m_squares)
        {
            if (square == null)
                continue;

            square.GetComponent<BasicEnemy>().ShootingBeats = new List<int>() { 1 };
            square.GetComponent<BasicEnemy>().TargetPlayer = true;
        }

        for (int i = 0; i < 4; i++)
        {
            if (m_squares[i] != null)
                StartCoroutine(MoveTowards(m_squares[i], new Vector3(-7.0f, Random.Range(-4.0f, 4.0f))));

            if (m_squares[(i + 1) % 4] != null)
                StartCoroutine(MoveTowards(m_squares[(i + 1) % 4], new Vector3(Random.Range(-7.0f, 7.0f), -4.0f)));

            if (m_squares[(i + 2) % 4] != null)
                StartCoroutine(MoveTowards(m_squares[(i + 2) % 4], new Vector3(7.0f, Random.Range(-4.0f, 4.0f))));

            if (m_squares[(i + 3) % 4] != null)
                StartCoroutine(MoveTowards(m_squares[(i + 3) % 4], new Vector3(Random.Range(-7.0f, 7.0f), 4.0f)));

            yield return new WaitForSeconds(1.8f);
        }

        StartCoroutine(ResetEnemyValues(true));
        m_stance = Stance.NORMAL;
    }

    private IEnumerator MoveTowards(GameObject square, Vector2 newPosition)
    {
        Vector2 originalPosition = square.transform.position;

        for (int i = 0; i < 16; i++)
        {
            if (square == null)
                yield break;

            square.transform.position = Vector2.Lerp(originalPosition, newPosition, i / 15.0f);
            yield return new WaitForSeconds(0.0125f);
        }
    }

    private IEnumerator ResetEnemyValues(bool resetPosition)
    {
        if (m_squares[0] != null)
            m_squares[0].GetComponent<BasicEnemy>().ShootingBeats = new List<int>() { 0 };

        if (m_squares[1] != null)
            m_squares[1].GetComponent<BasicEnemy>().ShootingBeats = new List<int>() { 0 };

        if (m_squares[2] != null)
            m_squares[2].GetComponent<BasicEnemy>().ShootingBeats = new List<int>() { 1 };

        if (m_squares[3] != null)
            m_squares[3].GetComponent<BasicEnemy>().ShootingBeats = new List<int>() { 1 };

        foreach (GameObject square in m_squares)
        {
            if (square == null)
                continue;

            square.GetComponent<BasicEnemy>().SpreadShot = false;
            square.GetComponent<BasicEnemy>().TargetPlayer = false;
        }

        if (!resetPosition)
            yield break;

        float angle = Mathf.PI * 0.5f * (m_timer + 0.2f);
        Vector2 newCenter = new Vector2(transform.position.x, 2.0f * Mathf.Sin(Mathf.PI * 0.25f * (m_timer + 0.2f)));

        if (m_squares[0] != null)
            StartCoroutine(MoveTowards(m_squares[0], newCenter + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle))));

        if (m_squares[1] != null)
            StartCoroutine(MoveTowards(m_squares[1], newCenter + new Vector2(-Mathf.Sin(angle), Mathf.Cos(angle))));

        if (m_squares[2] != null)
            StartCoroutine(MoveTowards(m_squares[2], newCenter + new Vector2(-Mathf.Cos(angle), -Mathf.Sin(angle))));

        if (m_squares[3] != null)
            StartCoroutine(MoveTowards(m_squares[3], newCenter + new Vector2(Mathf.Sin(angle), -Mathf.Cos(angle))));
    }

    private bool IsBossDefeated()
    {
        foreach (GameObject square in m_squares)
        {
            if (square != null)
                return false;
        }
        return true;
    }
}
