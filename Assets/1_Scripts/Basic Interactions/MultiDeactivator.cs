using UnityEngine;
using System.Collections.Generic;

public class MultiDeactivator : MonoBehaviour
{
    [Header("All Objects")]
    [SerializeField] private List<GameObject> allObj = new List<GameObject>();

    public void DeactivateAllExcept(int exceptIndex)
    {
        for (int i = 0; i < allObj.Count; i++)
        {
            if (i != exceptIndex) allObj[i].SetActive(false);
            else allObj[i].SetActive(true);
        }
    }
}
