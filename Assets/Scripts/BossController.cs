using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

using JugadorController = CapryController;

public class BossController : MonoBehaviour
{

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
        anim.SetBool("Walking", true);
    }


    private void flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }



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



    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Jugador" && Input.GetKey(KeyCode.Space))
        {

            vidas--;
            if (vidas == 0)
            {
                //el boss ha muerto
                anim.SetBool("Walking", false);
                anim.SetBool("Hurting", false);
                anim.SetBool("Dying", true); 
                quienEmite.PlayOneShot(sonidoMuerto, 1f);
                Destroy(gameObject, 0.5f);

                //gano el jugador 
                collision.gameObject.GetComponent<CapryController>().winLevel();

            }
            else
            {
                //el boss ha sido golpeado
                anim.SetBool("Attacking", false);
                anim.SetBool("Hurting", true); 
                quienEmite.PlayOneShot(sonidoGolpe, 1f);
                Invoke("desactivarEstadoGolpeado", 1);
            }
        }
        else
        {
            if (collision.gameObject.tag == "Jugador" && !Input.GetKey(KeyCode.Space))
            {

                //el boss ataca al jugador
                anim.SetBool("Attacking", true);
                anim.SetBool("Hurting", false);
                collision.gameObject.GetComponent<CapryController>().restarVida();
            }

        }
    }


    private void desactivarEstadoGolpeado()
    {
        anim.SetBool("Hurting", false);
    }

}
