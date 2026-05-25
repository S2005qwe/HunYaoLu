using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;
using UnityEngine.InputSystem;
using UnityEngine.Animations.Rigging;

public class ThirdPersonShooterController : MonoBehaviour
{

    [SerializeField] private Rig aimRig;
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField] private float normalSensitivity;
    [SerializeField] private float aimSensitivity;
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    //[SerializeField] private Transform debugTransform;
    [SerializeField] private Transform pfBulletProjectile;
    [SerializeField] private Transform spawnBulletPosition;
    [SerializeField] private Bowstring bowstring;
    [SerializeField] private GameObject Bow;
    [SerializeField] private GameObject Bow_bei;
    [SerializeField] private GameObject Arrow;
    //[SerializeField] private Transform vfxHitGreen;
    //[SerializeField] private Transform vfxHitRed;

    private ThirdPersonController thirdPersonController;
    private StarterAssetsInputs starterAssetsInputs;
    private Animator animator;
    private float aimRigWeight;
    private Vector3 mouseWorldPosition;

    private void Awake()
    {
        thirdPersonController = GetComponent<ThirdPersonController>();
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        aimRig.weight = Mathf.Lerp(aimRig.weight, aimRigWeight, Time.deltaTime * 20f);
        mouseWorldPosition = Vector3.zero;

        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        //Transform hitTransform = null;
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
        {
            //debugTransform.position = raycastHit.point;
            mouseWorldPosition = raycastHit.point;
            //hitTransform = raycastHit.transform;
        }

        if (starterAssetsInputs.aim)
        {
            animator.SetBool("Aiming", true);
            aimVirtualCamera.gameObject.SetActive(true);
            thirdPersonController.SetSensitivity(aimSensitivity);
            thirdPersonController.SetRotateOnMove(false);
            aimRig.weight = 1f;
            //animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1),1f,Time.deltaTime*10f));

            Vector3 worldAimTarget = mouseWorldPosition;
            worldAimTarget.y = transform.position.y;
            Vector3 aimDirection = (worldAimTarget - transform.position).normalized;

            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);

        }
        else
        {
            Apper(0);
            isHand(0);
            BowApper(0);
            animator.SetBool("Aiming", false);
            aimVirtualCamera.gameObject.SetActive(false);
            thirdPersonController.SetSensitivity(normalSensitivity);
            thirdPersonController.SetRotateOnMove(true);
            aimRig.weight = 0f;
            //animator.SetBool("Shooting", false);
            //animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 0f, Time.deltaTime * 10f));
        }
        if (starterAssetsInputs.shoot)
        {
            animator.SetBool("Shooting", true);
            //if(hitTransform !=null)
            //{
            //    //Hit something
            //    if (hitTransform.GetComponent<BulletTarget>() != null)
            //    {
            //        //Hit target
            //        Instantiate(vfxHitGreen, transform.position, Quaternion.identity);
            //    }
            //    else
            //    {
            //        //Hit something else
            //        Instantiate(vfxHitRed, transform.position, Quaternion.identity);
            //    }
            //}
            //Vector3 aimDir = (mouseWorldPosition - spawnBulletPosition.position).normalized;
            //Instantiate(pfBulletProjectile, spawnBulletPosition.position, Quaternion.LookRotation(aimDir, Vector3.up));
            //starterAssetsInputs.shoot = false;
        }
        else
        {
            animator.SetBool("Shooting", false);
        }

    }
    public void Apper(int isApper)
    {
        if (isApper == 1)
        {
            Arrow.SetActive(true);
        }
        else if (isApper == 0)
        {
            Arrow.SetActive(false);
            isHand(0);
        }
    }
    public void BowApper(int isBow)
    {
        if (isBow == 1)
        {
            Bow.SetActive(true);
            Bow_bei.SetActive(false);
        }
        else if (isBow == 0)
        {
            Bow.SetActive(false);
            Bow_bei.SetActive(true);
        }
    }

    public void isHand(int Push)
    {
        bowstring.InHand = Push;
    }

    public void point(int istarget)
    {
        if (istarget == 1)
        {
            if (mouseWorldPosition != Vector3.zero)
            {
                Vector3 aimDir = (mouseWorldPosition - spawnBulletPosition.position).normalized;
                Instantiate(pfBulletProjectile, spawnBulletPosition.position, Quaternion.LookRotation(aimDir, Vector3.up));
                starterAssetsInputs.shoot = false;
            }
        }
    }
}
