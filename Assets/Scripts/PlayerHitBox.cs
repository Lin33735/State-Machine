using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHitBox : MonoBehaviour
{
    [SerializeField] private GameObject player;

    private void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject.tag == "BossAttackHitBox")
        {   
            player.gameObject.SetActive(false);
            SceneManager.LoadScene("MainGame");
        }
    }
}
