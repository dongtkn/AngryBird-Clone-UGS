using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class SlingShotHandler : MonoBehaviour
{
    [Header("Line Renderers")]
    [SerializeField] private LineRenderer leftLineRendered;
    [SerializeField] private LineRenderer rightLineRendered;

    [Header("Transform References")]
    [SerializeField] private Transform leftStartPosition;
    [SerializeField] private Transform rightStartPosition;
    [SerializeField] private Transform centerPosition;
    [SerializeField] private Transform idlePosition;
    [SerializeField] private Transform elasticTransform;

    [Header("Scripts")]
    [SerializeField] private SlingShotArea slingShotArea;
    [SerializeField] private CameraManager cameraManager;

    [Header("Slingshot Stats")]
    [SerializeField] private float maxDistance = 3.5f;
    [SerializeField] private float shotForce = 5f;
    [SerializeField] private float timeBetweenBirdRespawn = 2f;
    [SerializeField] private float elasticDivider = 1.2f;
 
    [Header("Bird")]
    [SerializeField] private AngryBirds angryBirdPrefab;
    [SerializeField] private float angryBirdPositionOffset = 2f;

    [Header("Sound")]
    [SerializeField] private AudioClip elasticPulledClip;
    [SerializeField] private AudioClip[] elasticReleasedClips;

    private Vector2 slingShotLinesPosition;
    private Vector2 direction;
    private Vector2 directionNormnalized;
    private bool clickedWithinArea;
    private bool birdOnSlingshot;
    private AngryBirds spawnAngryBird;
    private AudioSource audioSource;
    private Camera mainCamera; // Cache camera để tối ưu hiệu năng
    private WaitForSeconds respawnDelay;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        mainCamera = Camera.main;
        respawnDelay = new WaitForSeconds(timeBetweenBirdRespawn);
        HideLines();
        SpawnAngryBird();
    }

    private void Update()
    {

        if (InputManager.WasLeftMouseButtonPressed && slingShotArea.isWithinSlingShotArea())
        {
            clickedWithinArea = true;
            if (birdOnSlingshot)
            {
                SoundManager.instance.PlayClip(elasticPulledClip, audioSource);
                cameraManager.SwitchToFollowCam(spawnAngryBird.transform);
            }
        }

        if (InputManager.IsLeftMousePressed && clickedWithinArea && birdOnSlingshot)
        {
            DrawSlingShot();
            PositionRotateAngryBird();
        }

        if (InputManager.WasLeftMouseButtonReleased && clickedWithinArea && birdOnSlingshot)
        {
            clickedWithinArea = false;
            birdOnSlingshot = false;

            if (GameManager.instance.HasEnoughShots())
            {

                spawnAngryBird.LaunchBird(direction, shotForce);

                SoundManager.instance.PlayRandomClip(elasticReleasedClips, audioSource);

                GameManager.instance.UseShot();
                AnimateSlingShot();

                if (GameManager.instance.HasEnoughShots())
                {
                    StartCoroutine(SpawnAngryBirdAfterTime());

                }

            }

        }

    }

    #region SlingShot methods
    private void DrawSlingShot()
    {
        Vector3 mousePos = InputManager.MousePosition;
        mousePos.z = -mainCamera.transform.position.z; // Khóa Z theo vị trí Camera

        Vector3 touchPosition = mainCamera.ScreenToWorldPoint(mousePos);
        touchPosition.z = 0f;
        slingShotLinesPosition = centerPosition.position + Vector3.ClampMagnitude(touchPosition - centerPosition.position, maxDistance);

        SetLines(slingShotLinesPosition);

        direction = (Vector2)centerPosition.position - slingShotLinesPosition;
        directionNormnalized = direction.normalized;
    }

    private void SetLines(Vector2 position)
    {
        if (!leftLineRendered.enabled && !rightLineRendered.enabled)
        {
            leftLineRendered.enabled = true;
            rightLineRendered.enabled = true;
        }

        leftLineRendered.SetPosition(0, position);
        leftLineRendered.SetPosition(1, leftStartPosition.position);

        rightLineRendered.SetPosition(0, position);
        rightLineRendered.SetPosition(1, rightStartPosition.position);
    }

    private void HideLines()
    {
        leftLineRendered.enabled = false;
        rightLineRendered.enabled = false;
    }
    #endregion

    #region Angry bird methods
    private void SpawnAngryBird()
    {
        SetLines(idlePosition.position);

        Vector2 dir = (centerPosition.position - idlePosition.position).normalized;

        spawnAngryBird = Instantiate(angryBirdPrefab, idlePosition.position, Quaternion.identity);
        spawnAngryBird.transform.right = dir;

        birdOnSlingshot = true;
    }

    private void PositionRotateAngryBird()
    {
        // Giữ vị trí chim luôn ở trục Z = 0
        Vector3 birdPos = (Vector3)slingShotLinesPosition + (Vector3)directionNormnalized * angryBirdPositionOffset;
        birdPos.z = 0f;

        spawnAngryBird.transform.position = birdPos;
        spawnAngryBird.transform.right = directionNormnalized;
    }
    private IEnumerator SpawnAngryBirdAfterTime()
    {
        yield return respawnDelay;
        SpawnAngryBird();

        cameraManager.SwitchToIdleCam();

    }
    #endregion
    #region animate slingshot
    private void AnimateSlingShot()
    {
        elasticTransform.position = leftLineRendered.GetPosition(0);
        //tinh do dai day
        float dist = Vector2.Distance(elasticTransform.position, centerPosition.position);
        float time = dist / elasticDivider;
        //Phan tram luc keo
        //float pullPercentage = dist / maxDistance;
        //Do rung, quan tinh keo
        //float overshoot = Mathf.Lerp(0.5f, 3.5f, pullPercentage);
        //float vibration = Mathf.Lerp(0.8f, 0.2f, pullPercentage);

        elasticTransform.DOMove(centerPosition.position, time)
            .SetEase(Ease.OutElastic, 1f, 0.5f)
            .OnUpdate(() => {
                if (elasticTransform != null) SetLines(elasticTransform.position);
            })
            .SetLink(elasticTransform.gameObject);
    }
    #endregion
}