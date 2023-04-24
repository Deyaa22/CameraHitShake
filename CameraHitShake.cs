using System.Collections;
using UnityEngine;

public class CameraHitShake : MonoBehaviour {

    // If you like to improve it, Or create a pull request for it. 
    // public enum DampingStyle { Lerp, Linear, Smooth }
    // public DampingStyle m_DampingStyle = DampingStyle.Lerp;

    [HideInInspector] public Vector3 originalPos;
    [HideInInspector] public Quaternion originalRot;
    private Coroutine currentShake = null;
    public bool isShaking { get { return currentShake != null; } }

    // Transform of the camera to make Hit Shake.
    [Header ("Camera Transform")]
    [Tooltip ("Assign local transform automatically on Awake")]
    [SerializeField] private Transform camTransform;

    // The Power of the Hit Shake.
    [Header ("Hit Shake Power")]
    public float powerMultiplier = 1f;
    [SerializeField][Range (0f, 50f)] private float _PositionShakePower = 2f;
    [SerializeField][Range (0f, 360f)] private float _RotationShakePower = 15f;

    // Speed of camera to get back to original Poition and Rotation.
    [Header ("Damping Speed")]
    [SerializeField][Range (0f, 10f)] private float _PositionDampingSpeed = 10;
    [SerializeField][Range (0f, 10f)] private float _RotationDampingSpeed = 10;

    // Minimum distance and orientation that not effect if changed.
    private float minDistance = 0.05f;
    private float minOrientation = 0.5f;

    private void Awake () {
        if (camTransform == null)
            camTransform = this.transform;

        originalPos = camTransform.localPosition;
        originalRot = camTransform.localRotation;
    }

    /// <summary>
    /// Play the Hit Shake of Camera transform.
    /// </summary>
    /// <param name="forced">Stop previous Hit Shake and start new one.</param>
    public void Play (bool forced = true) {
        if (!camTransform)
            return;

        if (isShaking) {
            if (!forced)
                return;
            else
                StopCoroutine (currentShake);
        } else {
            originalPos = camTransform.localPosition;
            originalRot = camTransform.localRotation;
        }
        currentShake = StartCoroutine (Shake ());
    }
    private IEnumerator Shake () {
        camTransform.localPosition = originalPos + Random.insideUnitSphere * _PositionShakePower;
        camTransform.localRotation = originalRot * Quaternion.Euler (Random.insideUnitSphere * _RotationShakePower);
        while (camTransform.localPosition != originalPos || camTransform.localRotation != originalRot) {
            camTransform.localPosition = Vector3.Lerp (camTransform.localPosition, originalPos, _PositionDampingSpeed * powerMultiplier * Time.deltaTime);
            camTransform.localRotation = Quaternion.Slerp (camTransform.localRotation, originalRot, _RotationDampingSpeed * powerMultiplier * Time.deltaTime);
            if (Vector3.Distance (camTransform.localPosition, originalPos) <= minDistance && Quaternion.Angle (camTransform.localRotation, originalRot) <= minOrientation) {
                camTransform.localPosition = originalPos;
                camTransform.localRotation = originalRot;
                break;
            }
            yield return null;
        }
        currentShake = null;
    }
    public void Stop () {
        if (currentShake != null) {
            StopCoroutine (currentShake);
            currentShake = null;
        }
        camTransform.localPosition = originalPos;
        camTransform.localRotation = originalRot;
    }
}