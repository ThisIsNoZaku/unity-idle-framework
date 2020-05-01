﻿using IdleFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LabelComponentGenerator : UiComponentGenerator<LabelConfiguration>
{
    public GameObject Generate(LabelConfiguration uiConfiguration, GameObject parent, IdleEngine engine)
    {
        GameObject instantiatedObject = GameObject.Instantiate(Resources.Load<GameObject>("UI/Component/Prefabs/Label"), parent.transform, false);
        var component = instantiatedObject.GetComponent<LabelComponent>();
        var engineField = typeof(LabelComponent).GetField("engine", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        engineField.SetValue(component, engine);
        component.toDisplay = uiConfiguration.Value;

        return instantiatedObject;
    }
}