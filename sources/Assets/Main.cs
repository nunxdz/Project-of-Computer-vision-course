using Accord;
using Accord.Statistics.Distributions.Multivariate;
using Accord.Statistics.Models.Markov;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour {

    public static HiddenMarkovClassifier<MultivariateNormalDistribution, double[]> hmm;
    public static Database database;
    public static bool IsLoadDatabase = false;

    private List<Point> sequence = new List<Point>();
    private bool onSpace = false;
    public Text text;
    public Text text2;

    private GameObject table;
    private GameObject tv;
    private GameObject sofa;
    private GameObject shelf;
    private GameObject book;
    private GameObject lamp;
    private GameObject bin;
    private GameObject micro;
    private GameObject refrig;
    private GameObject sink;
    private GameObject currentFocus = null;
    private bool capturing;
    private Vector3 oriTranTablel;
    private Vector3 oriTranTV;
    private Vector3 oriTranSofa;
    private Vector3 oriTranShelf;
    private Vector3 oriTranBook;
    private Vector3 oriTranLamp;
    private Vector3 oriTranBin;
    private Vector3 oriTranMicro;
    private Vector3 oriTranRefrig;
    private Vector3 oriTranSink;
    private bool isWantRealease = false;

    // Use this for initialization
    void Start () {
        //Debug.Log("Log1");
        //Debug.Log("Log2");
        //Debug.Log("Log3");
        //Debug.Log("Log4");
        database = new Database();

        table = GameObject.Find("Table");
        tv = GameObject.Find("TV");
        sofa = GameObject.Find("Sofa");
        shelf = GameObject.Find("Shelf");
        book = GameObject.Find("Book");
        lamp = GameObject.Find("Lamp");
        bin = GameObject.Find("Bin");
        micro = GameObject.Find("Microwave");
        refrig = GameObject.Find("Refrigerator");
        sink = GameObject.Find("Sink");
        oriTranTablel = table.transform.position;
        oriTranTV = tv.transform.position;
        oriTranSofa = sofa.transform.position;
        oriTranShelf = shelf.transform.position;
        oriTranBook = book.transform.position;
        oriTranLamp = lamp.transform.position;
        oriTranBin = bin.transform.position;
        oriTranMicro = micro.transform.position;
        oriTranRefrig = refrig.transform.position;
        oriTranSink = sink.transform.position;
    }

    void OnGUI()
    {
        Event e = Event.current;
        int controlID = GUIUtility.GetControlID(FocusType.Passive);
        switch (e.GetTypeForControl(controlID))
        {
            case EventType.MouseDown:
                capturing = true;
                e.Use();
                break;
            case EventType.MouseUp:
                capturing = false;
                isWantRealease = false;
                Preprocess();
                e.Use();
                break;
            case EventType.MouseDrag:
            case EventType.MouseMove:
                if (capturing)
                {
                    Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);

                    sequence.Add(new Point(Mathf.Abs(r.GetPoint(0).x * 100), Mathf.Abs(r.GetPoint(0).y * 100)));

                    Debug.Log("X = " + Mathf.Abs(r.GetPoint(0).x * 1000) + "      Y = " + Mathf.Abs(r.GetPoint(0).y*1000));
                }
                e.Use();
                break;
        }
    }

    // Update is called once per frame
    void Update () {
        if (currentFocus != null && !isWantRealease)
        {
            Vector3 temp = Input.mousePosition;
            temp.z = 10f; // Set this to be the distance you want the object to be placed in front of the camera.
            currentFocus.transform.position = Camera.main.ScreenToWorldPoint(temp);
        }
    }

    public void Preprocess()
    {
        double[][] input;
        if (sequence.Count > 1)
        {
            input = Sequence.Preprocess(GetSequence());
            if (hmm != null)
            {
                int index = hmm.Decide(input);
                string label = database.Classes[index];
                text.text = "Do you Mean : " + label + "?";
                CreateObject(label);
                sequence.Clear();
            }
        }
    }

    public Point[] GetSequence()
    {
        return sequence.ToArray();
    }

    private void CreateObject(string objName)
    {
        switch(objName)
        {
            case "Table":
                currentFocus = table;
                currentFocus.tag = table.tag;
                break;
            case "TV":
                currentFocus = tv;
                currentFocus.tag = "TV";
                break;
            case "Sofa":
                currentFocus = sofa;
                currentFocus.tag = "Sofa";
                break;
            case "Shelf":
                currentFocus = shelf;
                currentFocus.tag = "Shelf";
                break;
            case "Book":
                currentFocus = book;
                currentFocus.tag = "Book";
                break;
            case "Lamp":
                currentFocus = lamp;
                currentFocus.tag = "Lamp";
                break;
            case "Bin":
                currentFocus = bin;
                currentFocus.tag = "Bin";
                break;
            case "Microwave":
                currentFocus = micro;
                currentFocus.tag = "Microwave";
                break;
            case "Refrigerator":
                currentFocus = refrig;
                currentFocus.tag = "Refrigerator";
                break;
            case "Sink":
                currentFocus = sink;
                currentFocus.tag = sink.tag;
                break;
            case "Release":
                if (currentFocus != null)
                {
                    GetObjectFromTag(currentFocus.tag);
                    //currentFocus.transform.position = tempTrans.position;
                    isWantRealease = true;
                }
                break;
        }
        
    }

    private void GetObjectFromTag(string tag)
    {
        switch (tag)
        {
            case "Table":
                currentFocus.transform.position = oriTranTablel;
                break;
            case "TV":
                currentFocus.transform.position = oriTranTV;
                break;
            case "Sofa":
                currentFocus.transform.position = oriTranSofa;
                break;
            case "Shelf":
                currentFocus.transform.position = oriTranShelf;
                break;
            case "Book":
                currentFocus.transform.position = oriTranBook;
                break;
            case "Lamp":
                currentFocus.transform.position = oriTranLamp;
                break;
            case "Bin":
                currentFocus.transform.position = oriTranBin;
                break;
            case "Microwave":
                currentFocus.transform.position = oriTranMicro;
                break;
            case "Refrigerator":
                currentFocus.transform.position = oriTranRefrig;
                break;
            case "Sink":
                currentFocus.transform.position = oriTranSink;
                break;
        }
    }
}
