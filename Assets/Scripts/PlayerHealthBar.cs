using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] private Sprite m_empty;

    public void UpdateHealth(int health)
    {
        int i = 0;

        foreach (Transform box in transform)
        {
            if (i >= health)
                box.GetComponent<SpriteRenderer>().sprite = m_empty;

            i++;
        }
    }
}
