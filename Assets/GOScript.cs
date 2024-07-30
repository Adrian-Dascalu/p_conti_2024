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

public class GOScript : MonoBehaviour
{
    public string text1;

    public TextMeshPro textMesh;

    private ARMarker QRCode;

    private ARMarker rb;

    private bool qrCodeCoordinatesCorected = false;

    private bool qrCodeAdded = false;
    
    bool delayForQRCodeTextStarted = false;
    bool delayForQRCodeCoordinatesStarted = false;
    bool delayForQRCodeIDStarted = false;

    [SerializeField]
    GameObject quad;

    /*[SerializeField]
    private ARMarker m_marker;

    [SerializeField]
    private MeshRenderer m_markerRenderer;*/

    //private string m_text = "Marker text";
    //private const int m_countToUpdateFrame = 10;
    //private int m_countUntilNextUpdate = 0;
    /*
    public string UpdateText()
    {
        if(m_marker != null && m_countUntilNextUpdate-- <= 0)
        {
            m_countUntilNextUpdate = m_countToUpdateFrame;

            m_text = $"{m_marker.trackableId} {m_marker.trackingState}\n" +
                     $"DurationSinceLastSeen (s): {Math.Round(Time.realtimeSinceStartup - m_marker.lastSeenTime, 2)}\n " +
                     $"{Math.Round(m_marker.lastSeenTime, 2)}";
        }

        return m_text;
    }*/

    public void SlateClosed()
    {
        Debug.Log("Slate closed");

        qrCodeCoordinatesCorected = false;

        qrCodeAdded = false;
    }

    void OnQRCodesChanged(ARMarkersChangedEventArgs args)
    {
        foreach (ARMarker qrCode in args.added)
        {
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
            }

            rb = qrCode;

            qrCodeCoordinatesCorected = false;

            qrCodeAdded = true;
        }

        foreach(ARMarker qrCode in args.updated)
        {
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

            if (!qrCodeCoordinatesCorected)
            {
                if (!(rb.transform.position.x - qrCode.transform.position.x < 0.2 && rb.transform.position.x - qrCode.transform.position.x > -0.2))
                {
                    quad.SetActive(true);

                    quad.transform.position = qrCode.transform.position;

                    Debug.Log("QR code x position: " + qrCode.transform.position.x);

                    qrCodeCoordinatesCorected = true;
                }
                else if (!(rb.transform.position.y - qrCode.transform.position.y < 0.2 && rb.transform.position.y - qrCode.transform.position.y > -0.2))
                {
                    quad.SetActive(true);

                    quad.transform.position = qrCode.transform.position;

                    Debug.Log("QR code y position: " + qrCode.transform.position.y);

                    qrCodeCoordinatesCorected = true;
                }
                else if (!(rb.transform.position.z - qrCode.transform.position.z < 0.2 && rb.transform.position.z - qrCode.transform.position.z > -0.2))
                {
                    quad.SetActive(true);

                    quad.transform.position = qrCode.transform.position;

                    Debug.Log("QR code z position: " + qrCode.transform.position.z);
                    
                    qrCodeCoordinatesCorected = true;
                }
                
                rb = qrCode;
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
        Debug.Log("QR code ID: " + QRCode.trackableId);

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
        if (qrCodeAdded)
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
        }

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