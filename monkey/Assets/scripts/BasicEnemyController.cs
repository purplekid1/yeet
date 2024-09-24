using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BasicEnemyController : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform target;
    public PlayerController pc;

    [Header("Enemy Stats")]
    public int health = 3;
    public int maxHealth = 5;
    public int damageGiven = 1;
    public int damageReceived = 1;
    public float pushBackForce = 7f;


    void Start()
    {
        pc = GameObject.Find("player").GetComponent<PlayerController>();
        agent = GetComponent<NavMeshAgent>();

    }

    
    void Update()
    {

        target = GameObject.Find("player").transform;
        agent.destination = target.position;

        if (health <= 0 )
        {
            Destroy(gameObject);
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "bullet")
        {
            health -= damageReceived;
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.tag == "player" && !pc.takenDamage)
        {
            pc.health -= damageGiven;
            pc.GetComponent<Rigidbody>().AddForce(transform.forward * pushBackForce);
            pc.takenDamage = true;
            pc.StartCoroutine("coolDownDamage");

        }    
 
    }


}
