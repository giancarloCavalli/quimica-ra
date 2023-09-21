using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

public class onCollision : MonoBehaviour
{
  private Transform mainCameraTransform;

  private GameObject objToSpawn;

  private bool shouldCreateObjOnCollide = true;

  private bool otherShouldApproximate;
  void Start()
  {
    mainCameraTransform = GameObject.FindWithTag("MainCamera").transform;
  }

  void Update()
  {
    if (objToSpawn != null && otherShouldApproximate)
    {
      //https://docs.unity3d.com/ScriptReference/Vector3.MoveTowards.html
      var step = 0.08f * Time.deltaTime;
      objToSpawn.transform.position = Vector3.MoveTowards(objToSpawn.transform.position, transform.position, step);

      if (Vector3.Distance(objToSpawn.transform.position, transform.position) <= 0.0225)
      {
        otherShouldApproximate = false;
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
      objToSpawn = Instantiate(newObj, other.transform.position, transform.rotation, GameObject.FindWithTag("OxigenioTarget").transform);
    }
  }

  private void OnTriggerExit(Collider other)
  {
    GameObject.Destroy(objToSpawn, 0f);
    shouldCreateObjOnCollide = true;

    other.gameObject.GetComponent<Renderer>().enabled = true;
  }
}
