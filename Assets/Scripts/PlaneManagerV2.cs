using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Vuforia;
using System.Collections;

public class PlaneManagerV2 : MonoBehaviour
{
    #region PUBLIC_MEMBERS

    public PlaneFinderBehaviour m_PlaneFinder;
    public GameObject m_PlacementPreview, m_PlacementAugmentation;
    public Text m_TitleMode, m_OnScreenMessage;
    public Button m_ResetButton;
    public CanvasGroup m_ScreenReticleGround;
    public GameObject m_RotationIndicator;
    public GameObject m_TranslationIndicator;
    public Transform m_Floor;

    [Range(0.1f, 2.0f)]
    public float ProductSize = 0.65f;

    #endregion //PUBLIC_MEMBERS

    #region PRIVATE_MEMBERS

    const string AppTitle = "Zero Hunger";
    const string unsuportedDeviceTitle = "Unsuported Device";
    const string unsuportedDeviceBody =
        "This device has failed to start the Positional Device Tracker. " +
        "Please check the list of supported Ground Plane devices on our site: " +
        "\n\nhttps://library.vuforia.com/articles/Solution/ground-plane-supported-devices.html";
    const string EmulatorGroundPlane = "Emulator Ground Plane";

    StateManager m_StateManager;
    SmartTerrain m_SmartTerrain;
    PositionalDeviceTracker m_PositionalDeviceTracker;
    TouchHandler m_TouchHandler;

    GameObject m_PlacementAnchor;

    float m_PlacementAugmentationScale;
    int AutomaticHitTestFrameCount;
    int m_AnchorCounter;
    Vector3 ProductScaleVector;

    GraphicRaycaster m_GraphicRayCaster;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;

    Camera mainCamera;
    Ray cameraPlaneRay;
    RaycastHit cameraToPlaneHit;

    #endregion //PRIVATE_MEMBERS

