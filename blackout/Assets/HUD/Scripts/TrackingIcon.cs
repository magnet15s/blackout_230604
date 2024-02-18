using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class TrackingIcon : MonoBehaviour
{
    public RectTransform canvas;
    public static TrackingIcon closestIconToCenter = null;
    public static List<TrackingIcon> Icons = new();

    public Image image;
    public GameObject trackingTarget;
    public GameObject player;
    private RectTransform rectTransform;
    public float trackingUpdateInterval = 0.3f;
    private float TUICnt = 0;
    private Vector3 trackingPoint;
    public Color iconColor = Color.white;
    public bool overrideClosestIconToCenter = true;
    public bool selfDestroyWithTarget = false;

    [SerializeField] Vector2 trackBounsOnScreen = new Vector2(0.9f, 0.9f);

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        trackingPoint = player.transform.position - trackingTarget.transform.position;
        TrackTarget();
        Icons.Add(this);
    }

    // Update is called once per frame
    void Update()
    {

        if (selfDestroyWithTarget && !trackingTarget) Destroy(this.gameObject);

        if(TrackingIcon.closestIconToCenter != null)
        {
            if(new Vector2(TrackingIcon.closestIconToCenter.rectTransform.localPosition.x, TrackingIcon.closestIconToCenter.rectTransform.localPosition.y).magnitude > 
                new Vector2(rectTransform.localPosition.x, rectTransform.localPosition.y).magnitude)
            {
               if(overrideClosestIconToCenter) TrackingIcon.closestIconToCenter = this;
            }

        }
        else
        {
            if (overrideClosestIconToCenter) TrackingIcon.closestIconToCenter = this;
        }


        TUICnt -= Time.deltaTime;
        
        //Debug.Log(rectTransform.localPosition);
        if (TUICnt < 0)
        {
            TUICnt = trackingUpdateInterval;
            if (trackingTarget != null)
                trackingPoint = player.transform.position - trackingTarget.transform.position;

        }

        TrackTarget();

        if (Vector3.Dot(trackingPoint.normalized, player.transform.forward) < 0)
        {
            if (!overrideClosestIconToCenter || (TrackingIcon.closestIconToCenter != null && TrackingIcon.closestIconToCenter.Equals(this)))
            {
                image.color = iconColor;
            }
            else
            {
                image.color = iconColor * 0.3f;
            }

        }
        else
        {
            if(overrideClosestIconToCenter) image.color = Color.black;
        }

        //Debug.Log(Vector3.Dot((trackingTarget.transform.position - player.transform.position).normalized, player.transform.forward));

    }

    private void OnDestroy()
    {
        if (closestIconToCenter != null && TrackingIcon.closestIconToCenter == this) closestIconToCenter = null;
        Icons.Remove(this);
        
    }

    private void TrackTarget()
    {
        Vector3 rlp = Camera.main.WorldToScreenPoint(player.transform.position + trackingPoint) - new Vector3(Screen.width / 2, Screen.height / 2, 0);
        float sxLim = (Screen.width / 2) * trackBounsOnScreen.x;
        float syLim = (Screen.height / 2) * trackBounsOnScreen.y;
        if (Mathf.Abs(rlp.x) > sxLim) rlp.x = sxLim * Mathf.Sign(rlp.x);
        if (Mathf.Abs(rlp.y) > syLim) rlp.y = syLim * Mathf.Sign(rlp.y);
        if (Vector3.Dot((trackingPoint.normalized), player.transform.forward) > 0)
        {
            rlp.x = sxLim * -Mathf.Sign(rlp.x);
            rlp.y = -rlp.y;
        }
        
        rectTransform.localPosition = new Vector3(rlp.x, rlp.y, 0);
    }

    
}
