using System;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

public class SigmaTerrainGUI : SigmaPBRGUI
{
    private readonly MaterialHeaderScopeList materialScopeList = new MaterialHeaderScopeList();
    private MaterialEditor materialEditor;
    private bool firstTimeOpen = true;
    private const int queueOffsetRange = 50;
    
    private PBRShaderProperty useTriplanarMapping = new("_TRIPLANAR_MAPPING", "Use Triplanar Mapping");
    private PBRShaderProperty triplanarTile = new("_TriplanarTile", "Triplanar Tile");
    private PBRShaderProperty triplanarBlendOffset = new("_TriplanarBlendOffset", "Triplanar Blend Offset");
    private PBRShaderProperty triplanarBlendExponent = new("_TriplanarBlendExponent", "Triplanar Blend Exponent");
    
    public struct TerrainLayerProperties
    {
        public PBRShaderProperty baseColor;
        public PBRShaderProperty baseTexture;
        public PBRShaderProperty metallicMap;
        public PBRShaderProperty metallic;
        public PBRShaderProperty smoothnessMap;
        public PBRShaderProperty smoothness;
        public PBRShaderProperty convertFromRoughness;
        public PBRShaderProperty normalTexture;
        public PBRShaderProperty normalStrength;
        public PBRShaderProperty heightMap;
        public PBRShaderProperty heightMapStrength;
        public PBRShaderProperty occlusionMap;
        public PBRShaderProperty occlusionStrength;
        public PBRShaderProperty emissionMap;
        public PBRShaderProperty emissionColor;
    }

    public static int layerCount = 4;
    public TerrainLayerProperties[] terrainLayerPropertiesArr = new TerrainLayerProperties[layerCount];
        
    private string IndexedName(string name, int index)
    {
        return index == 0 ? name : $"{name}_{index}";
    }
    
    private void InitTerrainLayerProperties(MaterialProperty[] props)
    {
        for (int i = 0; i < layerCount; i++)
        {
            TerrainLayerProperties p = new TerrainLayerProperties();
            p.baseColor = new PBRShaderProperty(IndexedName("_BaseTint", i), "Base Color");
            p.baseTexture = new PBRShaderProperty(IndexedName("_BaseTexture", i), "Base Texture");
            p.metallicMap = new PBRShaderProperty(IndexedName("_MetallicMap", i), "Metallic Map");
            p.metallic = new PBRShaderProperty(IndexedName("_Metallic", i), "Metallic");
            p.smoothnessMap = new PBRShaderProperty(IndexedName("_SmoothnessMap", i), "Smoothness Map");
            p.smoothness = new PBRShaderProperty(IndexedName("_Smoothness", i), "Smoothness");
            p.normalTexture = new PBRShaderProperty(IndexedName("_NormalTexture", i), "Normal Texture");
            p.normalStrength = new PBRShaderProperty(IndexedName("_NormalStrength", i), "Normal Strength");
            p.heightMap = new PBRShaderProperty(IndexedName("_HeightMap", i), "Height Map");
            p.heightMapStrength = new PBRShaderProperty(IndexedName("_HeightMapStrength", i), "Height Map Strength");
            p.occlusionMap = new PBRShaderProperty(IndexedName("_OcclusionMap", i), "Occlusion Map");
            p.occlusionStrength = new PBRShaderProperty(IndexedName("_OcclusionStrength", i), "Occlusion Strength");
            p.emissionMap = new PBRShaderProperty(IndexedName("_EmissionTexture", i), "Emission Texture");
            p.emissionColor = new PBRShaderProperty(IndexedName("_EmissionColor", i), "Emission Color");
            
            p.baseColor.prop = FindProperty(p.baseColor.name, props, true);
            p.baseTexture.prop = FindProperty(p.baseTexture.name, props, true);
            p.metallicMap.prop = FindProperty(p.metallicMap.name, props, true);
            p.metallic.prop = FindProperty(p.metallic.name, props, true);
            p.smoothnessMap.prop = FindProperty(p.smoothnessMap.name, props, true);
            p.smoothness.prop = FindProperty(p.smoothness.name, props, true);
            p.normalTexture.prop = FindProperty(p.normalTexture.name, props, true);
            p.normalStrength.prop = FindProperty(p.normalStrength.name, props, true);
            p.heightMap.prop = FindProperty(p.heightMap.name, props, true);
            p.heightMapStrength.prop = FindProperty(p.heightMapStrength.name, props, true);
            p.occlusionMap.prop = FindProperty(p.occlusionMap.name, props, true);
            p.occlusionStrength.prop = FindProperty(p.occlusionStrength.name, props, true);
            p.emissionMap.prop = FindProperty(p.emissionMap.name, props, true);
            p.emissionColor.prop = FindProperty(p.emissionColor.name, props, true);

            terrainLayerPropertiesArr[i] = p;
        }
    }

