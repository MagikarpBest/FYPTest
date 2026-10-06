using System;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;    
using System.Collections.Generic;
using System.Reflection;

public class SigmaPBRGUI : ShaderGUI
{
    protected readonly MaterialHeaderScopeList materialScopeList = new MaterialHeaderScopeList();
    protected MaterialEditor materialEditor;
    protected bool firstTimeOpen = true;
    protected const int queueOffsetRange = 50;
    
    public struct PBRShaderProperty
    {
        public MaterialProperty prop;
        public readonly string name;
        public readonly GUIContent info;
        public readonly int id;

        public PBRShaderProperty(string name, string label)
        {
            prop = null;
            this.name = name;
            info = new GUIContent(label);
            id = Shader.PropertyToID(name);
        }
    }

    public enum SurfaceType
    {
        Opaque = 0,
        Transparent = 1
    }

    public enum RenderFace
    {
        Front = 2,
        Back = 1,
        Both = 0
    }

    public enum BlendFunction
    {
        Alpha = 0,
        Premultiply = 1,
        Additive = 2,
        Multiply = 3
    }
    
    public enum ZWriteControl
    {
        Auto = 0,
        ForceEnabled = 1,
        ForceDisabled = 2
    }
    
    public enum QueueControl
    {
        Auto = 0,
        UserOverride = 1
    }
    
    public enum CompareFunction
    {
        Disabled,
        Never,
        Less,
        Equal,
        LessEqual,
        Greater,
        NotEqual,
        GreaterEqual,
        Always,
    }
    
    protected string[] surfaceTypeNames = Enum.GetNames(typeof(SurfaceType));
    protected string[] renderFaceNames = Enum.GetNames(typeof(RenderFace));
    protected string[] blendFunctionNames = Enum.GetNames(typeof(BlendFunction));
    protected string[] zWriteControlNames = Enum.GetNames(typeof(ZWriteControl));
    protected string[] queueControlNames =  Enum.GetNames(typeof(QueueControl));
    protected string[] compareFunctionNames = Enum.GetNames(typeof(CompareFunction));
    
    protected PBRShaderProperty textureFilter = new("_TEXTUREFILTER", "Texture Filter");
    protected PBRShaderProperty textureWrap = new("_TEXTUREWRAP", "Texture Wrap");

    protected PBRShaderProperty useTriplanarMapping = new("_TRIPLANAR_MAPPING", "Use Triplanar Mapping");
    protected PBRShaderProperty triplanarTile = new("_TriplanarTile", "Triplanar Tile");
    protected PBRShaderProperty triplanarBlendOffset = new("_TriplanarBlendOffset", "Triplanar Blend Offset");
    protected PBRShaderProperty triplanarBlendExponent = new("_TriplanarBlendExponent", "Triplanar Blend Exponent");

    //Base map
    protected PBRShaderProperty baseColor = new("_BaseColor", "Base Color");
    protected PBRShaderProperty baseTexture = new("_BaseTexture", "Base Texture");
    private PBRShaderProperty useSpecularSetup = new("_UseSpecularSetup", "Use Specular Setup");
    protected PBRShaderProperty metallicMap = new("_MetallicMap", "Metallic Map");
    protected PBRShaderProperty metallic = new("_Metallic", "Metallic");
    protected PBRShaderProperty specularMap = new("_SpecularMap", "Specular Map");
    protected PBRShaderProperty specularColor = new("_SpecularColor", "Specular Color");
    protected PBRShaderProperty smoothnessMap = new("_SmoothnessMap", "Smoothness Map");
    protected PBRShaderProperty smoothness = new("_Smoothness", "Smoothness");
    protected PBRShaderProperty convertFromRoughness = new("_ConvertFromRoughness", "Convert From Roughness");
    protected PBRShaderProperty normalTexture = new("_NormalTexture", "Normal Texture");
    protected PBRShaderProperty normalStrength = new("_NormalStrength", "Normal Strength");
    protected PBRShaderProperty heightMap = new("_HeightMap", "Height Map");
    protected PBRShaderProperty heightMapStrength = new("_HeightMapStrength", "Height Map Strength");
    protected PBRShaderProperty occlusionMap = new("_OcclusionMap", "Occlusion Map");
    protected PBRShaderProperty occlusionStrength = new("_OcclusionStrength", "Occlusion Strength");
    protected PBRShaderProperty emissionMap = new("_EmissionMap", "Emission Map");
    protected PBRShaderProperty emissionColor = new("_EmissionColor", "Emission Color");

    //Top map
    private PBRShaderProperty useSeparateTopMap = new("_SeparateTopMap", "Use Top Map");
    
