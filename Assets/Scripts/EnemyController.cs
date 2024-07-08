using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject pointA;
    public GameObject pointB;
    private Rigidbody2D rb;
    private Animator anim;
    private Transform currentPoint;
    public float speed;

    public AudioClip sonidoMuerto;
    public AudioClip sonidoGolpe;
    public AudioSource quienEmite;
    public int vidas = 3;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        currentPoint = pointA.transform;
        anim.SetBool("Run", true);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 direction = currentPoint.position - transform.position;
        if (currentPoint == pointB.transform)
        {
            rb.velocity = new Vector2(speed, 0);
        }
        else
        {
            rb.velocity = new Vector2(-speed, 0);
        }

        if (Vector2.Distance(transform.position, currentPoint.position) < 0.5f && currentPoint == pointB.transform)
        {
            flip();
            currentPoint = pointA.transform;

        }
        if (Vector2.Distance(transform.position, currentPoint.position) < 0.5f && currentPoint == pointA.transform)
        {
            flip();
            currentPoint = pointB.transform;
        }

    }

    private void flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    //cuando se muera ejecutar la animación de muerte
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Jugador" && Input.GetKey(KeyCode.Space))
        {
            vidas--;
            if (vidas == 0)
            {
                anim.SetBool("Run", false);
                anim.SetBool("Death", true);
                quienEmite.PlayOneShot(sonidoMuerto, 1f);
                Destroy(gameObject, 0.5f);
            }
            else
            {
                anim.SetBool("Attack", true);
                quienEmite.PlayOneShot(sonidoGolpe, 1f);
            }
        }
        else
        {
            if (collision.gameObject.tag == "Jugador" && !Input.GetKey(KeyCode.Space))
            {
                anim.SetBool("Hit", true);
                collision.gameObject.GetComponent<CapryController>().restarVida();
            }

        }
    }


    public void Die()
    {
        anim.SetBool("Run", false);
        anim.SetBool("Death", true);
        quienEmite.PlayOneShot(sonidoMuerto, 1f);
        Destroy(gameObject, 0.5f);
    }


}
