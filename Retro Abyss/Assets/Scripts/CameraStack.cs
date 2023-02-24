﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;


public class CameraStack : MonoBehaviour
{
    private Camera _camera;
    private Camera uiCamera;
    private Camera postProcessingCamera;
    
    // Start is called before the first frame update
    void Start()
    {
        _camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        var cameraData = _camera.GetUniversalAdditionalCameraData();
        if (uiCamera == null)
        {
            if(GameObject.FindGameObjectWithTag("UICamera"))
                uiCamera = GameObject.FindGameObjectWithTag("UICamera").GetComponent<Camera>();
        }
        if (postProcessingCamera == null)
            if(GameObject.FindGameObjectWithTag("PostProcessingCamera"))
                postProcessingCamera = GameObject.FindGameObjectWithTag("PostProcessingCamera").GetComponent<Camera>();

        if(!cameraData.cameraStack.Contains(uiCamera))
        cameraData.cameraStack.Add(uiCamera);
        if(!cameraData.cameraStack.Contains(postProcessingCamera))
        cameraData.cameraStack.Add(postProcessingCamera);

        if(cameraData.cameraStack[0] == postProcessingCamera)
        {
            cameraData.cameraStack[0] = uiCamera;
            cameraData.cameraStack[1] = postProcessingCamera;
        }
    }
}
