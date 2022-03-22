using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
    public float speed = 200f;
    public float nextWaypointDistance = 3f;
    private Transform target;
    public bool canFly;

    public Transform enemyGFX;

    Path path;
    int currentWaypoint = 0;
    bool reachedEndOfPath = false;

    Seeker seeker;
    Rigidbody2D rb;


    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        InvokeRepeating("UpdatePath", 0f, 0.5f);
    }

    void UpdatePath () {
        if (seeker.IsDone()) {
            // current position, ending position, and the method to call:
            seeker.StartPath(rb.position, target.position, OnPathComplete);

        }
    }

    void OnPathComplete (Path p) {
        if (!p.error) {
            path = p;
            currentWaypoint = 0;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (path == null) {
            return;
        }

        if (currentWaypoint >= path.vectorPath.Count) {
            reachedEndOfPath = true;
            return;
        } else {
            reachedEndOfPath = false;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;  // force that will move the enemy in the desired direction

        Vector2 velocity = rb.velocity;

        if (canFly) {
            // If a flyer, apply velocity in all directions
            velocity = force;
            rb.velocity = velocity;
        } else {
            // If not a flyer, only apply velocity to the x axis
            velocity.x = force.x;
            rb.velocity = velocity;
        }

        // This part doesn't work yet :(
        void OnTriggerEnter2D(Collider2D other) {
            if (other.CompareTag("Hill") && !canFly) {
                velocity.y = force.y;
                rb.velocity = velocity;
            }
        }

        void OnTriggerExit2D(Collider2D other) {
            if (other.CompareTag("Hill") && !canFly) {
                velocity.y = 0;
                rb.velocity = velocity;
            }
        }


        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance) {
            currentWaypoint ++;
        }

        if (rb.velocity.x >= 0.1f) { // moving right
            enemyGFX.rotation = Quaternion.Euler(0,0,0);
        } else if (rb.velocity.x <= -0.1f) { // moving left
            enemyGFX.rotation = Quaternion.Euler(0,180,0);
        }
    }
}