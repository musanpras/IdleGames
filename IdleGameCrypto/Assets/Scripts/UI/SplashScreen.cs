using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SplashScreen : MonoBehaviour
{
    public float _duration;

    private bool _complete;

    public float _timer;

    public Slider _progressBar;

    // Start is called before the first frame update
    void Start()
    {
        _complete = false;
        _timer = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if(!_complete && _timer < _duration)
        {
            _timer += Time.deltaTime;
            _progressBar.value = _timer/_duration;
            //UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
        }
        else
        {
            _complete = true;
            UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
        }
    }
}
