using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatueBehavior : MonoBehaviour
{
    public GameObject hero;

    void Update()
    {
        if (hero.transform.position.y < this.transform.position.y)
            gameObject.layer = hero.layer - 1;
    }
}
