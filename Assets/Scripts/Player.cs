using System.Collections;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_addedScoreText, m_scoreText;
    [SerializeField] GameObject m_bulletPrefab;
    [SerializeField] TextMeshPro m_feedbackText;
    [SerializeField] PlayerHealthBar m_healthBar;
    [SerializeField] int m_hp = 16;
    bool m_isInvulnerable;
    Rigidbody2D m_rb;
    int m_score = 0;
    [SerializeField] int m_xSpeed, m_ySpeed;

    public int Score { get { return m_score; } }

    private void Start()
    {
        m_rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        ComputeVelocity();
        if (Controls.GetShootPress() && LevelManager.Instance.IsStarted)
            Shoot();
    }

    public IEnumerator ChangeScore(int value)
    {
        m_score += value;
        m_scoreText.text = m_score.ToString("00000");
        m_addedScoreText.text = value.ToString("+000;-000");
        m_addedScoreText.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.25f);
        m_addedScoreText.gameObject.SetActive(false);
    }

    private void ComputeVelocity()
    {
        int vertical = Controls.GetVertical();
        int horizontal = Controls.GetHorizontal();

        m_rb.velocity = new Vector2(horizontal * m_xSpeed, vertical * m_ySpeed);
    }

    private void Shoot()
    {
        float bulletPower = LevelManager.Instance.RhythmTimer.ComputeBulletPower();

        if (bulletPower > 0.0f)
        {
            GameObject bulletInstance = Instantiate(m_bulletPrefab, transform.position, Quaternion.identity);
            bulletInstance.GetComponent<PlayerBullet>().Power = bulletPower;
        } else
        {
            StartCoroutine(WriteFeedBack("MISS"));
            StartCoroutine(ChangeScore(-500));
        }

        if (bulletPower <= 0.5f)
        {
            StartCoroutine(LevelManager.Instance.BeatGuide.Show(4.0f));

            if (bulletPower != 0.0f)
                StartCoroutine(WriteFeedBack("WHOOPS"));
        }

        else if (bulletPower <= 0.75f)
            StartCoroutine(WriteFeedBack("OK"));

        else if (bulletPower <= 0.9f)
            StartCoroutine(WriteFeedBack("GOOD"));

        else
            StartCoroutine(WriteFeedBack("PERFECT"));
    }

    private IEnumerator TakeDamage() 
    {
        m_hp--;
        StartCoroutine(ChangeScore(-500));

        if (m_hp <= 0)
        {
            m_feedbackText.gameObject.SetActive(false);
            StartCoroutine(LevelManager.Instance.KillPlayer());
            yield break;
        }

        m_isInvulnerable = true;
        m_healthBar.gameObject.SetActive(true);
        m_healthBar.UpdateHealth(m_hp);

        yield return new WaitForSeconds(2.0f);

        m_healthBar.gameObject.SetActive(false);
        m_isInvulnerable = false;
    }

    private IEnumerator WriteFeedBack(string text)
    {
        m_feedbackText.text = text;
        m_feedbackText.color = new Color(219.0f / 255.0f, 90.0f / 255.0f, 140.0f / 255.0f);
        yield return new WaitForSeconds(0.125f);
        m_feedbackText.color = new Color(1.0f, 192.0f / 255.0f, 192.0f / 255.0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!m_isInvulnerable && collision.tag.Equals("EnemyBullet")) {
            GetComponent<Animator>().SetTrigger("Hurt");
            StartCoroutine(TakeDamage());
            Destroy(collision.gameObject);
        }
    }
}
