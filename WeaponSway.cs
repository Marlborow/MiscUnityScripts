using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("External GameObjects")]
    public Camera cam;

    //when rotationg
    [Header("Sway Settings")]
    [SerializeField] public float smooth;
    [SerializeField] public float multiplier;

    //when moving
    [Header("ViewBobbing Settings")]
    [SerializeField] public float bobSpeed = 1f;
    [SerializeField] public float bobDistance = 1f;
    private float horizontal, vertical, timer, waveSlice;
    private Vector3 midPoint;


    //when weapon is close to walls or when cooling off
    [Header("Weapon Dynamics Settings")]
    [SerializeField] public float maxDistanceToWall;
    [SerializeField] public float DistanceStep = .02f;
    [SerializeField] public float maxDistanceToMoveBack;

    //when shooting
    [Header("Weapon Recoil Settings")]
	public Transform recoilPosition;
    public Transform rotationPoint;
    public float positionalRecoilSpeed = 8f;
    public float rotationalRecoilSpeed = 8f;
    public float positionalReturnSpeed = 18f;
    public float rotationalReturnSpeed = 38f;
    public Vector3 RecoilRotation = new Vector3(10, 5, 7);
    public Vector3 RecoilKickBack = new Vector3(0.015f, 0f, -0.2f);
    public Vector3 RecoilRotationAim = new Vector3(10, 4, 6);
    public Vector3 RecoilKickBackAim = new Vector3(0.015f, 0f, -0.2f);

    Vector3 rotationalRecoil;
    Vector3 positionalRecoil;
    Vector3 Rot;

    private Vector3 localPosition;

    private Vector3 previousPosition;
    private Vector3 currentPosition;
    private float currentDistance;
    public bool Holster = false;

    private void Start()
    {
        cam = Camera.main;
        localPosition = transform.localPosition;

        currentPosition = transform.position;
        previousPosition = transform.position;

        currentDistance = localPosition.z;
    }

    private void Update()
    {

        float mouseX = Input.GetAxisRaw("Mouse X") * multiplier - 90f; // - 90f = forward, 0f = down
        float mouseY = Input.GetAxisRaw("Mouse Y") * multiplier;

        //VIEW BOBBING
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        if (Mathf.Abs(horizontal) == 0 && Mathf.Abs(vertical) == 0)
        {
            timer = 0.0f;
        }
        else
        {
            waveSlice = Mathf.Sin(timer);
            timer = timer + bobSpeed * Time.deltaTime;
            if (timer > Mathf.PI * 2)
            {
                timer = timer - (Mathf.PI * 2);
            }
        }
        if (waveSlice != 0 && currentPosition != previousPosition)
        {

            float translateChange = waveSlice * bobDistance;
            float totalAxes = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
            totalAxes = Mathf.Clamp(totalAxes, 0.0f, 1.0f);
            translateChange = totalAxes * translateChange;
            localPosition.y = midPoint.y + translateChange;
            localPosition.x = midPoint.x + translateChange * 2;
        }

        //WEAPON SWAY
        Quaternion rotationX = Quaternion.AngleAxis(mouseY, Vector3.left);
        Quaternion rotationY = Quaternion.AngleAxis(mouseX + 90, Vector3.up);

        //WEAPON HIDE NEAR WALL/ RELOAD
        Vector3 rayOrigin = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
        RaycastHit hit;
        if (Holster || Physics.Raycast(rayOrigin, cam.transform.forward, out hit, maxDistanceToWall))
        {
            if (transform.localPosition.z > (localPosition.z - maxDistanceToMoveBack))
            {
                rotationX = Quaternion.AngleAxis(mouseY - 20f, Vector3.left);
                rotationY = Quaternion.AngleAxis(mouseX + 90, Vector3.up);
                currentDistance -= DistanceStep * Time.deltaTime;
            }
            else if (transform.localPosition.z < (localPosition.z - maxDistanceToMoveBack)) currentDistance = (localPosition.z - maxDistanceToMoveBack);
        }
        else
        {
            if (transform.localPosition.z < localPosition.z) currentDistance += DistanceStep * Time.deltaTime;
            else if (transform.localPosition.z > localPosition.z) currentDistance = localPosition.z;
        }

        //WEAPON RECOIL
        rotationalRecoil = Vector3.Lerp(rotationalRecoil, Vector3.zero, rotationalReturnSpeed * Time.deltaTime);
        positionalRecoil = Vector3.Lerp(positionalRecoil, Vector3.zero, positionalReturnSpeed * Time.deltaTime);

        transform.localPosition = Vector3.Slerp(transform.localPosition, positionalRecoil, positionalRecoilSpeed * Time.deltaTime);
        Rot = Vector3.Slerp(Rot, rotationalRecoil, rotationalRecoilSpeed * Time.deltaTime);

        Quaternion targetRotation = rotationX * rotationY;
        targetRotation *= Quaternion.Euler(Rot);

        Quaternion targetRot2 = Quaternion.Slerp(targetRotation, Quaternion.Euler(Rot), rotationalRecoilSpeed * Time.deltaTime);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRot2, smooth * Time.deltaTime);
        transform.localPosition = localPosition;


        previousPosition = currentPosition;
        currentPosition = transform.position;
    }
    //WEAPON RECOIL
    public void Shoot()
    {
        rotationalRecoil += new Vector3(-RecoilRotation.x, Random.Range(-RecoilRotation.y, RecoilRotation.y), Random.Range(-RecoilRotation.z, RecoilRotation.z));
        rotationalRecoil += new Vector3(Random.Range(-RecoilKickBack.x, RecoilKickBack.x), Random.Range(-RecoilKickBack.y, RecoilKickBack.y), RecoilKickBack.z);
    }
}
