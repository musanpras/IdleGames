using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Control : MonoBehaviour
{
   // public Transform _reference;

    public static Control _instance;

    public Transform _reference_level2, _reference_level1;

    public Dictionary<string, Transform> generatorControllers = new Dictionary<string, Transform>();

    private void Awake()
    {
        _instance = this;
        generatorControllers.Add("cube", _reference_level1);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Upgrade();
    }

    public void Upgrade()
    {
        
        Transform lv1Obj = generatorControllers["cube"];
        generatorControllers.Remove("cube");
        Destroy(lv1Obj.gameObject);
        generatorControllers.Add("cube", _reference_level2);
    }
}
