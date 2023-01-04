using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateBoundObject : MonoBehaviour
{
    [Header("State Enabled")]
    public string State;

    [Header("State List (runtime to view)")]
    public List<string> States = new List<string>();

    [Header("State Triggers")]
    public List<GameObject> EnableList = new List<GameObject>();
    public List<GameObject> DisableList = new List<GameObject>();

    public List<GameObject> EnableLayerList = new List<GameObject>();
    public List<GameObject> DisableLayerList = new List<GameObject>();

    private bool StatePassed = false;

    [Header("Self Settings")]
    public bool DestroyOnPassState = true;
    public bool ExistBeforeState = false;
    
    private int StateNum;

    private void Start()
    {
        var i = 0;
        foreach(string s in StateController.instance.States)
        {
            States.Add(s);
            if (s == State) StateNum = i;
            else i++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(StateController.instance.CurrentState != State)
        {
            if (DestroyOnPassState)
            {
                if(ExistBeforeState)
                {
                    if(StateController.instance.currentStateNum > StateNum)
                        this.gameObject.SetActive(false);

                }
                else this.gameObject.SetActive(false);
            }

            if (StateController.instance.currentStateNum > StateNum)
            {
                if(!StatePassed)
                {
                    foreach (GameObject gameObject in EnableList)
                        gameObject.SetActive(true);
                    
                    foreach (GameObject gameObject in DisableList)
                        gameObject.SetActive(false);

                    foreach (GameObject gameObjectLayer in EnableLayerList)
                        gameObjectLayer.layer = LayerMask.NameToLayer("Selectable");

                    foreach (GameObject gameObjectLayer in DisableLayerList)
                        gameObjectLayer.layer = LayerMask.NameToLayer("Default");

                    StatePassed = true;
                }
            }
        }
        else
        {
            if(DestroyOnPassState)
                this.gameObject.SetActive(true);
        }

    }
}
