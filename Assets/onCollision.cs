using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;

public class OnCollision : MonoBehaviour
{
  private Transform MainCameraTransform;

  private readonly float ProximityTriggerDistance = 0.3f;

  private readonly Dictionary<string, GameObject> HidrogensByName = new();

  private readonly Dictionary<string, AtomCommand> CommandByHidrogenName = new();

  private readonly Queue DestroyHidrogenQueue = new();
  void Start()
  {
    MainCameraTransform = GameObject.FindWithTag("MainCamera").transform;
  }

  // TODO - feat -  on camera proximity
  // TODO - fix - better handling of hidrogens that are already in the molecule (molecule breaks when getting far from untracked Target)
  // TODO - refactor - create command ENUM
  // TODO - style - easy-out transition when approximating
  // TODO - style - change border when forming molecule
  // TODO - style - make atoms go to opposite poles when forming molecules
  void Update()
  {
    lock (HidrogensByName)
    {
      // refactor - make a queue of Maps (string hidrogenName, Commmand command) for commands and iterate through it
      foreach (GameObject hidrogen in HidrogensByName.Values)
      {
        // Debug.Log($"Executing {CommandByHidrogenName[hidrogen.name]} for hidrogen {hidrogen.name}");
        HandleCommand(CommandByHidrogenName[hidrogen.name], hidrogen);
      }

      while (DestroyHidrogenQueue.Count > 0)
      {
        string hidrogenName = (string)DestroyHidrogenQueue.Dequeue();
        GameObject.FindWithTag(hidrogenName).GetComponent<Renderer>().enabled = true;
        GameObject hidrogen = HidrogensByName[hidrogenName];
        HidrogensByName.Remove(hidrogenName);
        Destroy(hidrogen, 0f);
      }
    }

    // if (mainCameraTransform && otherShouldReturnToOriginalPosition == false)
    // {
    //   if (Vector3.Distance(transform.position, mainCameraTransform.position) <= ProximityTriggerDistance)
    //   {
    //     GetComponent<Renderer>().material.color = Color.blue;
    //   }
    //   else
    //   {
    //     GetComponent<Renderer>().material.color = Color.white;
    //   }
    // }
  }

  private void OnTriggerEnter(Collider other)
  {
    if (other != null && !other.gameObject.CompareTag("Untagged") && !HidrogensByName.ContainsKey(other.gameObject.tag))
    {
      HidrogensByName.Add(other.gameObject.tag, IntantiateNewSphere(other.transform.position, other.gameObject.tag, GameObject.FindWithTag("OxigenioTarget").transform));
      other.GetComponent<Renderer>().enabled = false;
      CommandByHidrogenName[other.gameObject.tag] = AtomCommand.MoveToBond;
    }
  }

  private void OnTriggerExit(Collider other)
  {
    CommandByHidrogenName[other.gameObject.tag] = AtomCommand.MoveToTarget;
  }

  private GameObject IntantiateNewSphere(Vector3 position, string name, Transform parent)
  {
    GameObject sphereModel = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    sphereModel.GetComponent<Renderer>().material.color = Color.green;
    sphereModel.transform.localScale = new Vector3(0.015f, 0.015f, 0.015f);
    GameObject sphere = Instantiate(sphereModel, position, transform.rotation, parent);
    sphere.name = name;

    Destroy(sphereModel, 0f);

    return sphere;
  }

  private bool ShouldApproximateCustomHidrogen(GameObject customHidrogen)
  {
    return Vector3.Distance(customHidrogen.transform.position, transform.position) >= 0.0225;
  }

  private void ApproximateTo(GameObject @object, Vector3 toPosition)
  {
    var step = 0.08f * Time.deltaTime;
    @object.transform.position = Vector3.MoveTowards(@object.transform.position, toPosition, step);
  }

  private bool HasCustomHidrogenReachedImageTarget(GameObject @object)
  {
    return Vector3.Distance(@object.transform.position, GameObject.FindWithTag(@object.name).transform.position) <= 0;
  }

  private void HandleCommand(AtomCommand command, GameObject hidrogen)
  {
    switch (command)
    {
      case AtomCommand.MoveToBond:
        ApproximateTo(hidrogen, transform.position);
        if (!ShouldApproximateCustomHidrogen(hidrogen))
        {
          CommandByHidrogenName[hidrogen.name] = AtomCommand.Steady;
        }
        break;
      case AtomCommand.MoveToTarget:
        Vector3 imageTargetObjectPosition = GameObject.FindWithTag(hidrogen.name).transform.position;
        ApproximateTo(hidrogen, imageTargetObjectPosition);
        if (HasCustomHidrogenReachedImageTarget(hidrogen))
        {
          CommandByHidrogenName[hidrogen.name] = AtomCommand.QueueToDestroy;
        }
        break;
      case AtomCommand.QueueToDestroy:
        DestroyHidrogenQueue.Enqueue(hidrogen.name);
        break;
    }
  }
}
