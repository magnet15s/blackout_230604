using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        trackingPoint = player.transform.position - trackingTarget.transform.position;
        Icons.Add(this);
    }

    // Update is called once per frame
    void Update()
    {

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
        
        Vector3 rlp = Camera.main.WorldToScreenPoint(player.transform.position + trackingPoint) - new Vector3(Screen.width / 2, Screen.height / 2, 0);

        rectTransform.localPosition = new Vector3(rlp.x, rlp.y, 0); 
        //Debug.Log(rectTransform.localPosition);
        if (TUICnt < 0)
        {
            TUICnt = trackingUpdateInterval;
            if (trackingTarget != null)
                trackingPoint = player.transform.position - trackingTarget.transform.position;

        }
        if(Vector3.Dot((trackingTarget.transform.position - player.transform.position).normalized, player.transform.forward) >= 0)
        {
            if (overrideClosestIconToCenter || (TrackingIcon.closestIconToCenter != null && TrackingIcon.closestIconToCenter.Equals(this)))
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
            image.color = Color.black;
        }

        //Debug.Log(Vector3.Dot((trackingTarget.transform.position - player.transform.position).normalized, player.transform.forward));

    }

    private void OnDestroy()
    {
        if (closestIconToCenter != null && TrackingIcon.closestIconToCenter == this) closestIconToCenter = null;
        Icons.Remove(this);
        
    }
}
