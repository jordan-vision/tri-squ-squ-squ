using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : MonoBehaviour
{
    [SerializeField] GameObject m_bulletPrefab;
    int m_counter = 0;
    float m_homeY;
    [SerializeField] List<int> m_shootingBeats;
    [SerializeField] bool m_spreadShot;
    [SerializeField] bool m_sway;
    [SerializeField] float m_swayAmplitude = 2.0f;
    [SerializeField] float m_swayFrequency;
    [SerializeField] bool m_targetPlayer;
    float m_timer = 0.0f;

    public bool Slide = true;
    public List<int> ShootingBeats { get { return m_shootingBeats; } set { m_shootingBeats = value; } }

    public bool SpreadShot { get { return m_spreadShot; } set { m_spreadShot = value; } }

    public bool TargetPlayer { get { return m_targetPlayer; } set {  m_targetPlayer = value; } }

    private void Awake()
    {
        m_homeY = transform.position.y;
        m_timer = LevelManager.Instance.RhythmTimer.TimerValue;
        
        if (Slide)
            StartCoroutine(SlideIn());
    }

    void Update()
    {
        m_timer += Time.deltaTime;
        transform.Rotate(new Vector3(0, 0, 1.0f));

        if (m_sway)
        {
            transform.position = new Vector2(7, m_swayAmplitude * Mathf.Sin(Mathf.PI * m_swayFrequency * m_timer) + m_homeY);
        }

        if (LevelManager.Instance.RhythmTimer.IsStartOfMeasure())
        {
            m_counter = 1 - m_counter;

            if (m_shootingBeats.Contains(m_counter))
                Shoot();
        }
    }

    void Shoot()
    {
        GameObject bulletInstace = Instantiate(m_bulletPrefab, transform.position, Quaternion.identity);

        if (m_spreadShot)
        {
            GameObject upperBullet = Instantiate(m_bulletPrefab, transform.position, Quaternion.identity);
            GameObject lowerBullet = Instantiate(m_bulletPrefab, transform.position, Quaternion.identity);
            upperBullet.GetComponent<EnemyBullet>().Target = new Vector2(-0.97f, 0.26f);
            lowerBullet.GetComponent<EnemyBullet>().Target = new Vector2(-0.97f, -0.26f);
        }

        if (m_targetPlayer)
        {
            Vector2 playerPosition = LevelManager.Instance.Player.transform.position;
            Vector2 direction = (playerPosition - (Vector2)transform.position).normalized;
            bulletInstace.GetComponent<EnemyBullet>().Target = direction;
        }
    }

    IEnumerator SlideIn()
    {
        for (int i = 0; i < 16; i++)
        {
            transform.position = new Vector2(11.0f - 0.25f * i, transform.position.y);
            yield return new WaitForSeconds(0.0125f);
        }
    }
}
