using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class spawner_behaviour : MonoBehaviour
{
    [SerializeField]
    GameObject actor_p;
    public float speed;
    SceneManager man;
    actor_behaviour behave;
    Rigidbody2D body;
    Collider2D col;
    [SerializeField]
    public int spawn_delay = 250;
    bool start_counter = false;
    public float counter;
    float vector;
    int obj_counter;
    Vector2 spawn_loc;
    // Start is called before the first frame updatez
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        spawn_loc = GameObject.FindGameObjectWithTag("sp_loc").GetComponent<Rigidbody2D>().position;
    }

    // Update is called once per frame
    void Update()
    {
        move();
  controller();
        spawn();


    }

    void move()
    {
        float velocity = Input.GetAxis("Horizontal");
        body.position = new Vector2(body.position.x + (speed * velocity), body.position.y);



    }
    void OnCollisionEnter2D(Collision2D col)
    {
        string tag = col.gameObject.tag;
    }
    void spawn()
    {
        if (Input.GetKeyDown(KeyCode.S) && !start_counter)

        {
            float r = UnityEngine.Random.Range(0, 100);
            float b = UnityEngine.Random.Range(100, 255);
            float g = UnityEngine.Random.Range(67, 205);
            actor_p.GetComponent<SpriteRenderer>().sharedMaterial.color = new Color(r, b, g, 1.0f);

            Instantiate(actor_p, spawn_loc, Quaternion.identity);
            start_counter = true;
        }
        if (start_counter)
        {
            this.counter += (float)0.5;
            if (counter >= spawn_delay)
            {
                start_counter = false;
                counter = 0;
            }
        }


    }
    void controller()
    {
        actor_behaviour[] objects = actor_behaviour.FindObjectsByType<actor_behaviour>(0);
        if (Input.GetKeyDown(KeyCode.W))
        {
            for (int i = 0; i < objects.Length; i++)
            {

                if (objects[i].transform.position.x >= transform.position.x - 2 && objects[i].transform.position.x <= transform.position.x + 2)
                {
                    objects[i].make_stop();
                }


            }
        }

    }
}
