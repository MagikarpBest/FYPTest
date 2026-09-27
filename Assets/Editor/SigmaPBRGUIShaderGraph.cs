using System;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;    

public class SigmaPBRGUIShaderGraph : SigmaPBRGUI
{
    private readonly MaterialHeaderScopeList materialScopeList = new MaterialHeaderScopeList();
    private MaterialEditor materialEditor;
    private bool firstTimeOpen = true;
    private const int queueOffsetRange = 50;
    
    private string[] surfaceTypeNames = Enum.GetNames(typeof(SurfaceType));
    private string[] renderFaceNames = Enum.GetNames(typeof(RenderFace));
    private string[] blendFunctionNames = Enum.GetNames(typeof(BlendFunction));
    private string[] zWriteControlNames = Enum.GetNames(typeof(ZWriteControl));
    private string[] queueControlNames =  Enum.GetNames(typeof(QueueControl));
    private string[] compareFunctionNames = Enum.GetNames(typeof(CompareFunction));
    
    private PBRShaderProperty textureFilter = new("_TEXTUREFILTER", "Texture Filter");
    private PBRShaderProperty textureWrap = new("_TEXTUREWRAP", "Texture Wrap");
    
    private PBRShaderProperty textureTiling = new("_TextureTiling", "Texture Tiling");
    
    private PBRShaderProperty textureOffset = new("_TextureOffset", "Texture Offset");
    
    private PBRShaderProperty useTriplanarMapping = new("_TRIPLANAR_MAPPING", "Use Triplanar Mapping");
    private PBRShaderProperty triplanarTile = new("_TriplanarTile", "Triplanar Tile");
    private PBRShaderProperty triplanarBlendOffset = new("_TriplanarBlendOffset", "Triplanar Blend Offset");
    private PBRShaderProperty triplanarBlendExponent = new("_TriplanarBlendExponent", "Triplanar Blend Exponent");
    
    private PBRShaderProperty baseColor = new("_BaseColor", "Base Color");
    private PBRShaderProperty baseTexture = new("_BaseTexture", "Base Texture");
    private PBRShaderProperty workflowMode = new("_WorkflowMode", "Workflow Mode");
    private readonly string[] workflowModeNames = { "Specular", "Metallic" };
    private PBRShaderProperty metallicMap = new("_MetallicMap", "Metallic Map");
    private PBRShaderProperty metallic = new("_Metallic", "Metallic");
    private PBRShaderProperty specularMap = new("_SpecularMap", "Specular Map");
    private PBRShaderProperty specularColor = new("_SpecularColor", "Specular Color");
    private PBRShaderProperty smoothnessMap = new("_SmoothnessMap", "Smoothness Map");
    private PBRShaderProperty smoothness = new("_Smoothness", "Smoothness");
    private PBRShaderProperty convertFromRoughness = new("_ConvertFromRoughness", "Convert From Roughness");
    private PBRShaderProperty normalTexture = new("_NormalTexture", "Normal Texture");
    private PBRShaderProperty normalStrength = new("_NormalStrength", "Normal Strength");
    private PBRShaderProperty heightMap = new("_HeightMap", "Height Map");
    private PBRShaderProperty heightMapStrength = new("_HeightMapStrength", "Height Map Strength");
    private PBRShaderProperty useHeightMap = new("_UseHeightMap", "Use Height Map");
    private PBRShaderProperty occlusionMap = new("_OcclusionMap", "Occlusion Map");
    private PBRShaderProperty occlusionStrength = new("_OcclusionStrength", "Occlusion Strength");
    private PBRShaderProperty useOcclusion = new("_UseOcclusion", "Use Occlusion");
    private PBRShaderProperty emissionMap = new("_EmissionMap", "Emission Map");
    private PBRShaderProperty emissionColor = new("_EmissionColor", "Emission Color");
    private PBRShaderProperty useEmission = new("_UseEmission", "Use Emission");

    private PBRShaderProperty surface = new("_Surface", "Surface Type");
    private PBRShaderProperty cutoff = new("_Cutoff", "Alpha Cutoff");
    private PBRShaderProperty srcBlend = new("_SrcBlend", "Source Blend");
    private PBRShaderProperty dstBlend = new("_DstBlend", "Destination Blend");
    private PBRShaderProperty srcBlendAlpha = new("_SrcBlendAlpha", "Source Blend Alpha");
    private PBRShaderProperty dstBlendAlpha = new("_DstBlendAlpha", "Destination Blend Alpha");
    private PBRShaderProperty zWrite = new("_ZWrite", "ZWrite");
    private PBRShaderProperty zTest = new("_ZTest", "ZTest");
    private PBRShaderProperty cull = new("_Cull", "Render Face");
    private PBRShaderProperty alphaToMask = new("_AlphaToMask", "Alpha To Mask");

