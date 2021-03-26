using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalVariables : MonoBehaviour
{

    public float Gravity = 9.8f;

    private PlayerControls playerControls;
    public LayerMask groundLayerMask;

    // Start is called before the first frame update

    private void Awake()
    {
        playerControls = new PlayerControls();
    }
    void Start()
    {
        
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    // Update is called once per frame
    void Update()
    {
 //       Debug.Log(playerControls.InGameControls.BasicMovement.ReadValue<float>());
    }
}
