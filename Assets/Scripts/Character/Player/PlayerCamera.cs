using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera instance;
    public PlayerManager player;
    public Camera cameraObject;
    [SerializeField] Transform cameraPivotTransform;

    //change these to tweak camera peformance
    [Header("Camera Setting")]
    private float cameraSmoothSpeed = 15f; // the bigger this number, the longer for the camera to reach its position during movement
    [SerializeField] float leftAndRightRotationSpeed = 220;
    [SerializeField] float upAndDownRotationSpeed = 220;
    [SerializeField] float minimumPivot = -30; // the lowest point are able to look down
    [SerializeField] float maximumPivot = 60; // the highest point are able to look up
    [SerializeField] float cameraCollisionsRadius = 0.2f;
    [SerializeField] LayerMask colliderWithLayers;

    [Header ("Camera Value")]
    private Vector3 cameraVelocity;
    private Vector3 cameraObjectPosition;
    [SerializeField] float leftAndRightLookAngle;
    [SerializeField] float upAndDownLookAngle;
    private float cameraZPosition; // values uses for camera collisions
    private float targetCameraZPosition; // values uses for camera collisions

    [Header("Lock On")]
    [SerializeField] float lockOnRadius = 20f;
    [SerializeField] float miniumViewableAngle = -50;
    [SerializeField] float maxiumViewableAngle = 50;
    [SerializeField] float unlockedCameraHeight = 1.65f;
    [SerializeField] float lockedCameraHeight = 2.0f;
    [SerializeField] float setCameraHeightSpeed = 1;
    private List<CharacterManager> availableTargets = new List<CharacterManager>();
    public CharacterManager nearestLockOnTarget;
    public CharacterManager leftLockOnTarget;
    public CharacterManager rightLockOnTarget;
    [SerializeField] float lockOnTargetFollowSpeed = 0.2f;
    private Coroutine cameraLockOnHeightCoroutine;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        cameraZPosition = cameraObject.transform.localPosition.z;
    }
    public void HandleAllCameraActions()
    {
        if(player != null)
        {
            HandleFollowTarget();
            HandleRotation();
            HandleCollisions();
        }
    }
    private void HandleFollowTarget()
    {
        Vector3 targetCameraPosition = Vector3.SmoothDamp(transform.position, player.transform.position, ref cameraVelocity, cameraSmoothSpeed * Time.deltaTime);
        transform.position = targetCameraPosition;
    }
    private void HandleRotation()
    {
        // if lock on, force rotation toward target
        if(player.playerNetworkManager.isLockedOn.Value)
        {
            // Main camera object this rotates this gameobject
            Vector3 rotationDirection = player.playerCombatManager.currentTarget.characterCombatManager.lockOnTransform.position - transform.position;
            rotationDirection.Normalize();
            rotationDirection.y = 0;

            Quaternion targetRotation = Quaternion.LookRotation(rotationDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lockOnTargetFollowSpeed);

            // this rotates the pivot object
            rotationDirection = player.playerCombatManager.currentTarget.characterCombatManager.lockOnTransform.position - cameraPivotTransform.position;
            rotationDirection.Normalize();

            targetRotation = Quaternion.LookRotation(rotationDirection);
            cameraPivotTransform.transform.rotation = Quaternion.Slerp(cameraPivotTransform.rotation, targetRotation, lockOnTargetFollowSpeed);

            // save my rotation to my look angle
            leftAndRightLookAngle = transform.eulerAngles.y;
            upAndDownLookAngle = transform.eulerAngles.x;
        }
        // else regularly
        else
        {
            // rotate left and right based on horizontal movement on the right side
            leftAndRightLookAngle += (PlayerInputManager.instance.cameraHorizontalInput * leftAndRightRotationSpeed) * Time.deltaTime;
            // rotate up and down right based on vertical movement on the right side
            upAndDownLookAngle -= (PlayerInputManager.instance.cameraVerticalInput * upAndDownRotationSpeed) * Time.deltaTime;
            // clamp the up and down look angle between a min and max value
            upAndDownLookAngle = Mathf.Clamp(upAndDownLookAngle, minimumPivot, maximumPivot);

            Vector3 cameraRotation = Vector3.zero;
            Quaternion targetRotation;
            // rotate this gameobject left and right
            cameraRotation.y = leftAndRightLookAngle;
            targetRotation = Quaternion.Euler(cameraRotation);
            transform.rotation = targetRotation;

            // rotate the pivot gameobject up and down
            cameraRotation = Vector3.zero;
            cameraRotation.x = upAndDownLookAngle;
            targetRotation = Quaternion.Euler(cameraRotation);
            cameraPivotTransform.localRotation = targetRotation;
        }
    }
    private void HandleCollisions()
    {
        targetCameraZPosition = cameraZPosition;
        
        RaycastHit hit;

        // direction for collision check
        Vector3 direction = cameraObject.transform.position - cameraPivotTransform.position;
        direction.Normalize();

        // we check if there is an object in front of my desired direction
        if(Physics.SphereCast(cameraPivotTransform.position, cameraCollisionsRadius, direction, out hit, Mathf.Abs(targetCameraZPosition), colliderWithLayers))
        {
            //if there is, get distance from it
            float distanceFromHitObject = Vector3.Distance(cameraPivotTransform.position, hit.point);
            // equate my target z position to the following
            targetCameraZPosition = -(distanceFromHitObject - cameraCollisionsRadius);
        }

        // iff target position is less than my collision radius, make a subtract our collision radius(make it snap back)
        if(Mathf.Abs(targetCameraZPosition) < cameraCollisionsRadius)
        {
            targetCameraZPosition = -cameraCollisionsRadius;
        }

        // then apply a final position using lerp over a time of 0.2f
        cameraObjectPosition.z = Mathf.Lerp(cameraObject.transform.localPosition.z, targetCameraZPosition, 0.2f);
        cameraObject.transform.localPosition = cameraObjectPosition;
    }
    public void HandleLocatingLockOnTargets()
    {
        float shortestDistance = Mathf.Infinity; // determine the target close to me
        float shortestDistanceOfRightTarget = Mathf.Infinity; // will be used to determine shortest distance on 1 axis to the right target of current target (+)
        float shortestDistanceOfLeftTarget = -Mathf.Infinity; // will be used to determine shortest distance on 1 axis to the left target of current target (-)

        Collider[] collider = Physics.OverlapSphere(player.transform.position, lockOnRadius,
            WorldUtilityManager.Instance.getCharacterLayer());

        for(int i = 0; i < collider.Length; i++)
        {
            CharacterManager lockOnTarget = collider[i].GetComponent<CharacterManager>();

            if(lockOnTarget != null)
            {
                // check if they are within my field of view
                Vector3 lockOnTargetsDirection =  lockOnTarget.transform.position - player.transform.position;
                float distanceFromTarget = Vector3.Distance(player.transform.position, lockOnTarget.transform.position);
                float viewableAngle = Vector3.Angle(lockOnTargetsDirection, cameraObject.transform.forward);

                // if target dead, check next potential target
                if (lockOnTarget.isDead.Value)
                    continue;

                if(lockOnTarget.transform.root == player.transform.root)
                    continue;

                if (lockOnTarget.characterCombatManager.lockOnTransform == null)
                {
                    Debug.LogError("lockOnTransform chýa ðý?c gán trong CharacterCombatManager!");
                    return;
                }

                // lastly if target is outside field of view or is block by enviro, check next potential target
                if (viewableAngle > miniumViewableAngle && viewableAngle < maxiumViewableAngle)
                {
                    RaycastHit hit;

                    if (Physics.Linecast(player.playerCombatManager.lockOnTransform.position,
                        lockOnTarget.characterCombatManager.lockOnTransform.position,
                        out hit, WorldUtilityManager.Instance.getEnviroLayer()))
                    {
                        continue;
                    }
                    else
                    {
                        Debug.Log("lock on made it"); 
                        availableTargets.Add(lockOnTarget);
                    }
                }
            }
        }

        for(int k = 0; k < availableTargets.Count; k++)
        {
            if(availableTargets[k] != null)
            {
                float distanceFromTarget = Vector3.Distance(player.transform.position, availableTargets[k].transform.position);
                

                if(distanceFromTarget < shortestDistance)
                {
                    shortestDistance = distanceFromTarget;
                    nearestLockOnTarget = availableTargets[k];
                }

                if(player.playerNetworkManager.isLockedOn.Value)
                {
                    Vector3 relativeEnemyPosition = player.transform.InverseTransformPoint(availableTargets[k].transform.position);

                    var distanceFormLeftTarget = relativeEnemyPosition.x;
                    var distanceFromRightTarget = relativeEnemyPosition.x;

                    if (availableTargets[k] == player.playerCombatManager.currentTarget)
                        continue;

                    // check left side for target
                    if (relativeEnemyPosition.x <= 0.00 && distanceFormLeftTarget > shortestDistanceOfLeftTarget)
                    {
                        shortestDistanceOfLeftTarget = distanceFormLeftTarget;
                        leftLockOnTarget = availableTargets[k];
                    }
                    // check right side for target
                    else if(relativeEnemyPosition.x >= 0.00 && distanceFromRightTarget < shortestDistanceOfRightTarget)
                    {
                        shortestDistanceOfRightTarget = distanceFromRightTarget;
                        rightLockOnTarget = availableTargets[k];
                    }
                }
            }
            else
            {
                ClearLockOnTargets();
                player.playerNetworkManager.isLockedOn.Value = false;
            }
        }
    }
    public void SetLockOnCameraTarget()
    {
        if(cameraLockOnHeightCoroutine != null)
        {
            StopCoroutine(cameraLockOnHeightCoroutine);
        }

        cameraLockOnHeightCoroutine = StartCoroutine(SetCameraHeight());
    }
    public void ClearLockOnTargets()
    {
        nearestLockOnTarget = null;
        leftLockOnTarget = null;
        rightLockOnTarget = null;
        availableTargets.Clear();
    }
    public IEnumerator WaitThenFindNewTarget()
    {
        while(player.isPerformingAction)
        {
            yield return null;
        }
        ClearLockOnTargets();
        HandleLocatingLockOnTargets();

        if(nearestLockOnTarget != null)
        {
            player.playerCombatManager.SetTarget(nearestLockOnTarget);
            player.playerNetworkManager.isLockedOn.Value = true;
        }

        yield return null;
    }
    private IEnumerator SetCameraHeight()
    {
        float duration = 1;
        float timer = 0;

        Vector3 velocity = Vector3.zero;
        Vector3 newLockedCameraHeight = new Vector3(cameraPivotTransform.transform.localPosition.x, lockedCameraHeight);
        Vector3 newUnlockedCameraHeight = new Vector3(cameraPivotTransform.transform.localPosition.x, unlockedCameraHeight);

        while(timer < duration)
        {
            timer += Time.deltaTime;
            
            if(player != null)
            {
                if(player.playerCombatManager.currentTarget != null)
                {
                    cameraPivotTransform.transform.localPosition = 
                        Vector3.SmoothDamp(cameraPivotTransform.transform.localPosition,newLockedCameraHeight, ref velocity, setCameraHeightSpeed);

                    cameraPivotTransform.transform.localRotation =
                        Quaternion.Slerp(cameraPivotTransform.transform.localRotation, Quaternion.Euler(0, 0, 0), lockOnTargetFollowSpeed);
                }
                else
                {
                    cameraPivotTransform.transform.localPosition =
                        Vector3.SmoothDamp(cameraPivotTransform.transform.localPosition, newUnlockedCameraHeight, ref velocity, setCameraHeightSpeed);
                }
            }

            yield return null;
        }

        if(player != null)
        {
            if(player.playerCombatManager.currentTarget != null)
            {
                cameraPivotTransform.transform.localPosition = newLockedCameraHeight;

                cameraPivotTransform.transform.localRotation = Quaternion.Euler(0,0,0);
            }
            else
            {
                cameraPivotTransform.transform.localPosition = newUnlockedCameraHeight;
            }
        }

        yield return null;
    }
}
