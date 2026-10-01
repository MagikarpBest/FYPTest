using System;
using UnityEditor;
using UnityEngine;

public class PlayerCreateSkill : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private GameObject skillPrefab;
    [SerializeField] private GameObject skillPrefabLaunch;
    [SerializeField] private PlayerInputManager inputManager;
    [SerializeField] private LayerMask summonLayer;

    [SerializeField] private bool isDebug = false;
    private float maxCreateSkillRange = 10f;
    private Camera camera;

    private void Awake()
    {
        camera = Camera.main;
    }

    private void Update()
    {
        if (isDebug == true)
        {
            DrawCameraRay();
        }
    }

    // See what is camera aiming (idk why drawray doesnt show in game view)
    private void DrawCameraRay()
    {
        Ray ray = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        lineRenderer.positionCount = 2;

        lineRenderer.SetPosition(0, new Vector3(ray.origin.x, ray.origin.y - 0.1f, ray.origin.z));
        lineRenderer.SetPosition(1, ray.origin + ray.direction * 100f);

    }

    // Temporary to see skill range
    private void OnDrawGizmos()
    {
        if (isDebug == true)
        {
            Handles.color = Color.green;
            Handles.DrawWireDisc(transform.position, Vector3.up, maxCreateSkillRange);
        }

    }


    private void TrySummon(GameObject skillPrefab)
    {
        Debug.Log("Summon Pressed");
        //middle of camera
        Ray ray = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, summonLayer))
        {
            float distance = Vector3.Distance(transform.position, hit.point);

            if (distance <= maxCreateSkillRange)
            {
                Debug.Log("Summoned");
                float buriedDepth = 4.5f;
                
                Vector3 spawnPosition = hit.point - hit.normal * buriedDepth;
                GameObject skill = Instantiate(
                    skillPrefab,
                    spawnPosition,
                    Quaternion.identity
                );


                PillarRiseAnimation pillar = skill.GetComponent<PillarRiseAnimation>();

                if (pillar != null)
                {
                    pillar.SetSpawnSurface(hit.point, hit.normal);
                }
                
            }

        }
    }

    // keep it for now im lazy to do delegate
    private void TrySummonSkill1()
    {
        TrySummon(skillPrefab);
    }
    
    private void TrySummonSkill2()
    {
        TrySummon(skillPrefabLaunch);
    }

    private void OnEnable()
    {
        inputManager.OnSkillPressed += TrySummonSkill1;
        inputManager.OnSkill2Pressed += TrySummonSkill2;
    }



    private void OnDisable()
    {
        inputManager.OnSkillPressed -= TrySummonSkill1;
        inputManager.OnSkill2Pressed -= TrySummonSkill2;
    }
}