    private PBRShaderProperty castShadows = new("_CastShadows", "Cast Shadows");
    private PBRShaderProperty receiveShadows = new("_ReceiveShadows", "Receive Shadows");
    private PBRShaderProperty blend = new("_Blend", "Blend Mode");
    private PBRShaderProperty alphaClip = new("_AlphaClip", "Alpha Clipping");
    private PBRShaderProperty zWriteControl = new("_ZWriteControl", "ZWrite Control");
    private PBRShaderProperty queueOffset = new("_QueueOffset", "Sorting Priority");
    private PBRShaderProperty queueControl = new("_QueueControl", "Queue Control");
    private void FindProperties(MaterialProperty[] props)
    {
        textureFilter.prop = FindProperty(textureFilter.name, props, true);
        textureWrap.prop = FindProperty(textureWrap.name, props, true);
        
        textureTiling.prop = FindProperty(textureTiling.name, props, true);
        textureOffset.prop = FindProperty(textureOffset.name, props, true);
        
        baseColor.prop = FindProperty(baseColor.name, props, true);
        baseTexture.prop = FindProperty(baseTexture.name, props, true);
        
        useTriplanarMapping.prop = FindProperty(useTriplanarMapping.name, props, true);
        triplanarTile.prop = FindProperty(triplanarTile.name, props, true);
        triplanarBlendOffset.prop = FindProperty(triplanarBlendOffset.name, props, true);
        triplanarBlendExponent.prop = FindProperty(triplanarBlendExponent.name, props, true);
        
        workflowMode.prop = FindProperty(workflowMode.name, props, true);
        metallicMap.prop = FindProperty(metallicMap.name, props, true);
        metallic.prop = FindProperty(metallic.name, props, true);
        specularMap.prop = FindProperty(specularMap.name, props, true);
        specularColor.prop = FindProperty(specularColor.name, props, true);
        smoothnessMap.prop = FindProperty(smoothnessMap.name, props, true);
        smoothness.prop = FindProperty(smoothness.name, props, true);
        convertFromRoughness.prop = FindProperty(convertFromRoughness.name, props, true);
        normalTexture.prop = FindProperty(normalTexture.name, props, true);
        normalStrength.prop = FindProperty(normalStrength.name, props, true);
        heightMap.prop = FindProperty(heightMap.name, props, true);
        heightMapStrength.prop = FindProperty(heightMapStrength.name, props, true);
        useHeightMap.prop = FindProperty(useHeightMap.name, props, true);
        occlusionMap.prop = FindProperty(occlusionMap.name, props, true);
        occlusionStrength.prop = FindProperty(occlusionStrength.name, props, true);
        useOcclusion.prop = FindProperty(useOcclusion.name, props, true);
        emissionMap.prop = FindProperty(emissionMap.name, props, true);
        emissionColor.prop = FindProperty(emissionColor.name, props, true);
        useEmission.prop = FindProperty(useEmission.name, props, true);
        
        surface.prop = FindProperty(surface.name, props, true);
        cutoff.prop = FindProperty(cutoff.name, props, true);
        srcBlend.prop = FindProperty(srcBlend.name, props, true);
        dstBlend.prop = FindProperty(dstBlend.name, props, true);
        srcBlendAlpha.prop = FindProperty(srcBlendAlpha.name, props, true);
        dstBlendAlpha.prop = FindProperty(dstBlendAlpha.name, props, true);
        zWrite.prop = FindProperty(zWrite.name, props, true);
        zTest.prop = FindProperty(zTest.name, props, true);
        cull.prop = FindProperty(cull.name, props, true);
        alphaToMask.prop = FindProperty(alphaToMask.name, props, true);
        
        castShadows.prop = FindProperty(castShadows.name, props, true);
        receiveShadows.prop = FindProperty(receiveShadows.name, props, true);
        blend.prop = FindProperty(blend.name, props, true);
        alphaClip.prop = FindProperty(alphaClip.name, props, true);
        zWriteControl.prop = FindProperty(zWriteControl.name, props, true);
        queueOffset.prop = FindProperty(queueOffset.name, props, true);
        queueControl.prop = FindProperty(queueControl.name, props, true);
    }
    
    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        if (materialEditor == null)
        {
            throw new ArgumentNullException("No MaterialEditor found in " + this);
        }
        
        this.materialEditor = materialEditor;
        var material = materialEditor.target as Material;
        
        FindProperties(properties);
        
        if (firstTimeOpen)
        {
            materialScopeList.RegisterHeaderScope(new GUIContent("Surface Options"), 1u << 0, DrawSurfaceProperties);
            materialScopeList.RegisterHeaderScope(new GUIContent("PBR Inputs"), 1u << 1, DrawPBRProperties);
            materialScopeList.RegisterHeaderScope(new GUIContent("Advanced Options"), 1u << 2, DrawAdvancedSettings);
            firstTimeOpen = false;
        }   

        materialScopeList.DrawHeaders(materialEditor, material);
        
