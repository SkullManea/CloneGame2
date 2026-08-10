using UnityEngine;

public class Pathing : MonoBehaviour
{
    public Transform[] pathingPoints;
    public float speed;
    public int destination;

    void FixedUpdate()
    {
        PathingControl();
    }
    public void PathingControl()
    {
        Transform targetPoint = pathingPoints[destination];
        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);

         if (Vector2.Distance(transform.position, targetPoint.position) < .2f)
        {
            destination++;
        }
    }
    
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("EndPoint"))
        {
            Destroy(gameObject); 
            //insert damage to the game system: other.gameObject.GetComponent<gameHealth>()health -= damage;
        }
       
    }
 
}
