using UnityEngine;
using Vuforia;

public class TrackingInterceptor : MonoBehaviour
{
  public ImageTargetBehaviour HidrogenioTarget;

  // Start is called before the first frame update
  void Start()
  {
    if (HidrogenioTarget != null)
    {
      HidrogenioTarget.OnTargetStatusChanged += OnTargetStatusChanged;
    }
  }

  private void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
  {
    GameObject imageTargetChild = HidrogenioTarget.transform.GetChild(0).gameObject;
    if (status.Status == Status.TRACKED)
    {
      Debug.Log($"MY TAG IS {imageTargetChild.tag}");
      foreach (Transform child in GameObject.FindWithTag("OxigenioTarget").transform)
      {
        if (imageTargetChild.tag == child.name)
        {
          Debug.Log($"Found clone. Hiding {imageTargetChild.tag}");
          imageTargetChild.GetComponent<Renderer>().enabled = false;
        }
      }
    }
  }

  // Update is called once per frame

  void Update()
  { }
}
