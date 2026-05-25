using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 光线反射视图 - MVC 视图层（渲染+射线）
/// </summary>
public class LightRefractionView : MonoBehaviour
{
    [Header("视图配置")]
    public LineRenderer lineRenderer;

    private Color originalColor;
    private bool isFading = false;

    void Awake()
    {
        if (lineRenderer != null)
            originalColor = lineRenderer.startColor;
    }

    public void DrawReflectionRay(Transform lightSource, List<GameObject> reflectObjects, List<GameObject> hitOrder = null)
    {
        if (isFading) return;

        if (lightSource == null || reflectObjects.Count == 0 || lineRenderer == null) return;

        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, lightSource.position);

        Vector3 firstTarget = reflectObjects[0].transform.position;
        Vector3 startDir = (firstTarget - lightSource.position).normalized;
        Ray ray = new Ray(lightSource.position, startDir);

        List<Transform> hitList = new List<Transform>();
        float fixedY = lightSource.position.y;

        while (true)
        {
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.collider.isTrigger)
                {
                    ray = new Ray(hit.point + ray.direction * 0.01f, ray.direction);
                    continue;
                }

                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, hit.point);

                if (reflectObjects.Contains(hit.collider.gameObject) && !hitList.Contains(hit.transform))
                {
                    hitList.Add(hit.transform);
                    hitOrder?.Add(hit.collider.gameObject);

                    Vector3 reflectDir = Vector3.Reflect(ray.direction.normalized, hit.normal);
                    reflectDir.y = 0;
                    reflectDir.Normalize();

                    ray = new Ray(hit.point, reflectDir);
                }
                else
                {
                    break;
                }
            }
            else
            {
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, ray.origin + ray.direction * 50);
                break;
            }
        }
    }

    // 缓慢消失
    public void FadeOutLine(float duration)
    {
        if (lineRenderer == null) return;
        StopAllCoroutines();
        StartCoroutine(FadeRoutine(duration));
    }

    private IEnumerator FadeRoutine(float duration)
    {
        isFading = true;
        float elapsed = 0;
        Color startColor = lineRenderer.startColor;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float a = Mathf.Lerp(startColor.a, 0, elapsed / duration);
            Color c = startColor;
            c.a = a;
            lineRenderer.startColor = c;
            lineRenderer.endColor = c;
            yield return null;
        }

        lineRenderer.startColor = originalColor;
        lineRenderer.endColor = originalColor;
        ClearRay();
        isFading = false;
    }

    public void ClearRay()
    {
        if (lineRenderer != null)
            lineRenderer.positionCount = 0;
    }
}