    private PBRShaderProperty topBaseColor = new("_TopBaseColor", "Base Color");
    private PBRShaderProperty topBaseTexture = new("_TopBaseTexture", "Base Texture");
    private PBRShaderProperty topMetallicMap = new("_TopMetallicMap", "Metallic Map");
    private PBRShaderProperty topMetallic = new("_TopMetallic", "Metallic");
    private PBRShaderProperty topSpecularMap = new("_TopSpecularMap", "Specular Map");
    private PBRShaderProperty topSpecularColor = new("_TopSpecularColor", "Specular Color");
    private PBRShaderProperty topSmoothnessMap = new("_TopSmoothnessMap", "Smoothness Map");
    private PBRShaderProperty topSmoothness = new("_TopSmoothness", "Smoothness");
    private PBRShaderProperty topConvertFromRoughness = new("_TopConvertFromRoughness", "Convert From Roughness");
    private PBRShaderProperty topNormalTexture = new("_TopNormalTexture", "Normal Texture");
    private PBRShaderProperty topNormalStrength = new("_TopNormalStrength", "Normal Strength");
    private PBRShaderProperty topHeightMap = new("_TopHeightMap", "Height Map");
    private PBRShaderProperty topHeightMapStrength = new("_TopHeightMapStrength", "Height Map Strength");
    private PBRShaderProperty topOcclusionMap = new("_TopOcclusionMap", "Occlusion Map");
    private PBRShaderProperty topOcclusionStrength = new("_TopOcclusionStrength", "Occlusion Strength");
    private PBRShaderProperty topEmissionMap = new("_TopEmissionMap", "Emission Map");
    private PBRShaderProperty topEmissionColor = new("_TopEmissionColor", "Emission Color");
    
    protected PBRShaderProperty surface = new("_Surface", "Surface Type");
    protected PBRShaderProperty cutoff = new("_Cutoff", "Alpha Cutoff");
    protected PBRShaderProperty srcBlend = new("_SrcBlend", "Source Blend");
    protected PBRShaderProperty dstBlend = new("_DstBlend", "Destination Blend");
    protected PBRShaderProperty srcBlendAlpha = new("_SrcBlendAlpha", "Source Blend Alpha");
    protected PBRShaderProperty dstBlendAlpha = new("_DstBlendAlpha", "Destination Blend Alpha");
    protected PBRShaderProperty zWrite = new("_ZWrite", "ZWrite");
    protected PBRShaderProperty zTest = new("_ZTest", "ZTest");
    protected PBRShaderProperty cull = new("_Cull", "Render Face");
    protected PBRShaderProperty alphaToMask = new("_AlphaToMask", "Alpha To Mask");
    
    protected PBRShaderProperty castShadows = new("_CastShadows", "Cast Shadows");
    protected PBRShaderProperty receiveShadows = new("_ReceiveShadows", "Receive Shadows");
    protected PBRShaderProperty blend = new("_Blend", "Blend Mode");
    protected PBRShaderProperty alphaClip = new("_AlphaClip", "Alpha Clipping");
    protected PBRShaderProperty zWriteControl = new("_ZWriteControl", "ZWrite Control");
    protected PBRShaderProperty queueOffset = new("_QueueOffset", "Sorting Priority");
    protected PBRShaderProperty queueControl = new("_QueueControl", "Queue Control");
    
