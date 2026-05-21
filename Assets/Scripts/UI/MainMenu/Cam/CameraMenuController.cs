using System;
using System.Collections;
using UnityEngine;

public class CameraMenuController : MonoBehaviour
{
    public static CameraMenuController Instance;

    [Header("Camera")]
    public Camera menuCamera;
    public float lerpSpeed = 2f;

    [Header("Camera Positions")]
    public Transform posHome;
    public Transform posLevelSelect;
    public Transform posOptions;
    public Transform posCredits;

    public event Action OnArriveHome;
    public event Action OnArriveLevelSelect;
    public event Action OnArriveOptions;
    public event Action OnArriveCredits;

    private Coroutine moveCoroutine;
    public bool IsMoving { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        GoHome(instant: true);
    }

    public void GoToLevelSelect()
    {
        if (IsMoving) return;
        Move(posLevelSelect, OnArriveLevelSelect);
    }

    public void GoToOptions()
    {
        if (IsMoving) return;
        Move(posOptions, OnArriveOptions);
    }

    public void GoToCredits()
    {
        if (IsMoving) return;
        Move(posCredits, OnArriveCredits);
    }

    public void GoHome(bool instant = false)
    {
        if (instant)
        {
            menuCamera.transform.SetPositionAndRotation(posHome.position, posHome.rotation);
            OnArriveHome?.Invoke();
            return;
        }

        if (IsMoving) return;
        Move(posHome, OnArriveHome);
    }

    void Move(Transform target, Action onArrive)
    {
        MenuManager.Instance.CloseAllPanels();
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(LerpCamera(target, onArrive));
    }

    IEnumerator LerpCamera(Transform target, Action onArrive)
    {
        IsMoving = true;

        Vector3 startPos = menuCamera.transform.position;
        Quaternion startRot = menuCamera.transform.rotation;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * lerpSpeed;
            float smooth = Mathf.SmoothStep(0f, 1f, t);

            menuCamera.transform.position = Vector3.Lerp(startPos, target.position, smooth);
            menuCamera.transform.rotation = Quaternion.Lerp(startRot, target.rotation, smooth);

            yield return null;
        }

        menuCamera.transform.SetPositionAndRotation(target.position, target.rotation);
        IsMoving = false;
        onArrive?.Invoke();
    }
}