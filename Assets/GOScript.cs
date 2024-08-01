using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.OpenXR.NativeTypes;

using MixedReality.Toolkit;
using MixedReality.Toolkit.UX;
using MixedReality.Toolkit.Input;
using MixedReality.Toolkit.Subsystems;
using MixedReality.Toolkit.SpatialManipulation;

using Microsoft.MixedReality.OpenXR;
using UnityEngine.UIElements;
using TMPro;
using Unity.VisualScripting;

public class GOScript : MonoBehaviour
{
    public string text1;

    public TextMeshPro textMesh;

    private ARMarker QRCode;

    private ARMarker rb;

    private bool[] qrCodeCoordinatesCorected = { false };

    private bool[] qrCodeAdded = { false };

    public int qrCodeID;

    private Dictionary<int, GameObject> qrCodeSlates = new Dictionary<int, GameObject>();

    GameObject[] newSlate;

    bool delayForQRCodeTextStarted = false;
    bool delayForQRCodeCoordinatesStarted = false;
    bool delayForQRCodeIDStarted = false;

    [SerializeField]
    GameObject quad;

    private bool[] qrCodeSlatesActive = { false };

    public void SlateClosed()
    {
        Debug.Log("Slate closed");

        qrCodeCoordinatesCorected[qrCodeID] = false;

        qrCodeAdded[qrCodeID] = false;

        qrCodeSlatesActive[qrCodeID] = false;

        qrCodeSlates.Remove(qrCodeID);
    }

    void OnQRCodesChanged(ARMarkersChangedEventArgs args)
    {
        foreach (ARMarker qrCode in args.added)
        {
            if(qrCode.GetInstanceID() < 0) qrCodeID = -qrCode.GetInstanceID();
            else qrCodeID = qrCode.GetInstanceID();
            
            text1 = qrCode.GetDecodedString();
            var text = qrCode.GetDecodedString();
            Debug.Log($"QR code text: {text}");

            var bytes = qrCode.GetRawData(Unity.Collections.Allocator.Temp);

            Debug.Log($"QR code bytes: {bytes.Length}");
            bytes.Dispose();

            if(qrCode.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Tracking)
            {
                Debug.Log($"Created Track qr with id {qrCode.trackableId}");
                Debug.Log($"Created QR code properties: {qrCode.GetQRCodeProperties()}");
                Debug.Log($"Created QR code coordinates: { qrCode.transform.position}");

                if(!qrCodeSlates.ContainsKey(qrCodeID))
                {
                    GameObject slate = GameObject.Instantiate(quad, qrCode.transform.position, qrCode.transform.rotation);
                    qrCodeSlates.Add(qrCodeID, slate);
                    //qrCodeSlates[qrCodeID] = slate;

                    qrCodeSlates[qrCodeID].SetActive(true);
                    slate.SetActive(true);

                    //newSlate[qrCode.GetInstanceID()] = GameObject.Instantiate(quad, qrCode.transform.position, qrCode.transform.rotation);
                    //newSlate[qrCode.GetInstanceID()].SetActive(true);

                    //slate.GetComponentInChildren<TextMeshPro>().text = text;

                    if(qrCode.GetInstanceID() < 0) qrCodeID = -qrCode.GetInstanceID();
                    else qrCodeID = qrCode.GetInstanceID();

                    Debug.Log("Created slate for qr code with id: " + qrCodeID);
                }
                else
                {
                    qrCodeSlates[qrCodeID].SetActive(true);

                    Debug.Log("Set slate for qr code with id: " + qrCodeID);
                }
            }

            rb = qrCode;

            qrCodeCoordinatesCorected[qrCodeID] = false;

            qrCodeAdded[qrCodeID] = true;
        }

        foreach(ARMarker qrCode in args.updated)
        {
            if(qrCode.GetInstanceID() < 0) qrCodeID = -qrCode.GetInstanceID();
            else qrCodeID = qrCode.GetInstanceID();

            QRCode = qrCode;

            text1 = qrCode.GetDecodedString();
            var text = qrCode.GetDecodedString();
            Debug.Log($"Updated QR code text: {text}");

            textMesh.text = text;

            var bytes = qrCode.GetRawData(Unity.Collections.Allocator.Temp);

            Debug.Log($"Updated QR code bytes: {bytes.Length}");
            bytes.Dispose();

            if(qrCode.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Tracking)
            {
                Debug.Log($"Updated Track qr with id {qrCode.trackableId}");
                Debug.Log($"Updated QR code properties: {qrCode.GetQRCodeProperties()}");
                Debug.Log($"Updated QR code coordinates: { qrCode.transform.position}");
            }

            Debug.Log("rb.transform.position.x - qrCode.transform.position.x: " + (rb.transform.position.x - qrCode.transform.position.x));

            if (!qrCodeCoordinatesCorected[qrCodeID])
            {
                if (!(rb.transform.position.x - qrCode.transform.position.x < 0.2 && rb.transform.position.x - qrCode.transform.position.x > -0.2))
                {
                    quad.SetActive(true);

                    quad.transform.position = qrCode.transform.position;

                    Debug.Log("QR code x position: " + qrCode.transform.position.x);

                    qrCodeCoordinatesCorected[qrCodeID] = true;
                }
                else if (!(rb.transform.position.y - qrCode.transform.position.y < 0.2 && rb.transform.position.y - qrCode.transform.position.y > -0.2))
                {
                    quad.SetActive(true);

                    quad.transform.position = qrCode.transform.position;

                    Debug.Log("QR code y position: " + qrCode.transform.position.y);

                    qrCodeCoordinatesCorected[qrCodeID] = true;
                }
                else if (!(rb.transform.position.z - qrCode.transform.position.z < 0.2 && rb.transform.position.z - qrCode.transform.position.z > -0.2))
                {
                    quad.SetActive(true);

                    quad.transform.position = qrCode.transform.position;

                    Debug.Log("QR code z position: " + qrCode.transform.position.z);
                    
                    qrCodeCoordinatesCorected[qrCodeID] = true;
                }
                
                rb = qrCode;
                
                if (qrCodeSlatesActive[qrCodeID] == false)
                {
                    qrCodeSlates[qrCodeID].SetActive(true);
                }
            }

            if (qrCodeSlatesActive[qrCodeID] == false)
            {
                qrCodeSlates[qrCodeID].SetActive(true);
            }
        }
    }

