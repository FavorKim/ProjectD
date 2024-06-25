using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedPerkList : MonoBehaviour
{
    [SerializeField]List<SelectedPerkView> selectedList = new List<SelectedPerkView>();


    public void ResetMarker()
    {
        foreach(SelectedPerkView p in selectedList)
        {
            p.SetMarkerActive(false);
        }
    }
}
