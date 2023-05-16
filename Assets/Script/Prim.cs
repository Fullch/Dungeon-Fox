using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct Tupla
{
    public Habitacion h1;
    public Habitacion h2;
    public Tupla(Habitacion h1, Habitacion h2) 
    { 
        this.h1 = h1; 
        this.h2 = h2; 
    }

    public override bool Equals(object obj)
    {
        if (!(obj is Tupla))
        {
            return false;
        }

        var other = (Tupla)obj;
        return h1 == other.h1 && h2 == other.h2;
    }

    public override int GetHashCode()
    {
        return h1.GetHashCode() ^ h2.GetHashCode();
    }
}
public abstract class Prim : MonoBehaviour
{
    [SerializeField]
    int maxDist = 100;


    public int calcularDistancia(Habitacion h1, Habitacion h2)
    {
        Vector2 v1 = h1.obtenerVertice();
        Vector2 v2 = h2.obtenerVertice();
        int arista;

        //d = sqr((x2 * x1)^2 + (y2 * y1)^2)
        if(h1.getGameObject() == null || h2.getGameObject() == null) arista = int.MaxValue;
        else arista = (int)Vector2.Distance(h1.getGameObject().transform.position, h2.getGameObject().transform.position);

        return arista;
    }

    public void crearGrafo(List<Habitacion> mains, List<KeyValuePair<Tupla, int>> grafo)
    {
        //{ h1-h1, h1-h2, h1-h3, h1-h4, ...
        //  h2-h1, h2-h2, h2-h3, h2-h4, ...
        //  h3-h1, h2-h3, h3-h3, h3-h4, ...}

        foreach(Habitacion h1 in mains)
        {

            foreach(Habitacion h2 in mains)
            {
                // Debug.Log(h1.name + ", " + h2.name);
                Tupla tupla = new Tupla(h1, h2);
                int dist = calcularDistancia(h1 , h2);

                // if(dist > maxDist) dist = 0;

                grafo.Add(new KeyValuePair<Tupla, int>(tupla, dist));

                // Debug.Log(tupla.h1.name + ", " + tupla.h2.name + ": " + grafo[tupla]);
            }
        }

        // foreach(KeyValuePair<Tupla, int> vp in grafodois){

        //     Debug.Log(vp.Key.h1.name + ", " + vp.Key.h2.name + ": " + vp.Value);
        // }
    }

    public void printGrafo(List<KeyValuePair<Tupla, int>> grafo)
    {
        // Debug.Log(grafo.Count);
        foreach(KeyValuePair<Tupla, int> kvp in grafo)
        {
            Debug.Log("Key = " + kvp.Key.h1.name + ", " + kvp.Key.h2.name + " Value = " + kvp.Value);
        }
    }

    Tupla buscarMinimo(List<KeyValuePair<Tupla, int>> grafo, HashSet<Tupla> mst, int[,] clave, int numVert, List<Habitacion> habitaciones) {

        Tupla indiceMin = new Tupla(habitaciones[0], habitaciones[0]);
        int distMin = int.MaxValue;
        foreach (KeyValuePair<Tupla, int> kvp in grafo) {
            Tupla tupla = kvp.Key;
            int i = habitaciones.IndexOf(tupla.h1);
            int j = habitaciones.IndexOf(tupla.h2);
            Debug.Log("Clave " + clave[i, j]);
            if (clave[i, j] < distMin && !mst.Contains(tupla)) {
                distMin = clave[i, j];
                indiceMin = tupla;
            }
        }

        return indiceMin;

        // Tupla indiceMin = new Tupla(habitaciones[0], habitaciones[0]);
        // int distMin = int.MaxValue;

        // for (int i = 0; i < numVert; i++) {
        //     for (int j = 0; j < numVert; j++) {
        //         if (boolGrafo[i, j] == false && clave[i, j] < distMin) {
        //             distMin = clave[i, j];
        //             indiceMin = new Tupla(habitaciones[i], habitaciones[j]);
        //             Debug.Log(indiceMin.h1.name + " " + indiceMin.h2.name);
        //         }
        //     }
        // }
        
        // return indiceMin;
    }

    private int[] buscarIndice(List<KeyValuePair<Tupla, int>> grafo, Tupla tupla, List<Habitacion> habitaciones) {

        foreach(KeyValuePair<Tupla, int> kvp in grafo) {

            // Debug.Log(tupla.h1.name + " " + kvp.Key.h1.name + " " + tupla.h2.name + " " + kvp.Key.h2.name);

            if(tupla.h1.Equals(kvp.Key.h1) && tupla.h2.Equals(kvp.Key.h2)) {
                int indice1 = habitaciones.FindIndex(h => h.Equals(tupla.h1));
                int indice2 = habitaciones.FindIndex(h => h.Equals(tupla.h2));
                return new int[] { indice1, indice2 }; 
            }
        }

        return null;
    }

    public void algoPrim(List<KeyValuePair<Tupla, int>> grafo, int numVert, List<Habitacion> habitaciones)
    {
        int[,] parent = new int[numVert, numVert];
        int[,] clave = new int[numVert, numVert];
        HashSet<Tupla> mst = new HashSet<Tupla>();

        for (int i = 0; i < numVert; ++i)
        {
            for (int j = 0; j < numVert; j++){

                clave[i, j] = int.MaxValue;
            }
        }

        clave[0, 0] = 0;
        parent[0, 0] = -1;

        for(int i = 1; i < numVert; ++i)
        {
            Tupla minimo = buscarMinimo(grafo, mst, clave, numVert, habitaciones);
            int[] indices = buscarIndice(grafo, minimo, habitaciones);

            Debug.Log(indices[0] + " " + indices[1]);
            mst.Add(minimo);
            
            for (int v = 0; v < numVert; ++v)
            {
                if(grafo.Exists(x => x.Key.h1 == habitaciones[indices[0]] && x.Key.h2 == habitaciones[v]))
                {
                    int dist = grafo.Find(x => x.Key.h1 == habitaciones[indices[0]] && x.Key.h2 == habitaciones[indices[1]]).Value;
                    if(dist < clave[indices[0], v])
                    {
                        Debug.Log("Distancia: " + dist);
                        parent[indices[0], v] = dist;
                        clave[indices[0], v] = indices[1];
                    }
                }
            }
        }

        printMST(parent, grafo, numVert);
    }


    private static void printMST(int[,] parent, List<KeyValuePair<Tupla, int>> grafo, int numVert)
    {
        Debug.Log("Edge     Weight");
        for (int i = 1; i < numVert; ++i)
            Debug.Log(grafo[i].Key.h1.name + ", " + grafo[i].Key.h2.name + ": " + grafo[i].Value);
    }
}
