using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class StateController : MonoBehaviour
{
    [Header("State List")]
    public string[] States;

    [Header("State Settings")]
    public string CurrentState;
    public int currentStateNum = 0;
    public int maxStates = 0;
    public bool StartNextState;

    public static StateController instance;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        CurrentState = States[currentStateNum];

        foreach (string s in States)
        {
            maxStates++;
        }
    }

    private void Update()
    {
        if(StartNextState)
        {
            if(currentStateNum != maxStates)
            {
                StartNextState = !StartNextState;
                currentStateNum++;
                FireState();
            }
        }
    }

    private void FireState()
    {
        CurrentState = States[currentStateNum];
    }

}
