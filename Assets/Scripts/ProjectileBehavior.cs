using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{
    [SerializeField] private GameObject player;
    private Vector3 destination;
    private float speed = 250;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("PlayerHitBox");
    }

    private void Start()
    {
        destination = player.transform.position;
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);

        if (this.transform.position == destination)
        {
            StartCoroutine(Disable());
        }
    }

    private IEnumerator Disable()
    {
        yield return new WaitForSeconds(0.05f);
        gameObject.SetActive(false);
    }
}
