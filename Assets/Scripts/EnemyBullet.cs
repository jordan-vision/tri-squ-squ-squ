using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    private float rotationDirection;

    public Vector2 Target = Vector2.left;

    private void Awake()
    {
        rotationDirection = Random.value < 0.5 ? -1 : 1;
    }

    void Update()
    {
        if (transform.position.x < -10)
            Destroy(gameObject);

        transform.position += 11.25f * Time.deltaTime * (Vector3)Target;
        transform.Rotate(new Vector3(0, 0, rotationDirection * 300.0f * Time.deltaTime));
    }
}
