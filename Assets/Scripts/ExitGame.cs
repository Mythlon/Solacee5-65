using UnityEngine;

public class ExitGameOnESC : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
            Debug.Log("Game Quit"); // Only works in Editor for testing
        }
    }
}
