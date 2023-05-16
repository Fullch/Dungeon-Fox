using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Habitacion : MonoBehaviour
{
    [SerializeField]
    GameObject go;
    [SerializeField]
    Material blueMaterial;

    public RectInt bounds;
    public Vector2 centro;
    public string name = "";

    public Habitacion(Vector2Int loc, Vector2Int tam)
    {
        bounds = new RectInt(loc, tam);
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        Habitacion other = (Habitacion)obj;
        return name == other.name;
    }

    public override int GetHashCode()
    {
        return name.GetHashCode() ^ name.GetHashCode();
    }

    public void setGameObject(GameObject go)
    {
        this.go = go;
    }

    public void setName()
    {
        this.name = go.name;
    }

    public GameObject getGameObject()
    {
        return go;
    }

    public Vector2 obtenerVertice()
    {
        if(go != null) {
            Vector2 centro;
            centro = new Vector2(go.transform.position.x / 2, go.transform.position.y / 2);
            return centro;
        }
        else return new Vector2(0, 0);
    }

}
