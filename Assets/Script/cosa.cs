using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using UnityEditor.PackageManager;
using UnityEngine;
using static UnityEditor.Progress;
using Random = System.Random;

public class cosa : PrimAlgorithm
{
    // Start is called before the first frame update
    void Start()
    {
        generar();
    }

    // Update is called once per frame
    bool parar = false;
    void Update()
    {


        if (Input.GetMouseButtonUp(0))
        {
        }

        if(Input.GetMouseButtonUp(1))
        {
            comprobarColisiones2();
            List<(Habitacion, Habitacion)> mst = Prim(mains);
            //Dictionary<Habitacion, (Vector2, Vector2)> salidas = asignarSalidas(mst);
            //generarPasillos(salidas);
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
    [SerializeField]
    private TilemapVisualizer tilemapVisualizer;
    [SerializeField]
    int minRooms = 5;

    int tiempoM;
    Vector2Int tam;
    Vector2Int loc;
    Vector2Int tamTotal;
    Random r;
    Habitacion room;
    List<Habitacion> habitaciones;
    List<Habitacion> mainsTemp = new List<Habitacion>();
    List<Habitacion> mains = new List<Habitacion>();
    List<KeyValuePair<Tupla, int>> grafo = new List<KeyValuePair<Tupla, int>>();

    void generar()
    {
        habitaciones = new List<Habitacion>();

        crearHabitaciones();
        spawnHabitaciones(habitaciones);
        elegirMain();   
        destroyNonMain();
        foreach (Habitacion go in mainsTemp)
        {
            go.setName();
            // Debug.Log(go.name);
        }


        // Debug.Log(mains.Count);
        //Recorrido inverso para poder eliminar elementos sin dejar de iterar
        // foreach (Habitacion go in mains.Reverse<Habitacion>())
        // {
        //     comprobarColisiones(go);
        // }

        // Debug.Log(mains.Count);
        // masComprobaciones();

        // Debug.Log(mains.Count);
    }

    void crearHabitaciones()
    {
        for (int i = 0; i < numHabitaciones; i++)
        {
            tiempoM = (DateTime.UtcNow.Millisecond) * i;
            r = new Random(tiempoM);
            tam = new Vector2Int(r.Next(64, tamMax.x), r.Next(64, tamMax.y)); //Decimal.ToInt32(Math.Round((decimal)(r.Next(64, tamMax.x)/16)*16)), Decimal.ToInt32(Math.Round((decimal)(r.Next(64, tamMax.y) / 16) * 16))
            loc = distribuirHabitacion(i);
            room = new Habitacion(loc, tam);
            tamTotal += tam;
            habitaciones.Add(room);
        }
    }


    void spawnHabitaciones(List<Habitacion> habitaciones)
    {
        foreach (Habitacion habitacion in habitaciones)
        {
            GameObject go = Instantiate(cuboPrefab, new Vector3(habitacion.bounds.x, habitacion.bounds.y, 0), Quaternion.identity);
            go.GetComponent<Transform>().localScale = new Vector3(habitacion.bounds.size.x, habitacion.bounds.size.y, 1);
            go.GetComponent<Renderer>().material = blueMaterial;
            habitacion.setGameObject(go);
        }
    }

    Vector2Int distribuirHabitacion(int i)
    {
        //tiempoM = (DateTime.UtcNow.Millisecond) * i;
        r = new Random(i);

        float angulo = ((float)r.NextDouble() * (1 - 0) + 0) * Mathf.PI * 2f;

        float rad = Mathf.Sqrt((float)r.NextDouble() * (1 - 0) + 0) * radio;
        float x = this.transform.localPosition.x + rad * Mathf.Cos(angulo);
        float y = this.transform.localPosition.y + rad * Mathf.Sin(angulo);

        return new Vector2Int((int)x, (int)y);
    }

    void elegirMain()
    {
        float avgX = tamTotal.x / numHabitaciones;
        float avgY = tamTotal.y / numHabitaciones;

        bool valido = false;

        for (int i = 0; i < numHabitaciones; i++)
        {
            // Debug.Log("habitacion: " + i);
            Habitacion hab = habitaciones[i];
            GameObject go = hab.getGameObject();

            if (go.transform.localScale.x % 16 == 0 && go.transform.localScale.y % 16 == 0)
            {
                // Debug.Log("Comprobamos " + i);

                GameObject[] mainGo = GameObject.FindGameObjectsWithTag("Main");

                // Debug.Log("Numero de mains " + mainGo.Length);
                // Debug.Log("creamos main");
                go.tag = "Main";
                go.name = "Habitacion" + i;
                hab.name = "Habitacion" + i;
                mainsTemp.Add(hab);
                //valido = false;
            }
            else
            {
                go.tag = "Inutil";
                go.name = "Habitacion" + i + " no valida";
                // Debug.LogError("No cumple los requisitos");
            }
        }
    }

    void comprobarColisiones2()
    {
        //guardarme las que elimino y luego borrarlas
        List<String> toRemove = new List<String>();

        for(int i = 0; i < mainsTemp.Count; i++)
        {
            for(int j = i + 1; j < mainsTemp.Count; j++)
            {
                Collider2D colliderA = mainsTemp[i].getGameObject().GetComponent<Collider2D>();
                Collider2D colliderB = mainsTemp[j].getGameObject().GetComponent<Collider2D>();

                if (colliderA.bounds.Intersects(colliderB.bounds))
                {
                    Debug.Log(mainsTemp[i].name + " colisiona con " + mainsTemp[j].name);

                    if (mainsTemp[i].bounds.y < mainsTemp[j].bounds.y)
                    {
                        Debug.Log("Eliminamos " + mainsTemp[i].name);
                        Destroy(mainsTemp[i].getGameObject());
                        toRemove.Add(mainsTemp[i].name);
                    } else
                    {
                        Debug.Log("Eliminamos " + mainsTemp[j].name);
                        Destroy(mainsTemp[j].getGameObject());
                        toRemove.Add(mainsTemp[j].name);
                    }
                }   
            }
        }

        rellenarMains(toRemove);
    }

    void destroyNonMain()
    {
        GameObject[] go = GameObject.FindGameObjectsWithTag("Inutil");

        foreach (GameObject goAux in go)
        {
            Destroy(goAux);
        }
    }

    void rellenarMains(List<String> toRemove)
    {
        HashSet<Vector2> posiciones = new HashSet<Vector2>();
        foreach(Habitacion h in mainsTemp){
            
            Debug.Log(h.name);
            if(!toRemove.Contains(h.name)) {
                Debug.Log("Anadimos a la nueva " + h.name);
                mains.Add(h);
               
                

            }
            
        }
        
        //tilemapVisualizer.paintFloorTiles(posiciones);  
    }

    void comprobarNumeroMains()
    {
        if (mains.Count() < minRooms)
        {
            generar();
        }
    }

    //puede que lo necesite meter dentro del generador de pasillos
    Dictionary<(Habitacion, Habitacion), (Vector2, Vector2)> asignarSalidas(List<(Habitacion, Habitacion)> mst)
    {
        Dictionary<(Habitacion, Habitacion), (Vector2, Vector2)> salidas = new Dictionary<(Habitacion, Habitacion), (Vector2, Vector2)>();
        foreach(Habitacion h in mains)
        {
            int i = 0;
            List<(Habitacion, Habitacion)> nodo = mst.FindAll(x => x.Item1.Equals(h));
            String[] caras = new string[nodo.Count];

            foreach ((Habitacion, Habitacion) n in nodo)
            {
                Debug.Log(i + " " + n.Item1.name + " " + n.Item2.name);
                caras[i] = buscarCara(n);
                Debug.Log(caras[i]);
                i++;
                salidas.Add((h, n.Item2), (new Vector2(0, 0), new Vector2(0, 0)));
            }

        }

        

        return salidas;
    }

    String buscarCara((Habitacion, Habitacion) nodo)
    {
        String cara = "";

        //comprobar si esta mas de la mitad del centro
        if(nodo.Item2.bounds.center.x > nodo.Item1.bounds.center.x)
        {
            cara += "derecha";
        }
        if (nodo.Item2.bounds.center.x < nodo.Item1.bounds.center.x)
        {
            cara += "izquierda";
        }
        if (nodo.Item2.bounds.center.y > nodo.Item1.bounds.center.y)
        {
            cara += "arriba";
        }
        if (nodo.Item2.bounds.center.y < nodo.Item1.bounds.center.y)
        {
            cara += "abajo";
        }

        return cara;
    }


    Vector2 crearPuntoApertura(String cara, Habitacion habitacion)
    {
        Vector2 apertura = new Vector2();

        switch (cara)
        {
            case "arriba":
                apertura = new Vector2(habitacion.centro.x, (habitacion.centro.y + habitacion.centro.y/2));
                break;

            case "abajo":
                apertura = new Vector2(habitacion.centro.x, (habitacion.centro.y - habitacion.centro.y / 2));
                break;

            case "izquierda":
                apertura = new Vector2((habitacion.centro.x - habitacion.centro.x/2), habitacion.centro.y);
                break;

            case "derecha":
                apertura = new Vector2((habitacion.centro.x + habitacion.centro.x / 2), habitacion.centro.y);
                break;

            case "derechaarriba":
                //apertura = new 
                break;

            case "derechaabajo":
                break;

            case "izquierdaarriba":
                break;

            case "izquierdaabajo":
                break;

        }  

        return apertura;
    }

    private void generarPasillos(Dictionary<Habitacion, (Vector2, Vector2)> salidas)
    {
        
    }
}
