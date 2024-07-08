using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MenuPrincipal : MonoBehaviour
{
    public AudioClip sonidoClick;
    public AudioSource quienEmite;

    public void Jugar()
    {
        ejecutarSonido(sonidoClick);
        CambiarDeNivel("nivel-1");
    }
    public void Opciones()
    {
        ejecutarSonido(sonidoClick);
        CambiarDeNivel("MenuOpciones");
    }
    public void Salir()
    {
        ejecutarSonido(sonidoClick);
        Debug.Log("Salir...");
        Application.Quit();
    }
    public void Atras()
    {
        ejecutarSonido(sonidoClick);
        CambiarDeNivel("MenuInicio");
    }


    public void ejecutarSonido(AudioClip sonido, float volumen = 1f)
    {
        StartCoroutine(ExecSound(sonido, 1f, volumen));

    }

    IEnumerator ExecSound(AudioClip sonido, float delay, float volumen)
    {
        //esperar el delay despues de ejecutar el sonido
        quienEmite.PlayOneShot(sonido, volumen);
        yield return new WaitForSeconds(delay);

    }

    public void CambiarDeNivel(string escena, float delay = 0.5f)
    {
        StartCoroutine(CambiarEscena(escena, delay));
    }

    IEnumerator CambiarEscena(string escena, float delay)
    {

        //esperar el delay antes de ir a la escena
        yield return new WaitForSeconds(delay);
        //cargar la escena
        SceneManager.LoadScene(escena);

    }

}
