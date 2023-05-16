using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using UnityEditor.PackageManager;
using UnityEngine;
using static UnityEditor.Progress;
using Random = System.Random;

public class prueba : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        generar();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            
        }
    }

    /// Atributos y clases ///
    //////////////////////////////////////////////////////////////

    [SerializeField]
    int numHabitaciones;
    [SerializeField]
    GameObject cuboPrefab;
    [SerializeField]
    Material blueMaterial;
    [SerializeField]
    Vector2Int tamMax = new Vector2Int(10, 4);
    [SerializeField]
    int radio = 10;

    int tiempoM;
    Vector2Int tam;
    Vector2Int loc;
    Vector2Int tamTotal;
    Random r;
    Habitacion room;
    List<Habitacion> habitaciones;
    List<GameObject> mains = new List<GameObject>();

    void generar()
    {
        habitaciones = new List<Habitacion>();

        crearHabitaciones();
        spawnHabitaciones(habitaciones);
        elegirMain();
        destroyNonMain();
        
        foreach(GameObject go in mains)
        {
            comprobarColisiones(go);
        }

        //List<Vector2> vertices = prim.obtenerVertices();
        //List<double> distancias = prim.calcularDistancia(vertices);

        //Array centros = arrCentros(mains);
        //for(int i = 0; i < centros.Length; i++)
        //{
        //    Debug.Log(centros.GetValue(i));
        //}
    }

    void crearHabitaciones()
    {
        for(int i = 0; i < numHabitaciones; i++)
        {
            tiempoM = (DateTime.UtcNow.Millisecond) * i;
            r = new Random(tiempoM);
            tam = new Vector2Int(r.Next(1, tamMax.x), r.Next(1, tamMax.y));
            loc = distribuirHabitacion(i);
            room = new Habitacion(loc, tam);
            tamTotal += tam;
            habitaciones.Add(room);
        }
    }


    void spawnHabitaciones(List<Habitacion> habitaciones)
    {
        foreach(Habitacion habitacion in habitaciones)
        {
            GameObject go = Instantiate(cuboPrefab, new Vector3(habitacion.bounds.x, habitacion.bounds.y, 0), Quaternion.identity);
            go.GetComponent<Transform>().localScale = new Vector3(habitacion.bounds.size.x, habitacion.bounds.size.y, 1);
            go.GetComponent<Renderer>().material = blueMaterial;
        }
    }

    Vector2Int distribuirHabitacion(int i)
    {
        //tiempoM = (DateTime.UtcNow.Millisecond) * i;
        r = new Random(i);

        float angulo = ((float)r.NextDouble() * (1 - 0) + 0) * Mathf.PI * 2f;

        float rad = Mathf.Sqrt((float)r.NextDouble() * (1-0) + 0) * radio;
        float x = this.transform.localPosition.x + rad * Mathf.Cos(angulo);
        float y = this.transform.localPosition.y + rad * Mathf.Sin(angulo);

        return new Vector2Int((int)x, (int)y);
    }
    
    void elegirMain()
    {
        float avgX = tamTotal.x / numHabitaciones;
        float avgY = tamTotal.y / numHabitaciones;

        GameObject go;
        
        bool valido = false;

        for (int i = 0; i < numHabitaciones; i++)
        {
            Debug.Log("habitacion: " + i);
            go = GameObject.Find("Cubo(Clone)");
            
            if(go.transform.localScale.x >= avgX * 1.25f && go.transform.localScale.y >= avgY * 1.25f)
            {
                Debug.Log("Comprobamos " + i);

                GameObject[] mainGo = GameObject.FindGameObjectsWithTag("Main");

                Debug.Log("Numero de mains " + mainGo.Length);

                if(mainGo.Length > 0)
                {
                    Debug.Log("hay main");

                    valido = true;
                }
                else
                {
                    valido = true;
                    Debug.LogError("No existen Mains");
                }

                if (valido)
                {
                    Debug.Log("creamos main");
                    go.tag = "Main";
                    go.name = "Habitacion" + i;
                    mains.Add(go);
                    //valido = false;
                }
                else
                {
                    //valido = true;
                    go.tag = "Inutil";
                    go.name = "Habitacion" + i + " no valida";
                    Debug.LogError("Habitacion no valida");
                }
            }
            else
            {
                go.tag = "Inutil";
                go.name = "Habitacion" + i + " no valida";
                Debug.LogError("No cumple los requisitos");
            }
        }
    }

    void comprobarColisiones(GameObject go)
    {
        GameObject[] mainGo = GameObject.FindGameObjectsWithTag("Main");

        foreach (GameObject aux in mainGo)
        {

            if (aux == go) continue;

            Debug.Log("Comparamos " + go.name + " con " + aux.name);
            //Debug.Log(go.transform.position.x + ", (" + aux.transform.position.x + ", " + (aux.transform.position.x + aux.transform.localScale.x) + ")");
            //Debug.Log(go.transform.position.y + ", (" + aux.transform.position.y + ", " + (aux.transform.position.y + aux.transform.localScale.y) + ")");

            if (go.transform.position.x >= aux.transform.position.x && 
                go.transform.position.x <= aux.transform.position.x + aux.transform.localScale.x &&
                go.transform.position.y >= aux.transform.position.y &&
                go.transform.position.y <= aux.transform.position.y + aux.transform.localScale.y)
            {
                Debug.Log("colision de " + go.name + " con " + aux.name);

                if(go.transform.localScale.x <= aux.transform.localScale.x || go.transform.localScale.y <= aux.transform.localScale.y)
                {
                    Debug.Log("Eliminamos " + go.name);
                    Destroy(go);
                }
                else
                {
                    Debug.Log("Eliminamos " + aux.name);
                    Destroy(aux);
                }
            }
        }
    }

    void destroyNonMain()
    {
        GameObject[] go = GameObject.FindGameObjectsWithTag("Inutil");

        foreach (GameObject goAux in go)
        {
            Destroy(goAux);
        }
    }

}
