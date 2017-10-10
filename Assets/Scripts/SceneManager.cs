using UnityEngine;

//[ExecuteInEditMode]
[System.Serializable]

public class SceneManager : MonoBehaviour
{

    public enum ParticleMode
    {
        looping,
        oneshot,
    }

    public enum Level
    {
        none,
        basic,
    }

    // =================================	
    // Variables.
    // =================================

    public Transform cameraRotationTransform;
    public Transform cameraTranslationTransform;

    public Vector3 cameraLookAtPosition = new Vector3(0.0f, 3.0f, 0.0f);

    Vector3 targetCameraPosition;
    Vector3 targetCameraRotation;

    Vector3 cameraPositionStart;
    Vector3 cameraRotationStart;

    Vector2 input;

    // Because Euler angles wrap around 360, I use
    // a separate value to store the full rotation.

    Vector3 cameraRotation;

    public float cameraMoveAmount = 2.0f;
    public float cameraRotateAmount = 2.0f;

    public float cameraMoveSpeed = 12.0f;
    public float cameraRotationSpeed = 12.0f;

    public Vector2 cameraAngleLimits = new Vector2(-8.0f, 60.0f);

    public GameObject[] levels;
    public Level currentLevel = Level.basic;

    public ParticleMode particleMode = ParticleMode.looping;

    public bool lighting = true;
    public bool advancedRendering = true;

    public Camera mainCamera;

    public Camera postEffectsCamera;

    public MonoBehaviour[] mainCameraPostEffects;

    // =================================	
    // Functions.
    // =================================

    void Start()
    {

        cameraPositionStart = cameraTranslationTransform.localPosition;
        cameraRotationStart = cameraRotationTransform.localEulerAngles;

        resetCameraTransformTargets();

        setLighting(lighting);
        setAdvancedRendering(advancedRendering);

    }

    public void setLevel(Level level)
    {
        for (int i = 0; i < levels.Length; i++)
        {
            if (i == (int)level)
            {
                levels[i].SetActive(true);
            }
            else
            {
                levels[i].SetActive(false);
            }
        }

        currentLevel = level;
    }

    public void setLighting(bool value)
    {
        lighting = value;
    }

    // ...

    public void setAdvancedRendering(bool value)
    {
        advancedRendering = value;

        if (value)
        {
            QualitySettings.SetQualityLevel(32, true);

            mainCamera.renderingPath = RenderingPath.UsePlayerSettings;

        }
        else
        {
            QualitySettings.SetQualityLevel(0, true);

            mainCamera.renderingPath = RenderingPath.VertexLit;

        }

        for (int i = 0; i < mainCameraPostEffects.Length; i++)
        {
            if (mainCameraPostEffects[i])
            {
                mainCameraPostEffects[i].enabled = value;
            }
        }
    }

    // ...

    public static Vector3 dampVector3(Vector3 from, Vector3 to, float speed, float dt)
    {
        return Vector3.Lerp(from, to, 1.0f - Mathf.Exp(-speed * dt));
    }

    // ...

    void Update()
    {
        // ...

        input.x = Input.GetAxis("Horizontal");
        input.y = Input.GetAxis("Vertical");

        // Get targets.

        if (Input.GetKey(KeyCode.LeftShift))
        {
            targetCameraPosition.z += input.y * cameraMoveAmount;
        }
        else
        {

            targetCameraRotation.y += input.x * cameraRotateAmount;
            targetCameraRotation.x += input.y * cameraRotateAmount;

            targetCameraRotation.x = Mathf.Clamp(targetCameraRotation.x, cameraAngleLimits.x, cameraAngleLimits.y);
        }

        // Camera position.

        cameraTranslationTransform.localPosition = Vector3.Lerp(
            cameraTranslationTransform.localPosition, targetCameraPosition, Time.deltaTime * cameraMoveSpeed);

        // Camera container rotation.

        cameraRotation = dampVector3(
            cameraRotation, targetCameraRotation, cameraRotationSpeed, Time.deltaTime);

        cameraRotationTransform.localEulerAngles = cameraRotation;

        // Look at origin.

        cameraTranslationTransform.LookAt(cameraLookAtPosition);

        // Reset.

        if (Input.GetKeyDown(KeyCode.R))
        {
            resetCameraTransformTargets();
        }
    }

    void resetCameraTransformTargets()
    {
        targetCameraPosition = cameraPositionStart;
        targetCameraRotation = cameraRotationStart;
    }

}
