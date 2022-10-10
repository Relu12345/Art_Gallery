using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Net;
using Intendix.Board;
using Unity.ItemRecever;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
public class charController : MonoBehaviour
{

    public static string ips = "127.0.0.1";
    public static int port = 1000;
    public static IPAddress ip = IPAddress.Parse(ips);
    public static Vector3 dir;

    private bool seMisca;
	private Vector3 origine, target;
	private float timp_miscare = 1.5f;

    void Update()
	{
        if (dir != Vector3.zero)
        {
            if (!seMisca)
                StartCoroutine(Misca(dir));
        }
    }

    void Start()
    {
        connection(ip, port);
        dir = Vector3.zero;
    }

    void connection(IPAddress ip, int port)
    {
        // Connection with Unicorn BCI
        try
        {
            //Start listening for Unicorn Speller network messages
            SpellerReceiver r = new SpellerReceiver(ip, port);

            //attach items received event
            r.OnItemReceived += OnItemReceived;

            Debug.Log(String.Format("Listening to {0} on port {1}.", ip, port));
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }

    }

    private void OnItemReceived(object sender, EventArgs args)
    {
        ItemReceivedEventArgs eventArgs = (ItemReceivedEventArgs)args;
        Debug.Log(String.Format("Received BoardItem:\tName: {0}\tOutput Text: {1}", eventArgs.BoardItem.Name, eventArgs.BoardItem.OutputText));
        if (eventArgs.BoardItem.OutputText == "Left")
        {
            Debug.Log("left!!!!!!!!!!!!!!!!!1");
            dir = Vector3.right;
        }
        if (eventArgs.BoardItem.OutputText == "Right")
        {
            Debug.Log("right!!!!!!!!!!!!!!!!!1");
            dir = Vector3.left;
        }
        if (eventArgs.BoardItem.OutputText == "Up")
        {
            Debug.Log("up!!!!!!!!!!!!!!!!!1");
            dir = Vector3.back;
        }
        if (eventArgs.BoardItem.OutputText == "Down")
        {
            Debug.Log("down!!!!!!!!!!!!!!!!!1");
            dir = Vector3.forward;
        }
    }

        private IEnumerator Misca(Vector3 directie)
    {
        seMisca = true;

        float timp_trecut = 0;
        bool inFata;
        if (transform.position.z == -30)
            inFata = false;
        else
            inFata = true;
        origine = transform.position;
        if (directie == Vector3.forward && transform.position.z == -32)
            target = origine + directie * 2;
        else if (directie == Vector3.back && transform.position.z == -30)
            target = origine + directie * 2;
        else if (directie == Vector3.left && !inFata)
        {
            if (transform.position.x > -115)
                target = origine + directie * 5;
            else
                target = new Vector3(20, 1.6f, -30);
        }
        else if (directie == Vector3.right && !inFata)
        {
            if (transform.position.x < 20)
                target = origine + directie * 5;
            else
                target = new Vector3(-115, 1.6f, -30);
        }
        while (timp_trecut < timp_miscare)
        {
            transform.position = Vector3.Lerp(origine, target, (timp_trecut / timp_miscare));
            timp_trecut += Time.deltaTime;
            yield return null;
        }

        transform.position = target;
        dir = Vector3.zero;
        seMisca = false;
    }
}
