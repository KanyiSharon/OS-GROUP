using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.Numerics;
using System.Security.Cryptography;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Rendering;

public class actor_behaviour : MonoBehaviour
{

    public enum state
    { Spawned, waiting, acting, rest, main_act, finished, idle };
    TextMeshPro text;
    public state current;
    Rigidbody2D body;
    string last_move = "";
    public int rest_counter;
    public int caught_counter;
    Collider2D coll;
   UnityEngine.Vector3 origin_size;

    SpriteRenderer rend;
    int stamina;

    UnityEngine.Vector2 spawn_loc;
    // Start is called before the first frame update
    void Start()
    {
        origin_size=transform.lossyScale;
        current = state.Spawned;
        stamina = 3;
        body = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        rend = GetComponent<SpriteRenderer>();
        /*float  r=UnityEngine.Random.Range(0,100);
         float  b=UnityEngine.Random.Range(100,255);
          float  g=UnityEngine.Random.Range(67,205);  
            Debug.Log(r);
          Debug.Log(b);
            Debug.Log(g);
        rend.material.color=new Color(r,b,g,1.0f);
      */
        // text=new TextMeshPro();
        //Instantiate(text, transform.position, quaternion.identity);

    }

    // Update is called once per frame
    void Update()
    {
        actor_update();

    }
    void update_text()
    {

        // text.SetText(current.ToString());
    }
    void actor_update()
    {

        update_text();

        switch (current)
        {
            case state.Spawned:
                current = state.waiting;
                break;
            case state.waiting:
                check_closest();
                break;

            case state.acting:
                path_find();

                break;
            case state.rest:
                rest();
                break;
            case state.main_act:
                caught_counter++;
                current = state.rest;
                break;
            case state.idle:
            erased();
                break;
            case state.finished:

              erase_self();
                break;


        }
    }

    void erase_self()
    {
          
        transform.localScale=new UnityEngine.Vector3(transform.lossyScale.x+(float)0.3,transform.lossyScale.y+(float)0.3,transform.lossyScale.z);
        if(transform.lossyScale.x>=origin_size.x+20)
        {
            Destroy(this.gameObject);
        }
    }
    public void erased()
    {
        GameObject cont=GameObject.FindGameObjectWithTag("GameController");
    
        body.position = new UnityEngine.Vector2(body.position.x , body.position.y+ (float)0.03);
        if(transform.position.y>=cont.transform.position.y)
        {
             Destroy(this.gameObject);
        }

  
    }
    void path_find()
    {

        if (check_closest())
        {
            float Target = find_ball().position.x;

            if (Target != transform.position.x)
            {
                if (transform.position.x > Target)
                {
                    move_right();
                    last_move = "right";
                }
                else
                {
                    move_left();
                }



            }
        }
        else
        {
            current = state.waiting;
        }
    }
    void rest()
    {
        rest_counter++;
        if (last_move == "right")
        {
            move_left();
        }
        else
        {
            move_right();
        }
        if (rest_counter > 600)
        {
            if (caught_counter >= stamina)
            {
                current = state.finished;
            }
            else
            {
                current = state.waiting;
            }
            rest_counter = 0;
        }


    }
    void move_left()
    {
        body.position = new UnityEngine.Vector2(body.position.x + (float)0.03, body.position.y);

    }
    void move_right()
    {
        body.position = new UnityEngine.Vector2(body.position.x - (float)0.03, body.position.y);

    }
    Rigidbody2D find_ball()
    {
        Rigidbody2D target = GameObject.FindGameObjectWithTag("ball").GetComponent<Rigidbody2D>();


        return target;

    }
    float find_dist()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("actor");
        bool negative = false;
        float dist_a;
        float comp_dist = find_ball().position.x;

        if (comp_dist < 0 && transform.position.x > 0 || comp_dist > 0 && transform.position.x < 0)
        {
            negative = true;
        }

        if (negative)
        {
            dist_a = comp_dist + transform.position.x;
        }
        else
        {
            dist_a = comp_dist - transform.position.x;
        }
        if (dist_a < 0)
        {
            dist_a *= -1;
        }


        return dist_a;

    }
    bool check_closest()
    {
        actor_behaviour[] objects = actor_behaviour.FindObjectsByType<actor_behaviour>(0);

        float dist_a = find_dist();


        for (int i = 0; i < objects.Length; i++)
        {

            float dist_b = objects[i].find_dist();

            if (objects[i].caught_counter <= caught_counter)
            {
                if (dist_b < dist_a)
                {
                    return false;
                }
            }



        }

        current = state.acting;

        return true;


    }
    void OnCollisionEnter2D(Collision2D col)
    {
        GameObject obj = col.gameObject;
        if (obj.tag == "ball")
        {
            current = state.main_act;

        }


    }
    public void make_stop()
    {
        current = state.idle;
    }


}


