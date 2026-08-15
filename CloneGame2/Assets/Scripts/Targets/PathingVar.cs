using UnityEngine;

public class PathingVar : MonoBehaviour
{
    public static PathingVar main;

    public Transform[] pathingPoints;

    private void Awake()
    {
        main = this;
    }
}