    // Use this for initialization
    void Start()
    {
        Debug.Log("Start() called.");
        VuforiaARController.Instance.RegisterVuforiaStartedCallback(OnVuforiaStarted);
        VuforiaARController.Instance.RegisterOnPauseCallback(OnVuforiaPaused);
        DeviceTrackerARController.Instance.RegisterTrackerStartedCallback(OnTrackerStarted);
        DeviceTrackerARController.Instance.RegisterDevicePoseStatusChangedCallback(OnDevicePoseStatusChanged);

        m_PlaneFinder.HitTestMode = HitTestMode.AUTOMATIC;

        m_PlacementAugmentationScale = VuforiaRuntimeUtilities.IsPlayMode() ? 0.1f : ProductSize;
        ProductScaleVector =
            new Vector3(m_PlacementAugmentationScale,
                        m_PlacementAugmentationScale,
                        m_PlacementAugmentationScale);
        m_PlacementPreview.transform.localScale = ProductScaleVector;
        m_PlacementAugmentation.transform.localScale = ProductScaleVector;

		m_ResetButton.interactable = true;

        // Enable floor collider if running on device; Disable if running in PlayMode
        m_Floor.gameObject.SetActive(!VuforiaRuntimeUtilities.IsPlayMode());

        m_TitleMode.text = AppTitle;

        mainCamera = Camera.main;

        m_TouchHandler = FindObjectOfType<TouchHandler>();
        m_GraphicRayCaster = FindObjectOfType<GraphicRaycaster>();
        m_EventSystem = FindObjectOfType<EventSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_PlacementAugmentation.activeInHierarchy)
        {
            m_RotationIndicator.SetActive(Input.touchCount == 2);
            m_TranslationIndicator.SetActive(TouchHandler.IsSingleFingerDragging());
            if (TouchHandler.IsSingleFingerDragging() || (VuforiaRuntimeUtilities.IsPlayMode() && Input.GetMouseButton(0)))
            {
                if (!IsCanvasButtonPressed())
                {
                    cameraPlaneRay = mainCamera.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(cameraPlaneRay, out cameraToPlaneHit))
                    {
                        if (cameraToPlaneHit.collider.gameObject.name == (VuforiaRuntimeUtilities.IsPlayMode() ? EmulatorGroundPlane : m_Floor.name))
                        {
                            m_PlacementAugmentation.PositionAt(cameraToPlaneHit.point);
                        }
                    }
                }
            }
        }
    }

    void LateUpdate()
    {
        if (AutomaticHitTestFrameCount == Time.frameCount)
        {
            m_ScreenReticleGround.alpha = 0;

            SetSurfaceIndicatorVisible(true);
            m_OnScreenMessage.transform.parent.gameObject.SetActive(true);
            m_OnScreenMessage.enabled = true;

            m_OnScreenMessage.text = "Tap to place Chair";
        }
        else
        {
            m_ScreenReticleGround.alpha = 1;
            SetSurfaceIndicatorVisible(false);
            m_OnScreenMessage.transform.parent.gameObject.SetActive(true);
            m_OnScreenMessage.enabled = true;

            m_PlacementPreview.SetActive(false);
            m_OnScreenMessage.text = "Point device towards ground";
        }
    }


    #region GROUNDPLANE_CALLBACKS

    public void HandleAutomaticHitTest(HitTestResult result)
    {
        AutomaticHitTestFrameCount = Time.frameCount;
        SetSurfaceIndicatorVisible(true);
        m_PlacementPreview.SetActive(true);
        m_PlacementPreview.PositionAt(result.Position);
    }

    public void HandleInteractiveHitTest(HitTestResult result)
    {
        // If the PlaneFinderBehaviour's Mode is Automatic, then the Interactive HitTestResult will be centered.

        Debug.Log("HandleInteractiveHitTest() called.");

        m_PlacementPreview.SetActive(false);

        if (result == null)
        {
            Debug.LogError("Invalid Hit Test.");
            return;
        }


        if (m_PositionalDeviceTracker != null && m_PositionalDeviceTracker.IsActive)
        {
            if (m_PlacementAnchor == null || TouchHandler.DoubleTap)
            {
                DestroyAnchors();

                m_PlacementAnchor = m_PositionalDeviceTracker.CreatePlaneAnchor("MyPlacementAnchor_" + (++m_AnchorCounter), result);
                m_PlacementAnchor.name = "PlacementAnchor";

                if (!VuforiaRuntimeUtilities.IsPlayMode())
                {
                    m_Floor.position = m_PlacementAnchor.transform.position;
                }
                m_PlacementAugmentation.transform.SetParent(m_PlacementAnchor.transform);
                m_PlacementAugmentation.transform.localPosition = Vector3.zero;
            }

            if (!m_PlacementAugmentation.activeInHierarchy)
            {
                Debug.Log("Setting Placement Augmentation to Active");
                // On initial placement, unhide the augmentation
                m_PlacementAugmentation.SetActive(true);

                Debug.Log("Positioning Placement Augmentation at: " + result.Position);
                // parent the augmentation to the anchor
                m_PlacementAugmentation.transform.SetParent(m_PlacementAnchor.transform);
                m_PlacementAugmentation.transform.localPosition = Vector3.zero;
                RotateTowardCamera(m_PlacementAugmentation);
                m_TouchHandler.enableRotation = true;
            }
        }
    }

    #endregion // GROUPPLANE_CALLBACKS

    #region PUBLIC_BUTTON_METHODS

    public void SetPlacementMode(bool active)
    {
        if (active)
        {
            m_PlaneFinder.gameObject.SetActive(true);
            m_TouchHandler.enableRotation = m_PlacementAugmentation.activeInHierarchy;
        }
    }

    public void ResetScene()
    {
        Debug.Log("ResetScene() called.");
        m_PlacementAugmentation.transform.position = Vector3.zero;
        m_PlacementAugmentation.transform.localEulerAngles = Vector3.zero;
        m_PlacementAugmentation.transform.localScale = ProductScaleVector;
        m_PlacementAugmentation.SetActive(false);

        m_ResetButton.interactable = true;
        m_TouchHandler.enableRotation = false;
    }

    public void ResetTrackers()
    {
        Debug.Log("ResetTrackets() Called.");

        m_SmartTerrain = TrackerManager.Instance.GetTracker<SmartTerrain>();
        m_PositionalDeviceTracker = TrackerManager.Instance.GetTracker<PositionalDeviceTracker>();


        m_SmartTerrain.Stop();
        m_PositionalDeviceTracker.Stop();
        m_PositionalDeviceTracker.Start();
        m_SmartTerrain.Start();
    }

    #endregion // PUBLIC_BUTTON_METHODS

    #region PRIVATE_METHODS

    void DestroyAnchors()
    {
        if (!VuforiaRuntimeUtilities.IsPlayMode())
        {
            IEnumerable<TrackableBehaviour> trackableBehaviours = m_StateManager.GetActiveTrackableBehaviours();

            string destroyed = "Destroying: ";

            foreach (TrackableBehaviour behaviour in trackableBehaviours)
            {
                Debug.Log(behaviour.name +
                          "\n" + behaviour.Trackable.Name +
                          "\n" + behaviour.Trackable.ID +
                          "\n" + behaviour.GetType());

                if (behaviour is AnchorBehaviour)
                {
                    // First determine which mode (Plane or MidAir) and then delete only the anchors for that mode
                    // Leave the other mode's anchors intact
                    // PlaneAnchor_<GUID>
                    // Mid AirAnchor_<GUID>


                    destroyed +=
                        "\nGObj Name: " + behaviour.name +
                        "\nTrackable Name: " + behaviour.Trackable.Name +
                        "\nTrackable ID: " + behaviour.Trackable.ID +
                        "\nPosition: " + behaviour.transform.position.ToString();

                    m_StateManager.DestroyTrackableBehavioursForTrackable(behaviour.Trackable);
                    m_StateManager.ReassociateTrackables();

                }
            }

            Debug.Log(destroyed);
        }
        else
        {
            m_PlacementAugmentation.transform.parent = null;
            DestroyObject(m_PlacementAnchor);
        }

    }

    void SetSurfaceIndicatorVisible(bool isVisible)
    {
        Renderer[] renderers = m_PlaneFinder.PlaneIndicator.GetComponentsInChildren<Renderer>(true);
        Canvas[] canvas = m_PlaneFinder.PlaneIndicator.GetComponentsInChildren<Canvas>(true);

        foreach (Canvas c in canvas)
            c.enabled = isVisible;

        foreach (Renderer r in renderers)
            r.enabled = isVisible;
    }

    void RotateTowardCamera(GameObject augmentation)
    {
        var lookAtPosition = mainCamera.transform.position - augmentation.transform.position;
        lookAtPosition.y = 0;
        var rotation = Quaternion.LookRotation(lookAtPosition);
        augmentation.transform.rotation = rotation;
    }

    bool IsCanvasButtonPressed()
    {
        m_PointerEventData = new PointerEventData(m_EventSystem)
        {
            position = Input.mousePosition
        };
        List<RaycastResult> results = new List<RaycastResult>();
        m_GraphicRayCaster.Raycast(m_PointerEventData, results);

        bool resultIsButton = false;
        foreach (RaycastResult result in results)
        {
            if (result.gameObject.GetComponentInParent<Toggle>() ||
                result.gameObject.GetComponent<Button>())
            {
                resultIsButton = true;
                break;
            }
        }
        return resultIsButton;
    }

    #endregion //PRIVATE_METHODS

    #region VUFORIA_CALLBACKS

    void OnVuforiaStarted()
    {
        Debug.Log("OnVuforiaStarted() called.");

        m_StateManager = TrackerManager.Instance.GetStateManager();

        // Check trackers to see if started and start if necessary
        m_PositionalDeviceTracker = TrackerManager.Instance.GetTracker<PositionalDeviceTracker>();
        m_SmartTerrain = TrackerManager.Instance.GetTracker<SmartTerrain>();

        if (m_PositionalDeviceTracker != null && m_SmartTerrain != null)
        {
            if (!m_PositionalDeviceTracker.IsActive)
                m_PositionalDeviceTracker.Start();
            if (m_PositionalDeviceTracker.IsActive && !m_SmartTerrain.IsActive)
                m_SmartTerrain.Start();
        }
        else
        {
            if (m_PositionalDeviceTracker == null)
                Debug.Log("PositionalDeviceTracker returned null. GroundPlane not supported on this device.");
            if (m_SmartTerrain == null)
                Debug.Log("SmartTerrain returned null. GroundPlane not supported on this device.");

            MessageBox.DisplayMessageBox(unsuportedDeviceTitle, unsuportedDeviceBody, false, null);
        }
    }

    void OnVuforiaPaused(bool paused)
    {
        Debug.Log("OnVuforiaPaused(" + paused.ToString() + ") called.");

        //if (paused)
            //ResetScene();
    }

    #endregion //VUFORIA_CALLBACKS

    #region DEVICE_TRACKER_CALLBACKS

    void OnTrackerStarted()
    {
        Debug.Log("OnTrackerStarted() called.");

        m_PositionalDeviceTracker = TrackerManager.Instance.GetTracker<PositionalDeviceTracker>();
        m_SmartTerrain = TrackerManager.Instance.GetTracker<SmartTerrain>();

        if (m_PositionalDeviceTracker != null)
        {
            if (!m_PositionalDeviceTracker.IsActive)
                m_PositionalDeviceTracker.Start();

            Debug.Log("PositionalDeviceTracker is Active?: " + m_PositionalDeviceTracker.IsActive +
                      "\nSmartTerrain Tracker is Active?: " + m_SmartTerrain.IsActive);
        }
    }

    void OnDevicePoseStatusChanged(TrackableBehaviour.Status status)
    {
        Debug.Log("OnDevicePoseStatusChanged(" + status.ToString() + ")");
    }

    #endregion // DEVICE_TRACKER_CALLBACK_METHODS
}
