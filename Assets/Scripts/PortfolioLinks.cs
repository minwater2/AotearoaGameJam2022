using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PortfolioLinks : MonoBehaviour
{
    public void Click(string link)
    {
        Application.OpenURL(link);
    }
}
