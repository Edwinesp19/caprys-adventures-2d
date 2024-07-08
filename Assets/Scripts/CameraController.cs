using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //Referencia a nuestro jugador
    public GameObject jugador;

    //para registrar la diferencia entre la posición de la cámara y la del jugador
    private Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        //diferencia entre la posición de la cámara y la del jugador
        offset = transform.position - jugador.transform.position;
    }

    // Se ejecuta cada frame, pero después de haber procesado todo. Es más exacto para la cámara

    void LateUpdate()
    {
        //Actualizo la posición de la cámara
        transform.position = jugador.transform.position + offset;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
