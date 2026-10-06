using System;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;    
using System.Collections.Generic;
using System.Reflection;

public class SigmaPBRGUIShaderGraph : SigmaPBRGUI
{
    private PBRShaderProperty textureTiling = new("_TextureTiling", "Texture Tiling");
    private PBRShaderProperty textureOffset = new("_TextureOffset", "Texture Offset");
    private PBRShaderProperty workflowMode = new("_WorkflowMode", "Workflow Mode");
    private readonly string[] workflowModeNames = { "Specular", "Metallic" };
    private PBRShaderProperty useHeightMap = new("_UseHeightMap", "Use Height Map");
    private PBRShaderProperty useOcclusion = new("_UseOcclusion", "Use Occlusion");
    private PBRShaderProperty useEmission = new("_UseEmission", "Use Emission");
    
    protected override void FindProperties(MaterialProperty[] props)
    {
        FindCommonProperties(props);
            
        textureTiling.prop = FindProperty(textureTiling.name, props, true);
        textureOffset.prop = FindProperty(textureOffset.name, props, true);
        workflowMode.prop = FindProperty(workflowMode.name, props, true);
        useHeightMap.prop = FindProperty(useHeightMap.name, props, true);
        useOcclusion.prop = FindProperty(useOcclusion.name, props, true);
        useEmission.prop = FindProperty(useEmission.name, props, true);
    }

    protected override void DrawPBRProperties(Material material)
    {
        materialEditor.ShaderProperty(textureFilter.prop, textureFilter.info);
        materialEditor.ShaderProperty(textureWrap.prop, textureWrap.info);
        
        materialEditor.ShaderProperty(useTriplanarMapping.prop, useTriplanarMapping.info);
        if (useTriplanarMapping.prop.floatValue > 0.5f)
        {
            EditorGUI.indentLevel++;
            materialEditor.ShaderProperty(triplanarTile.prop, triplanarTile.info);
            materialEditor.ShaderProperty(triplanarBlendOffset.prop, triplanarBlendOffset.info);
            materialEditor.ShaderProperty(triplanarBlendExponent.prop, triplanarBlendExponent.info);
            EditorGUI.indentLevel--;
            
            EditorGUILayout.Separator();
        }
        else
        {
            EditorGUILayout.Separator();
        
            materialEditor.ShaderProperty(textureTiling.prop, textureTiling.info);
            materialEditor.ShaderProperty(textureOffset.prop, textureOffset.info);
        }
        
        DrawBaseMapPBRProperties(material);
        
    }

    private void DrawBaseMapPBRProperties(Material material)
    {
        materialEditor.TexturePropertySingleLine(baseTexture.info, baseTexture.prop, baseColor.prop);
        
        materialEditor.PopupShaderProperty(workflowMode.prop, workflowMode.info, workflowModeNames);
        
        //Specular/Metallic
        if (material.GetFloat(workflowMode.id) < 0.5f)
        {
            materialEditor.TexturePropertySingleLine(specularMap.info, specularMap.prop, specularColor.prop);
            material.EnableKeyword("_SPECULAR_SETUP");
        }
        else
        {
            materialEditor.TexturePropertySingleLine(metallicMap.info, metallicMap.prop, metallic.prop);
            material.DisableKeyword("_SPECULAR_SETUP");
        }
        
        materialEditor.TexturePropertySingleLine(smoothnessMap.info, smoothnessMap.prop, smoothness.prop);
        materialEditor.ShaderProperty(convertFromRoughness.prop, convertFromRoughness.info);
        materialEditor.TexturePropertySingleLine(normalTexture.info, normalTexture.prop, normalStrength.prop);
        
        materialEditor.ShaderProperty(useHeightMap.prop, useHeightMap.info);
        if (useHeightMap.prop.floatValue > 0.5f)
        {
            materialEditor.TexturePropertySingleLine(heightMap.info, heightMap.prop, heightMapStrength.prop);
        }
        
        materialEditor.ShaderProperty(useOcclusion.prop, useOcclusion.info);
        if (useOcclusion.prop.floatValue > 0.5f)
        {
            materialEditor.TexturePropertySingleLine(occlusionMap.info, occlusionMap.prop, occlusionStrength.prop);
        }
        
        materialEditor.ShaderProperty(useEmission.prop, useEmission.info);
        if (useEmission.prop.floatValue > 0.5f)
        {
            materialEditor.TexturePropertySingleLine(emissionMap.info, emissionMap.prop, emissionColor.prop);
        }
    }
    
    protected override void GetDefinedShaderProperties()
    {
        definedNames = new HashSet<string>();
        
        FieldInfo[] fields = typeof(SigmaPBRGUIShaderGraph).GetFields( BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        
        foreach (var f in fields)
        {
            if (f.FieldType != typeof(PBRShaderProperty)) continue;

            var p = (PBRShaderProperty)f.GetValue(this);      
            definedNames.Add(p.name);                    
        }
    }
}
