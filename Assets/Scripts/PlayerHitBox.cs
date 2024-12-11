using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHitBox : MonoBehaviour
{
    [SerializeField] private GameObject player;
    public GameObject losePanel;
    public GameObject GameManager;

    private void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject.tag == "BossAttackHitBox")
        {   
            losePanel.gameObject.SetActive(true);
            Time.timeScale = 0f;
            GameManager.GetComponent<UIManager>().gamePaused = true;
        }
    }
}