    protected void FindCommonProperties(MaterialProperty[] props)
    {
        textureFilter.prop = FindProperty(textureFilter.name, props, true);
        textureWrap.prop = FindProperty(textureWrap.name, props, true);
        
        baseColor.prop = FindProperty(baseColor.name, props, true);
        baseTexture.prop = FindProperty(baseTexture.name, props, true);
        
        useTriplanarMapping.prop = FindProperty(useTriplanarMapping.name, props, true);
        triplanarTile.prop = FindProperty(triplanarTile.name, props, true);
        triplanarBlendOffset.prop = FindProperty(triplanarBlendOffset.name, props, true);
        triplanarBlendExponent.prop = FindProperty(triplanarBlendExponent.name, props, true);
        
        //Base map
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
        occlusionMap.prop = FindProperty(occlusionMap.name, props, true);
        occlusionStrength.prop = FindProperty(occlusionStrength.name, props, true);
        emissionMap.prop = FindProperty(emissionMap.name, props, true);
        emissionColor.prop = FindProperty(emissionColor.name, props, true);
        
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
    protected virtual void FindProperties(MaterialProperty[] props)
    {
        FindCommonProperties(props);
        
        useSpecularSetup.prop = FindProperty(useSpecularSetup.name, props, true);
        
        //Top map
        useSeparateTopMap.prop = FindProperty(useSeparateTopMap.name, props, true);
        
        topBaseColor.prop = FindProperty(topBaseColor.name, props, true);
        topBaseTexture.prop = FindProperty(topBaseTexture.name, props, true);
        topMetallicMap.prop = FindProperty(topMetallicMap.name, props, true);
        topMetallic.prop = FindProperty(topMetallic.name, props, true);
        topSpecularMap.prop = FindProperty(topSpecularMap.name, props, true);
        topSpecularColor.prop = FindProperty(topSpecularColor.name, props, true);
        topSmoothnessMap.prop = FindProperty(topSmoothnessMap.name, props, true);
        topSmoothness.prop = FindProperty(topSmoothness.name, props, true);
        topConvertFromRoughness.prop = FindProperty(topConvertFromRoughness.name, props, true);
        topNormalTexture.prop = FindProperty(topNormalTexture.name, props, true);
        topNormalStrength.prop = FindProperty(topNormalStrength.name, props, true);
        topHeightMap.prop = FindProperty(topHeightMap.name, props, true);
        topHeightMapStrength.prop = FindProperty(topHeightMapStrength.name, props, true);
        topOcclusionMap.prop = FindProperty(topOcclusionMap.name, props, true);
        topOcclusionStrength.prop = FindProperty(topOcclusionStrength.name, props, true);
        topEmissionMap.prop = FindProperty(topEmissionMap.name, props, true);
        topEmissionColor.prop = FindProperty(topEmissionColor.name, props, true);
        
    }
    
    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        if (materialEditor == null)
        {
            throw new ArgumentNullException("No MaterialEditor found in " + this);
        }
        
        this.materialEditor = materialEditor;
        var material = materialEditor.target as Material;
        
        currentProps = properties;  
        FindProperties(properties);
        
        if (firstTimeOpen)
        {
            GetDefinedShaderProperties();
            
            materialScopeList.RegisterHeaderScope(new GUIContent("Surface Options"), 1u << 0, DrawSurfaceProperties);
            materialScopeList.RegisterHeaderScope(new GUIContent("PBR Inputs"), 1u << 1, DrawPBRProperties);
            materialScopeList.RegisterHeaderScope(new GUIContent("Advanced Options"), 1u << 2, DrawAdvancedSettings);
            materialScopeList.RegisterHeaderScope(new GUIContent("Extra Properties"), 1u << 3, DrawExtraProperties);
            firstTimeOpen = false;
        }   

        materialScopeList.DrawHeaders(materialEditor, material);
        
        materialEditor.serializedObject.ApplyModifiedProperties();
    }
    
    protected void SetBlendMode(BlendFunction blendFunction, SurfaceType surfaceType, Material material)
    {
        var srcBlendRGB = BlendMode.One;
        var dstBlendRGB = BlendMode.Zero;
        var srcBlendA = BlendMode.One;
        var dstBlendA = BlendMode.Zero;

        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.DisableKeyword("_ALPHAMODULATE_ON");
        
        if (surfaceType == SurfaceType.Transparent)
        {
            switch (blendFunction)
            {
                case BlendFunction.Alpha:
                {
                    srcBlendRGB = BlendMode.SrcAlpha;
                    dstBlendRGB = BlendMode.OneMinusSrcAlpha;
                    srcBlendA = BlendMode.One;
                    dstBlendA = BlendMode.OneMinusSrcAlpha;
                    break;
                }
                case BlendFunction.Premultiply:
                {
                    srcBlendRGB = BlendMode.One;
                    dstBlendRGB = BlendMode.OneMinusSrcAlpha;
                    srcBlendA = BlendMode.One;
                    dstBlendA = BlendMode.OneMinusSrcAlpha;
                    material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                    break;
                }
                case BlendFunction.Additive:
                {
                    srcBlendRGB = BlendMode.SrcAlpha;
                    dstBlendRGB = BlendMode.One;
                    srcBlendA = BlendMode.One;
                    dstBlendA = BlendMode.One;
                    break;
                }
                case BlendFunction.Multiply:
                {
                    srcBlendRGB = BlendMode.DstColor;
                    dstBlendRGB = BlendMode.Zero;
                    srcBlendA = BlendMode.Zero;
                    dstBlendA = BlendMode.One;
                    material.EnableKeyword("_ALPHAMODULATE_ON");
                    break;
                }
            }
        }

        material.SetFloat(srcBlend.id, (float)srcBlendRGB);
        material.SetFloat(dstBlend.id, (float)dstBlendRGB);
        material.SetFloat(srcBlendAlpha.id, (float)srcBlendA);
        material.SetFloat(dstBlendAlpha.id, (float)dstBlendA);
    }
    
    protected virtual void DrawSurfaceProperties(Material material)
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

