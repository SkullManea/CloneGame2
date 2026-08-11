using UnityEngine;

public class Pathing : MonoBehaviour
{
    //[SerializeField] private Rigidbody2D rb;
    private Transform target;
    public float speed;
    public int destination;

    void Start()
    {
        target = PathingVar.main.pathingPoints[0];
    }
    void FixedUpdate()
    {
        PathingControl();
    }
    public void PathingControl()
    {
        transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

         if (Vector2.Distance(transform.position, target.position) < .1f)
        {
            destination++;

            if (destination == PathingVar.main.pathingPoints.Length)
            {
                Destroy (gameObject);
                return;
            }
            else
            {
                target = PathingVar.main.pathingPoints[destination];
            }
        }
    }
    
   
 
}
