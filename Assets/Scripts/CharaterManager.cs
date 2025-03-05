using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharaterManager : MonoBehaviour
{
    private static CharaterManager _instance;
    public static CharaterManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance =new GameObject("CharaterManager").AddComponent<CharaterManager>();
            }
            return _instance;
        }
    }

    public Player _player;
    public Player Player
    {
        get { return _player; }
        set { _player = value; }
    }

    private void Awake()
    {
        if (_instance ==null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (_instance == this)
            { 
                Destroy(gameObject);
            }
        }
    }
}
