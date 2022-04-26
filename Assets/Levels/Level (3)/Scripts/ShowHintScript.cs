using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowHintScript : MonoBehaviour
{
    [SerializeField] private GameObject _tutorialGhost;
   // [SerializeField] private Text _tutorialText;

   
    public void ShowHint()
    {
        _tutorialGhost.SetActive(true);
       // _tutorialText.gameObject.SetActive(true);

    }

    public void EndHint()
    {
        _tutorialGhost.SetActive(false);
//_tutorialText.gameObject.SetActive(false);

    }
}
