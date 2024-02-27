using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BeatGuide : MonoBehaviour
{
    [SerializeField] private List<GameObject> m_bumpInstances = new();
    [SerializeField] private GameObject m_bumpPrefab;
    [SerializeField] private GameObject m_bumpReceptacle;
    private bool m_shown;
    [SerializeField] private GameObject m_spaceText;

    private void Update()
    {
        if (LevelManager.Instance.RhythmTimer.IsBeat())
        {
            GameObject bump = Instantiate(m_bumpPrefab, transform);
            bump.GetComponent<SpriteRenderer>().enabled = m_shown;
            m_bumpInstances.Add(bump);
        }

        for (int i = 0; i < m_bumpInstances.Count;  i++)
        {
            m_bumpInstances[i].transform.localPosition -= new Vector3(2.0f * Time.deltaTime, 0.0f, 0.0f);
            if (m_bumpInstances[i].transform.localPosition.x <= -968.0f)
            {
                Destroy(m_bumpInstances[i]);
                m_bumpInstances[i] = null;

                if (m_shown)
                {
                    StartCoroutine(ShowSpaceText());
                }
            }
        }

        m_bumpInstances.RemoveAll(bump => bump == null);
    }

    public IEnumerator Show(float time)
    {
        if (m_shown)
            yield break;

        m_shown = true;

        for (int i = 1; i <= 16; i++)
        {
            m_bumpReceptacle.transform.localPosition = Vector2.Lerp(new Vector2(-964.0f, -534.0f), new Vector2(-964.0f, -536.0f), i / 16.0f);
            yield return new WaitForSeconds(0.0125f);
        }

        foreach (GameObject bump in m_bumpInstances)
        {
            if (bump != null)
                bump.GetComponent<SpriteRenderer>().enabled = true;
        }

        yield return new WaitForSeconds(time);

        m_shown = false;

        foreach (GameObject bump in m_bumpInstances)
        {
            if (bump != null)
                bump.GetComponent<SpriteRenderer>().enabled = false;
        }

        for (int i = 1; i <= 16; i++)
        {
            m_bumpReceptacle.transform.localPosition = Vector2.Lerp(new Vector2(-964.0f, -536.0f), new Vector2(-964.0f, -534.0f), i / 16.0f);
            yield return new WaitForSeconds(0.0125f);
        }
    }

    private IEnumerator ShowSpaceText()
    {
        m_spaceText.SetActive(true);
        yield return new WaitForSeconds(0.125f);
        m_spaceText.SetActive(false);
    }
}
