using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

public class OnCollision : MonoBehaviour
{
  private Transform mainCameraTransform;

  private GameObject newAtomObj;

  private bool shouldCreateObjOnCollide = true;

  private bool otherShouldApproximate;

  private bool otherShouldReturnToOriginalPosition;
  void Start()
  {
    mainCameraTransform = GameObject.FindWithTag("MainCamera").transform;
  }

  void Update()
  {
    if (newAtomObj != null && otherShouldApproximate)
    {
      //https://docs.unity3d.com/ScriptReference/Vector3.MoveTowards.html
      var step = 0.08f * Time.deltaTime;
      newAtomObj.transform.position = Vector3.MoveTowards(newAtomObj.transform.position, transform.position, step);

      if (Vector3.Distance(newAtomObj.transform.position, transform.position) <= 0.0225)
      {
        otherShouldApproximate = false;
      }
    }
    else if (newAtomObj != null && otherShouldReturnToOriginalPosition)
    {
      var step = 0.1f * Time.deltaTime;
      newAtomObj.transform.position = Vector3.MoveTowards(newAtomObj.transform.position, GameObject.FindWithTag("HidrogenObj").transform.position, step);

      if (Vector3.Distance(newAtomObj.transform.position, GameObject.FindWithTag("HidrogenObj").transform.position) <= 0)
      {
        otherShouldReturnToOriginalPosition = false;
        GameObject.FindWithTag("HidrogenObj").GetComponent<Renderer>().enabled = true;
        GameObject.Destroy(newAtomObj, 0f);
        shouldCreateObjOnCollide = true;
      }
    }
  }

  private void OnTriggerEnter(Collider other)
  {
    if (shouldCreateObjOnCollide)
    {
      GameObject newObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
      newObj.GetComponent<Renderer>().material.color = Color.green;
      newObj.transform.localScale = new Vector3(0.015f, 0.015f, 0.015f);

      shouldCreateObjOnCollide = false;
      otherShouldApproximate = true;

      other.gameObject.GetComponent<Renderer>().enabled = false;
      newAtomObj = Instantiate(newObj, other.transform.position, transform.rotation, GameObject.FindWithTag("OxigenioTarget").transform);
      GameObject.Destroy(newObj, 0f);
    }
  }

  private void OnTriggerExit(Collider other)
  {
    otherShouldReturnToOriginalPosition = true;
  }
}