    private void Awake()
    {
        ARMarkerManager aRMarkerManager = FindObjectOfType<ARMarkerManager>();
        aRMarkerManager.markersChanged += OnQRCodesChanged;
    }

    // Start is called before the first frame update
    void Start()
    {
        ARMarkerManager aRMarkerManager = FindObjectOfType<ARMarkerManager>();
        aRMarkerManager.markersChanged += OnQRCodesChanged;

        ARMarkerManager.Instance.markersChanged += OnQRCodesChanged;

        XRInputSubsystem inputSubsystem = null;
        var inputSubsystems = new List<XRInputSubsystem>();
        SubsystemManager.GetInstances(inputSubsystems);

        if (inputSubsystems.Count > 0)
        {
            inputSubsystem = inputSubsystems[0];
        }

        if (inputSubsystem != null)
        {
            inputSubsystem.TrySetTrackingOriginMode(TrackingOriginModeFlags.Unbounded);
            inputSubsystem.TrySetTrackingOriginMode(TrackingOriginModeFlags.Device);
            inputSubsystem.GetSupportedTrackingOriginModes();
            UnboundedTrackingMode unboundedTrackingMode = new UnboundedTrackingMode();
        }
    }

    void OnDestroy()
    {
        if (ARMarkerManager.Instance != null)
        {
            ARMarkerManager.Instance.markersChanged -= OnQRCodesChanged;
        }
    }

    public void PrintQRCodeText()
    {
        Debug.Log("QR code text: " + text1);

        delayForQRCodeTextStarted = false;
    }

    public void PrintQRCodeID()
    {
        Debug.Log("QR code ID: " + qrCodeID);

        delayForQRCodeIDStarted = false;
    }

    public void PrintQRCodeCoordinates()
    {
        Debug.Log("QR code coordinates: " + QRCode.transform.position);

        delayForQRCodeCoordinatesStarted = false;
    }

    // Update is called once per frame
    void Update()
    {
        //quad.transform.position = GameObject.Find("GameObject").transform.position;

        //quad.SetActive(true);
        //show the text in the console

        /*if (qrCodeAdded[qrCodeID])
        {
            if (!delayForQRCodeTextStarted)
            {
                delayForQRCodeTextStarted = true;

                CallAfterDelay.Create(2.0f, PrintQRCodeText);
            }

            if (!delayForQRCodeIDStarted)
            {
                delayForQRCodeIDStarted = true;

                CallAfterDelay.Create(2.0f, PrintQRCodeID);
            }

            //show the qr code coordinates in the console
            if (!delayForQRCodeCoordinatesStarted)
            {
                delayForQRCodeCoordinatesStarted = true;

                CallAfterDelay.Create(2.0f, PrintQRCodeCoordinates);
            }
        }*/

        /*if (m_marker != null && m_markerRenderer != null)
        {
            if (m_marker.trackingState != TrackingState.Tracking)
            {
                m_markerRenderer.material.color = Color.gray;
            }
            else
            {
                m_markerRenderer.material.color = Color.blue;
            }
        }*/

        //show the text in the scene

        //GameObject.Find("Main Text").GetComponent<TextMesh>().text = text1;
    }
}