using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using MyBox;

public class SceneCheck : MonoBehaviour
{
    Button button;
    [Scene] [SerializeField] string toLoad;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(Load);

        void Load()
        {
            SceneManager.LoadScene(toLoad);
            AudioManager.instance.Menu();
        }
    }
}