    protected virtual void DrawPBRProperties(Material material)
    {
        materialEditor.ShaderProperty(textureFilter.prop, textureFilter.info);
        materialEditor.ShaderProperty(textureWrap.prop, textureWrap.info);
        
        DrawBaseMapPBRProperties(material);
        
        EditorGUILayout.Separator();
        materialEditor.ShaderProperty(useTriplanarMapping.prop, useTriplanarMapping.info);
        if (useTriplanarMapping.prop.intValue > 0)
        {
            EditorGUI.indentLevel++;
            materialEditor.ShaderProperty(triplanarTile.prop, triplanarTile.info);
            materialEditor.ShaderProperty(triplanarBlendOffset.prop, triplanarBlendOffset.info);
            materialEditor.ShaderProperty(triplanarBlendExponent.prop, triplanarBlendExponent.info);
            
            EditorGUILayout.Separator();
            materialEditor.ShaderProperty(useSeparateTopMap.prop, useSeparateTopMap.info);

            if (useSeparateTopMap.prop.intValue > 0)
            {
                DrawTopMapPBRProperties(material);
            }

            EditorGUI.indentLevel--;
        }
    }

    private void DrawBaseMapPBRProperties(Material material)
    {
        materialEditor.TexturePropertySingleLine(baseTexture.info, baseTexture.prop, baseColor.prop);
        materialEditor.TextureScaleOffsetProperty(baseTexture.prop);
        materialEditor.ShaderProperty(useSpecularSetup.prop, useSpecularSetup.info);
        
        if (useSpecularSetup.prop.intValue > 0)
        {
            materialEditor.TexturePropertySingleLine(specularMap.info, specularMap.prop, specularColor.prop);
        }
        else
        {
            materialEditor.TexturePropertySingleLine(metallicMap.info, metallicMap.prop, metallic.prop);
        }
        
        materialEditor.TexturePropertySingleLine(smoothnessMap.info, smoothnessMap.prop, smoothness.prop);
        materialEditor.ShaderProperty(convertFromRoughness.prop, convertFromRoughness.info);
        materialEditor.TexturePropertySingleLine(normalTexture.info, normalTexture.prop, normalStrength.prop);
        materialEditor.TexturePropertySingleLine(heightMap.info, heightMap.prop, heightMapStrength.prop);
        materialEditor.TexturePropertySingleLine(occlusionMap.info, occlusionMap.prop, occlusionStrength.prop);
        materialEditor.TexturePropertySingleLine(emissionMap.info,  emissionMap.prop, emissionColor.prop);
    }
    
    private void DrawTopMapPBRProperties(Material material)
    {
        materialEditor.TexturePropertySingleLine(topBaseTexture.info, topBaseTexture.prop, topBaseColor.prop);
        materialEditor.TextureScaleOffsetProperty(topBaseTexture.prop);

        if (useSpecularSetup.prop.intValue > 0)
        {
            materialEditor.TexturePropertySingleLine(topSpecularMap.info, topSpecularMap.prop, topSpecularColor.prop);
        }
        else
        {
            materialEditor.TexturePropertySingleLine(topMetallicMap.info, topMetallicMap.prop, topMetallic.prop);
        }

        materialEditor.TexturePropertySingleLine(topSmoothnessMap.info, topSmoothnessMap.prop, topSmoothness.prop);
        materialEditor.ShaderProperty(topConvertFromRoughness.prop, topConvertFromRoughness.info);
        materialEditor.TexturePropertySingleLine(topNormalTexture.info, topNormalTexture.prop, topNormalStrength.prop);
        materialEditor.TexturePropertySingleLine(topHeightMap.info, topHeightMap.prop, topHeightMapStrength.prop);
        materialEditor.TexturePropertySingleLine(topOcclusionMap.info, topOcclusionMap.prop, topOcclusionStrength.prop);
        materialEditor.TexturePropertySingleLine(topEmissionMap.info, topEmissionMap.prop, topEmissionColor.prop);
    }
    
    protected virtual void DrawAdvancedSettings(Material material)
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
    
    protected HashSet<string> definedNames;
    protected virtual void GetDefinedShaderProperties()
    {
        definedNames = new HashSet<string>();
        
        FieldInfo[] fields = typeof(SigmaPBRGUI).GetFields( BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        
        foreach (var f in fields)
        {
            if (f.FieldType != typeof(PBRShaderProperty)) continue;

            var p = (PBRShaderProperty)f.GetValue(this);      
            definedNames.Add(p.name);                    
        }
    }
    
    protected MaterialProperty[] currentProps;
    protected void DrawExtraProperties(Material material)
    {
        foreach (var prop in currentProps)
        {
            if (definedNames.Contains(prop.name)) continue; // already defined, skip
            if ((prop.flags & (MaterialProperty.PropFlags.HideInInspector | MaterialProperty.PropFlags.PerRendererData)) != 0) continue;
            if (prop.name.StartsWith("unity_")) continue;

            materialEditor.ShaderProperty(prop, prop.displayName);
        }
    }
}
