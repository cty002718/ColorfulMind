using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed = 2.0f;

    Rigidbody2D rigidbody2D;
    Transform target;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
    }

    void OnCollisionStay2D(Collision2D other)
    {
        HeroController player = other.gameObject.GetComponent<HeroController>();

        if (player != null)
        {
            player.ChangeHealth(-1);
        }
    }

}
