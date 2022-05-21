using System;
using UnityEngine;

public class ActionController : MonoBehaviour
{
    private IActionPerformer _actionPerformer;

    private void Start()
    {
        _actionPerformer = GetComponentInChildren<IActionPerformer>();
    }

    public void PerformAction()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _actionPerformer.PerformAction();
        }
    }
}