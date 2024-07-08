using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class CapryController : MonoBehaviour
{
    private Rigidbody2D rb2D;

    private Animator Animator;

    public float JumpForce = 7;
    public float Speed = 5;
    private float Horizontal;

    public bool isOnTheGround;

    public Text textoContador, textoGanar, textoTiempo, textoVidas; //Inicializo variables para los textos
    public AudioClip sonidoAlGanar; //efecto de sonido al ganar
    public AudioClip sonidoAlPerder; //efecto de sonido al perder
    public AudioClip sonidoTiempoAcabado; //efecto de sonido al perder
    public AudioSource quienEmite; //variable de tipo AudioSource que asociaremos a nuestro jugador
    private int contador; // inizializamos el contador de los coleccionables recogidos
    public List<GameObject> coleccionablesDesactivados; //lista de coleccionables desactivados
    public List<GameObject> cajasDesactivadas; //lista de coleccionables desactivados

    public AudioClip inicioJuego;
    public AudioClip inicioBoss;
    public AudioClip sonidoColeccionable;
    public AudioClip sonidoColeccionableVida;
    public AudioClip sonidoCajaAlDesaparecer;
    public AudioClip sonidoCajaAlAparecer;
    public AudioClip sonidoLava;

    public AudioClip sonidoGolpe;

    public AudioClip sonidoPinchos;

    public AudioClip sonidoSaltar;
    public AudioClip perderBoss;
    public bool win = false;

    float time = 90f; //tiempo
    public float levelTime; //tiempo
    private float lifes; //vidas 

    public float cantVidas = 3;
    private BoxCollider2D boxCollider;


    private void Awake()
    {
        //Grab references for rigidbody and animator from object
        rb2D = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
        lifes = cantVidas;
        time = levelTime;
        win = false;

        switch (SceneManager.GetActiveScene().name)
        {
            case "menuInicio":
                Animator.SetBool("eating", true);
                break;

            case "MenuOpciones":
                Animator.SetBool("eating", true);
                break;
            default:
                Animator.SetBool("eating", false);
                break;

        }
    }


    void Update()
    {
        //para manejar el tiempo
        if (time > 0 && !win)
        {
            time -= Time.deltaTime;
            int minutes = Mathf.FloorToInt(time / 60);
            int seconds = Mathf.FloorToInt(time % 60);
            textoTiempo.text = minutes.ToString("00") + ":" + seconds.ToString("00");
        }
        if (time < 5)
        {
            quienEmite.pitch = 1.5f;
        }
        else
        {
            quienEmite.pitch = 1f;
        }

        if (time < 0 && !win)
        {
            quienEmite.PlayOneShot(sonidoTiempoAcabado, 1f);
            restarVida(0.5f);
            restartLevel();
        }

        //para mover la cabra de manera horizontal
        Horizontal = Input.GetAxisRaw("Horizontal"); // A, D, Left, Right

        if (Horizontal < 0.00f) transform.localScale = new Vector3(-2.66f, 2.66f, 2.66f); // para caminar a la izquierda
        else if (Horizontal > 0.00f) transform.localScale = new Vector3(2.66f, 2.66f, 2.66f); // para caminar a la derecha
        Animator.SetBool("running", Horizontal != 0.0f); // si esta corriendo aplicar animación correr

        Debug.DrawRay(transform.position, Vector3.down * 1.0f, Color.red);
        if (Physics2D.Raycast(transform.position, Vector3.down, 1.0f))
        {
            isOnTheGround = true;
        }
        else
        {
            isOnTheGround = false;
        }


        //para golpear presionando la tecla espacio
        Animator.SetBool("hitting", (Input.GetKeyDown(KeyCode.Space)) && isOnTheGround); // si esta golpeando aplicar animación golpear
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.LeftControl)) && isOnTheGround)
        {
            hit();
        }


        //para saltar si está en el piso y presiona la tecla W
        Animator.SetBool("jumping", (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && isOnTheGround); // si esta brincando aplicar animación brincar
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && isOnTheGround)
        {
            jump();
        }


    }

    void jump()
    {
        quienEmite.PlayOneShot(sonidoSaltar, 1f);
        rb2D.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
    }
    public void eat(bool eating)
    {
        Animator.SetBool("eating", eating);
    }

    void hit()
    {
        quienEmite.PlayOneShot(sonidoGolpe, 1f);
    }

    public void restarVida(float soundVolume = 1.0f)
    {
        quienEmite.PlayOneShot(sonidoAlPerder, soundVolume);
        lifes--;
        textoVidas.text = lifes.ToString();

        if (lifes == 0)
        {
            restartLevel();
        }
    }

    private void FixedUpdate()
    {
        rb2D.velocity = new Vector2(Horizontal * Speed, rb2D.velocity.y);
    }




    //Se ejecuta al entrar a un objeto con la opción isTrigger seleccionada
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Coleccionable"))
        {
            other.gameObject.SetActive(false);
            quienEmite.PlayOneShot(sonidoColeccionable, 1f);
            contador = contador + 1;
            setTextoContador(); // actualizo el texto del contador
            coleccionablesDesactivados.Add(other.gameObject); //agrego el coleccionable a la lista de coleccionables desactivados
        }
        if (other.gameObject.CompareTag("ColeccionableVida"))
        {
            other.gameObject.SetActive(false);
            quienEmite.PlayOneShot(sonidoColeccionableVida, 1f);

            lifes = lifes + 1;
            textoVidas.text = lifes.ToString();
            coleccionablesDesactivados.Add(other.gameObject); //agrego el coleccionable a la lista de coleccionables desactivados
        }

        if (other.gameObject.CompareTag("Ganar"))
        {
            winLevel();
        }


        if (other.gameObject.CompareTag("Obstaculo"))
        {
            quienEmite.PlayOneShot(sonidoPinchos, 1f);
            restarVida();
        }
        if (other.gameObject.CompareTag("Hoyo"))
        {
            restarVida();
            resetPosition();
        }
        if (other.gameObject.CompareTag("HoyoPincho"))
        {
            restarVida();
            quienEmite.PlayOneShot(sonidoPinchos, 1f);
            resetPosition();
        }
        if (other.gameObject.CompareTag("HoyoLava"))
        {
            restarVida();
            quienEmite.PlayOneShot(sonidoLava, 1f);
            resetPosition();
        }


    }

    void resetPosition()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case "nivel-1":
                transform.position = new Vector2(-4, -3.3f);
                transform.rotation = Quaternion.Euler(0, 0, 0);
                break;

            case "nivel-2":
                transform.position = new Vector2(-10, -1f);
                transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case "nivel-3":
                if (lifes == 0)
                {
                    textoGanar.text = "¡Perdiste!";
                    ejecutarSonido(perderBoss);
                }
                transform.position = new Vector2(-41.5f, 1.7f);
                transform.rotation = Quaternion.Euler(0, 0, 0);
                quienEmite.PlayOneShot(inicioJuego, 1f);
                ejecutarSonido(inicioBoss, 4f);
                break;

        }
    }




    void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("FailBox"))
        {
            //esperar un delay antes de desaparecer la caja
            StartCoroutine(DesaparecerCaja(collision.gameObject, 0.7f));

            //despues de un delay desaparecer la caja esperar otro delay para mostrarla
            StartCoroutine(MostrarCaja(collision.gameObject, 2.5f));


        }
    }

    IEnumerator DesaparecerCaja(GameObject caja, float delay)
    {
        quienEmite.PlayOneShot(sonidoCajaAlDesaparecer, 0.6f);
        yield return new WaitForSeconds(delay);
        Rigidbody2D cajaRb = caja.GetComponent<Rigidbody2D>();
        cajaRb.gravityScale = 1f;
        caja.SetActive(false);
        cajasDesactivadas.Add(caja);
    }

    IEnumerator MostrarCaja(GameObject caja, float delay)
    {
        yield return new WaitForSeconds(delay);
        caja.SetActive(true);
        cajasDesactivadas.Remove(caja);
        quienEmite.PlayOneShot(sonidoCajaAlAparecer, 0.6f);
    }

    // // Detectar cuando el jugador deja de estar en contacto con el Piso
    // void OnCollisionExit2D(Collision2D collision)
    // {
    //     if (collision.gameObject.CompareTag("Piso"))
    //     {
    //         isOnTheGround = false;
    //     }
    // }

    void setTextoContador()
    {
        textoContador.text = contador.ToString();
    }

    public void winLevel()
    {
        win = true;
        textoGanar.text = "¡Ganaste!";

        ejecutarSonido(sonidoAlGanar, 1f);

        rb2D.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;


        switch (SceneManager.GetActiveScene().name)
        {
            case "nivel-1":
                CambiarDeNivel("nivel-2");
                rb2D.constraints = RigidbodyConstraints2D.FreezeRotation;
                win = false;
                lifes = cantVidas;

                break;

            case "nivel-2":
                CambiarDeNivel("nivel-3");
                quienEmite.PlayOneShot(inicioBoss, 1f);
                rb2D.constraints = RigidbodyConstraints2D.FreezeRotation;
                win = false;
                lifes = cantVidas;
                break;

            case "nivel-3":
                CambiarDeNivel("MenuInicio");
                break;

            default:
                CambiarDeNivel("MenuInicio");
                break;
        }
        if (lifes == 0)
        {
            lifes = cantVidas;
        }
    }

    public void ejecutarSonido(AudioClip sonido, float delay = 2f, float volumen = 1f)
    {
        StartCoroutine(ExecSound(sonido, delay, volumen));

    }

    IEnumerator ExecSound(AudioClip sonido, float delay, float volumen)
    {
        quienEmite.PlayOneShot(sonido, volumen);
        //esperar el delay despues de ejecutar el sonido
        yield return new WaitForSeconds(delay);

    }

    void restartLevel()
    {
        //descongelar las teclas del teclado
        rb2D.constraints = RigidbodyConstraints2D.None;
        rb2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        contador = 0;

        time = levelTime;
        setTextoContador();

        if (coleccionablesDesactivados.Count > 0)
        {
            foreach (GameObject coleccionable in coleccionablesDesactivados)
            {
                coleccionable.SetActive(true);
            }

        }

        //cambiar la posición del jugador



        switch (SceneManager.GetActiveScene().name)
        {
            case "nivel-1":
                if (lifes == 0)
                {
                    lifes = cantVidas;
                    textoVidas.text = lifes.ToString();
                }
                quienEmite.PlayOneShot(inicioJuego, 1f);
                transform.position = new Vector2(-4, -3.3f);
                transform.rotation = Quaternion.Euler(0, 0, 0);
                break;

            case "nivel-2":
                if (lifes == 0)
                {
                    lifes = cantVidas;
                    CambiarDeNivel("nivel-1", 0.1f);
                    break;
                }

                // CambiarDeNivel("nivel-2", 0.1f);
                quienEmite.PlayOneShot(inicioJuego, 1f);
                transform.position = new Vector2(-10, -1f);
                transform.rotation = Quaternion.Euler(0, 0, 0);
                break;

            case "nivel-3":
                quienEmite.PlayOneShot(inicioBoss, 1f);
                if (lifes == 0)
                {
                    textoGanar.text = "¡Perdiste!";
                    ejecutarSonido(perderBoss);
                    CambiarDeNivel("nivel-1", 4);
                    textoGanar.text = "";
                    lifes = cantVidas;
                }
                quienEmite.PlayOneShot(inicioJuego, 1f);
                transform.position = new Vector2(-41.5f, 1.7f);
                transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            default:
                CambiarDeNivel("MenuInicio");

                break;
        }
    }

    public void CambiarDeNivel(string escena, float delay = 2f)
    {
        StartCoroutine(CambiarEscena(escena, delay));
    }

    IEnumerator CambiarEscena(string escena, float delay)
    {

        //esperar el delay antes de ir a la escena
        yield return new WaitForSeconds(delay);
        //cargar la escena
        SceneManager.LoadScene(escena);
        restartLevel();

    }


}
