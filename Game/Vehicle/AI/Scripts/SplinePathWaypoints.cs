using UnityEngine;
using System.Collections;

public class SplinePathWaypoints : SplinePath {

    public bool waypointActive = true;
    public bool show = false;
    //public GameObject waypointPrefab;
    private string m_waypointPreName = "MyWaypoint";
    private string m_waypointFolder = "MyWaypoints";
    private Transform parent;

    protected override void Awake() {
        
        if (waypointActive)
            Init();

    }
    
    void Start() {
        
        //if(waypointActive)
        //  Init();

        if (show && waypointActive) {
            SetRenderer(true);
        }
        
        if (waypointActive && show && Application.isEditor && Application.isPlaying) {
            GetWaypointNames();
            FillPath();
            FillSequence();
        }
    }
    
    protected override void OnDrawGizmos() {
        //if (waypointActive && (!Application.isPlaying || show))
        if (waypointActive && (!Application.isPlaying)) {
            GetWaypointNames();
            FillPath();
            FillSequence();
            //DrawGizmos();
        }

        if (waypointActive && (!Application.isPlaying || show)) {
            DrawGizmos();
        }
        /*
        /* 
        if(!Application.isPlaying)
        {
            //SetDrawLineToNext();
        }
        */
    }

    void Init() {

        GetWaypointNames();
        FillPath();
        FillSequence();
        parent = GameObject.Find(m_waypointFolder).transform;
        CreateNewWaypoints();
        RenamePathObjects();

    }

    void CreateNewWaypoints() {

        int counter = 0;
        //GameObject prefab = Resources.LoadAssetAtPath("Assets/GameVehicleAIDriverToolkit/Prefabs/Waypoint.prefab", typeof(GameObject)) as GameObject;      
        GameObject go;
        string currentName;
        currentName = "/" + m_waypointFolder + "/" + m_waypointPreName + 1;
        go = GameObject.Find(currentName);
        GameObject prefab = go;

        foreach (Vector3 point in sequence) {

            counter++;

            //den letzten erzeugen wir nicht, da dieses die gleich Position hat wie der erste
            if (counter < sequence.Count || !loop) {
                GameObject waypoint = Instantiate(prefab) as GameObject;
                waypoint.transform.position = point;
                waypoint.name = m_waypointPreName + counter.ToString();
                waypoint.transform.parent = parent;
                //GameVehicleAIWaypoint aiwaypointScript = waypoint.GetComponent("GameVehicleAIWaypoint") as GameVehicleAIWaypoint;    

                CopyParameters(ref waypoint, counter);
            }
        }
    }

    void CopyParameters(ref GameObject waypoint, int newIndex) {

        //float fltOldIndex = newIndex / (steps + 1);

        int intOldIndex;

        int modIndex = newIndex % (steps + 1);

        if (modIndex == 0) {
            intOldIndex = newIndex / (steps + 1);
        }
        else {
            intOldIndex = 1 + (newIndex / (steps + 1));
        }

        GameVehicleAIWaypoint oldAiWaypointScript = path[intOldIndex - 1].GetComponent("GameVehicleAIWaypoint") as GameVehicleAIWaypoint;

        GameVehicleAIWaypoint aiWaypointScript = waypoint.GetComponent("GameVehicleAIWaypoint") as GameVehicleAIWaypoint;

        aiWaypointScript.speed = oldAiWaypointScript.speed;
        aiWaypointScript.useTrigger = oldAiWaypointScript.useTrigger;

        //if (aiWaypointScript.useTrigger)
        //{                                 

        //BoxCollider bc = waypoint.AddComponent<BoxCollider>();    
        //bc.isTrigger = true;
        //waypoint.layer = path[intOldIndex - 1].gameObject.layer; Die automatische Zuweisung geschieht erst spaeter
        //waypoint.layer = 2;
        
        /*
            BoxCollider bcTest =gameObject.GetComponent<BoxCollider>();
            if (bcTest == null)                                         
            {
                BoxCollider bc = gameObject.AddComponent<BoxCollider>();    
                bc.isTrigger = true;
                gameObject.layer = 2;
            }
            else 
            {
                bcTest.isTrigger = true;
                gameObject.layer = 2;
            } 
            */

        //}

        waypoint.transform.localScale = path[intOldIndex - 1].localScale;
        waypoint.tag = path[intOldIndex - 1].gameObject.tag;
    }

    void RenamePathObjects() {
        foreach (Transform current in path) {

            current.gameObject.name = current.gameObject.name + "_original";
        }
    }

    void FillPath() {
        bool found = true;
        int counter = 1;

        path.Clear();

        while (found) {
            GameObject go;
            string currentName;
            currentName = "/" + m_waypointFolder + "/" + m_waypointPreName + counter.ToString();
            go = GameObject.Find(currentName);

            if (go != null) {
                path.Add(go.transform);
                counter++;
            }
            else {
                found = false;
            }
        }
    }

    void GetWaypointNames() {
        GameVehicleAIWaypointEditor aiWaypointEditor;

        aiWaypointEditor = GetComponent("GameVehicleAIWaypointEditor") as GameVehicleAIWaypointEditor;
        if (aiWaypointEditor != null) {
            m_waypointPreName = aiWaypointEditor.preName + "_";
            m_waypointFolder = aiWaypointEditor.folderName;
        }
    }

    void SetRenderer(bool waypointActive) {

        bool found = true;
        int counter = 1;

        path.Clear();

        while (found) {
            GameObject go;
            string currentName;
            currentName = "/" + m_waypointFolder + "/" + m_waypointPreName + counter.ToString();
            go = GameObject.Find(currentName);

            if (go != null && go.Has<Rigidbody>()) {
                go.GetComponent<Renderer>().enabled = waypointActive;
                counter++;
            }
            else {
                found = false;
            }

        }

    }

    void SetDrawLineToNext() {
        if (waypointActive) {

        }
        bool found = true;
        int counter = 1;

        path.Clear();

        while (found) {
            GameObject go;
            string currentName;
            currentName = "/" + m_waypointFolder + "/" + m_waypointPreName + counter.ToString();
            go = GameObject.Find(currentName);

            if (go != null) {
                DrawLineToNext drawLineToNext = go.GetComponent<DrawLineToNext>() as DrawLineToNext;
                if (drawLineToNext != null) {
                    if (waypointActive) {
                        drawLineToNext.gameObject.SetActive(false);
                    }
                    else {
                        drawLineToNext.gameObject.SetActive(true);
                    }
                }

            }
            else {
                found = false;
            }

        }
    }
}