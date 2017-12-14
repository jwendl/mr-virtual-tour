using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using UnityEngine.SceneManagement;

public class SceneNavigator : MonoBehaviour, IInputClickHandler
{
    public string DestinationScene;

    public void OnInputClicked(InputClickedEventData eventData)
    {
        SceneManager.LoadScene(DestinationScene, LoadSceneMode.Single);
    }
}