        materialEditor.serializedObject.ApplyModifiedProperties();
    }
    
    private void DrawSurfaceProperties(Material material)
    {
        materialEditor.PopupShaderProperty(surface.prop, surface.info, surfaceTypeNames);
        var surfaceTypeValue = (SurfaceType)material.GetFloat(surface.id);

        if (surfaceTypeValue == SurfaceType.Transparent)
        {
            materialEditor.PopupShaderProperty(blend.prop, blend.info, blendFunctionNames);
        }

        materialEditor.PopupShaderProperty(cull.prop, cull.info, renderFaceNames);
        materialEditor.PopupShaderProperty(zWriteControl.prop, zWriteControl.info, zWriteControlNames);
        materialEditor.PopupShaderProperty(zTest.prop, zTest.info, compareFunctionNames);
        
        //Alpha clip
        var alphaClipValue = material.GetFloat(alphaClip.id) > 0.5f;
        
        EditorGUI.BeginChangeCheck();
        alphaClipValue = EditorGUILayout.Toggle(alphaClip.info, alphaClipValue);
        
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(material, "Toggle Alpha Clip");
            material.SetFloat(alphaClip.id, alphaClipValue ? 1.0f: 0.0f);
        }

        if (alphaClipValue)
        {
            EditorGUI.indentLevel++;
            materialEditor.ShaderProperty(cutoff.prop, cutoff.info);
            EditorGUI.indentLevel--;
        }

        bool useAlphaToMask = false;
        int renderQueueValue = material.shader.renderQueue;
        bool useZWrite = false;
        
        var blendFuncValue = (BlendFunction)material.GetFloat(blend.id);
        
        //Opaque and Trans property
        if (surfaceTypeValue == SurfaceType.Opaque)
        {
            SetBlendMode(blendFuncValue, surfaceTypeValue, material);
            useZWrite = true;
            material.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");

            if (alphaClipValue)
            {
                material.EnableKeyword("_ALPHATEST_ON");
                renderQueueValue = (int)RenderQueue.AlphaTest;
                material.SetOverrideTag("RenderType", "AlphaTest");
                useAlphaToMask = true;
            }
            else
            {
                material.DisableKeyword("_ALPHATEST_ON");
                renderQueueValue = (int)RenderQueue.Geometry;
                material.SetOverrideTag("RenderType", "Opaque");
            }
        }
        else
        {
            material.SetOverrideTag("RenderType", "Transparent");
            SetBlendMode(blendFuncValue, surfaceTypeValue, material);
            useZWrite = false;
            renderQueueValue = (int)RenderQueue.Transparent;
            material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");

            if (alphaClipValue)
            {
                material.EnableKeyword("_ALPHATEST_ON");
            }
            else
            {
                material.DisableKeyword("_ALPHATEST_ON");
            }
        }
        
        material.SetFloat(alphaToMask.id, useAlphaToMask ? 1.0f : 0.0f);
        
        var useZWriteControl = (ZWriteControl)material.GetFloat(zWriteControl.id);

        if (useZWriteControl == ZWriteControl.ForceEnabled)
        {
            useZWrite = true;
        }
        else if (useZWriteControl == ZWriteControl.ForceDisabled)
        {
            useZWrite = false;
        }

        material.SetFloat(zWrite.id, useZWrite ? 1.0f : 0.0f);
        material.SetShaderPassEnabled("DepthOnly", useZWrite);
        
        //Queue control
        if (material.GetFloat(queueControl.id) == (float)QueueControl.Auto)
        {
            renderQueueValue += (int)material.GetFloat(queueOffset.id);
            material.renderQueue = renderQueueValue;
        }
        
        //Shadows
        bool castShadowsValue = material.GetFloat(castShadows.id) > 0.5f;

        EditorGUI.BeginChangeCheck();
        {
            castShadowsValue = EditorGUILayout.Toggle(castShadows.info, castShadowsValue);
        }
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(material, "Toggle Cast Shadows");
            material.SetFloat(castShadows.id, castShadowsValue ? 1.0f : 0.0f);
    
            material.SetShaderPassEnabled("ShadowCaster", castShadowsValue);
        }

        bool receiveShadowsValue = material.GetFloat(receiveShadows.id) > 0.5f;

        EditorGUI.BeginChangeCheck();
        {
            receiveShadowsValue = EditorGUILayout.Toggle(receiveShadows.info, receiveShadowsValue);
        }
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(material, "Toggle Receive Shadows");
            material.SetFloat(receiveShadows.id, receiveShadowsValue ? 1.0f : 0.0f);

            if (receiveShadowsValue)
            {
                material.DisableKeyword("_RECEIVE_SHADOWS_OFF");
            }
            else
            {
                material.EnableKeyword("_RECEIVE_SHADOWS_OFF");
            }
        }
    }

    private void DrawPBRProperties(Material material)
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

    private void DrawAdvancedSettings(Material material)
    {
        // If auto queue is used, then use sorting priority field. Otherwise, let user set render queue freely.
        materialEditor.PopupShaderProperty(queueControl.prop, queueControl.info, queueControlNames);

        if(material.GetFloat(queueControl.id) == (float)QueueControl.UserOverride)
        {
            materialEditor.RenderQueueField();
        }
        else
        {
            materialEditor.IntSliderShaderProperty(queueOffset.prop, -queueOffsetRange, queueOffsetRange, queueOffset.info);
        }
        
        materialEditor.EnableInstancingField();
    }
}
