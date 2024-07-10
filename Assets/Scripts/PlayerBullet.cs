using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    // Update is called once per frame
    public float Power;

    void Update()
    {
        transform.position = new Vector2(transform.position.x + 30.0f * Time.deltaTime, transform.position.y);
        if (transform.position.x > 10)
            Destroy(gameObject);
    }
}
