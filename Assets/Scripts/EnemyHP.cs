using UnityEngine;

public class EnemyHP : MonoBehaviour
{
    [SerializeField] private float m_hp = 4;
    [SerializeField] GameObject m_explosionPrefab;

    public float HP { get { return m_hp; } }
    private void TakeDamage(float damage)
    {
        m_hp -= damage;
        StartCoroutine(LevelManager.Instance.Player.ChangeScore((int)(damage * damage * 1000)));
        if (m_hp <= 0)
        {
            Instantiate(m_explosionPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag.Equals("PlayerBullet"))
        {
            TakeDamage(collider.GetComponent<PlayerBullet>().Power);
            GetComponent<Animator>().SetTrigger("Hurt");
            Destroy(collider.gameObject);
        }
    }
}
