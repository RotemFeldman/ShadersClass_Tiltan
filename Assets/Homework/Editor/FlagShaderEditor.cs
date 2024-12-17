using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEditor;


public class FlagShaderEditor : ShaderGUI
{
    private bool baseWind = true;
    private bool secondWind = true;
    private bool color = false;
    
    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        Material material = materialEditor.target as Material;

        MaterialProperty texture = FindProperty("_Texture", properties);
        materialEditor.ShaderProperty(texture, texture.displayName);
        MaterialProperty flipDirection = FindProperty("_FlipMaskDirection", properties);
        materialEditor.ShaderProperty(flipDirection, flipDirection.displayName);
        
        color = EditorGUILayout.Foldout(color, "Color",true);
        if (color)
        {
            MaterialProperty applyColor = FindProperty("_ApplyColorBySine", properties);
            materialEditor.ShaderProperty(applyColor, applyColor.displayName);
            
            MaterialProperty colorA = FindProperty("_ColorA", properties);
            materialEditor.ShaderProperty(colorA, colorA.displayName);
            MaterialProperty colorB = FindProperty("_ColorB", properties);
            materialEditor.ShaderProperty(colorB, colorB.displayName);
        }

        EditorGUILayout.Space();

        baseWind = EditorGUILayout.Foldout(baseWind, "Base Wind",true);
        if (baseWind)
        {
            MaterialProperty amplitude = FindProperty("_Amplitude", properties);
            materialEditor.ShaderProperty(amplitude, amplitude.displayName);
            MaterialProperty phaseShift = FindProperty("_Phase_Shift", properties);
            materialEditor.ShaderProperty(phaseShift, phaseShift.displayName);
            MaterialProperty period = FindProperty("_Period", properties);
            materialEditor.ShaderProperty(period, period.displayName);
            MaterialProperty verticalShift = FindProperty("_Vertical_Shift", properties);
            materialEditor.ShaderProperty(verticalShift, verticalShift.displayName);
        }
        
        EditorGUILayout.Space();
        
        secondWind = EditorGUILayout.Foldout(secondWind, "Second Wind", true);
        if (secondWind)
        {
            MaterialProperty secondAmplitude = FindProperty("_Second_Amplitude", properties);
            materialEditor.ShaderProperty(secondAmplitude, secondAmplitude.displayName);
            MaterialProperty secondPhaseShift = FindProperty("_Second_Phase_Shift", properties);
            materialEditor.ShaderProperty(secondPhaseShift, secondPhaseShift.displayName);
            MaterialProperty secondPeriod = FindProperty("_Second_Period", properties);
            materialEditor.ShaderProperty(secondPeriod, secondPeriod.displayName);
        }
        
    }
}