    private void FindProperties(MaterialProperty[] props)
    {
        useTriplanarMapping.prop = FindProperty(useTriplanarMapping.name, props, true);
        triplanarTile.prop = FindProperty(triplanarTile.name, props, true);
        triplanarBlendOffset.prop = FindProperty(triplanarBlendOffset.name, props, true);
        triplanarBlendExponent.prop = FindProperty(triplanarBlendExponent.name, props, true);
        
        InitTerrainLayerProperties(props);
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
            materialScopeList.RegisterHeaderScope(new GUIContent("PBR Inputs"), 1u << 1, DrawPBRProperties);
            materialScopeList.RegisterHeaderScope(new GUIContent("Advanced Options"), 1u << 2, DrawAdvancedSettings);
            firstTimeOpen = false;
        }   

        materialScopeList.DrawHeaders(materialEditor, material);
        
        materialEditor.serializedObject.ApplyModifiedProperties();
    } 

    private bool[] layerExpanded = new bool[layerCount];

    private void DrawPBRProperties(Material material)
    {
        //Triplanar
        materialEditor.ShaderProperty(useTriplanarMapping.prop, useTriplanarMapping.info);
        if (useTriplanarMapping.prop.floatValue > 0.5f)
        {
            EditorGUI.indentLevel++;
            materialEditor.ShaderProperty(triplanarTile.prop, triplanarTile.info);
            materialEditor.ShaderProperty(triplanarBlendOffset.prop, triplanarBlendOffset.info);
            materialEditor.ShaderProperty(triplanarBlendExponent.prop, triplanarBlendExponent.info);
            EditorGUI.indentLevel--;
        }
        
        EditorGUILayout.Separator();

        for (int i = 0; i < terrainLayerPropertiesArr.Length; i++)
        {
            layerExpanded[i] = EditorGUILayout.BeginFoldoutHeaderGroup(layerExpanded[i], $"Layer {i + 1}");
            
            if (layerExpanded[i])
            {
                DrawTerrainLayerProperties(material, terrainLayerPropertiesArr[i]);
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }
    }

    private void DrawTerrainLayerProperties(Material material, TerrainLayerProperties p)
    {
        // materialEditor.ShaderProperty(textureFilter.prop, textureFilter.info);
        // materialEditor.ShaderProperty(textureWrap.prop, textureWrap.info);
        
        materialEditor.TexturePropertySingleLine(p.baseTexture.info, p.baseTexture.prop, p.baseColor.prop);
        materialEditor.TexturePropertySingleLine(p.metallicMap.info, p.metallicMap.prop, p.metallic.prop);
        materialEditor.TexturePropertySingleLine(p.smoothnessMap.info, p.smoothnessMap.prop, p.smoothness.prop);
        materialEditor.TexturePropertySingleLine(p.normalTexture.info, p.normalTexture.prop, p.normalStrength.prop);
        materialEditor.TexturePropertySingleLine(p.heightMap.info, p.heightMap.prop, p.heightMapStrength.prop);
        materialEditor.TexturePropertySingleLine(p.occlusionMap.info, p.occlusionMap.prop, p.occlusionStrength.prop);
        materialEditor.TexturePropertySingleLine(p.emissionMap.info, p.emissionMap.prop, p.emissionColor.prop);
    }

    private void DrawAdvancedSettings(Material material)
    {
        materialEditor.EnableInstancingField();
    }
}
