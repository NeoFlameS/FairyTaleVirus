using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSystem : MonoBehaviour {

    public const int CM_MONSTER = 1;
    public const int CM_PLAYER = 2;
    public const int CM_QTVIEW = 3;
    public const int CM_FREE = 4;

    public MainGameSystem MGS;
    public Transform[] QTVIEW_POINT;
    int N_QTVIEW;
    int N_Character;

    public GameObject g_camera;

    public GameObject target;
    public float dist = 10.0f;
    public float height = 5.0f;
    public float smoothRotate = 5.0f;
    public float smoothPosition = 5.0f;

    float yrotation;
    float rotationv1;
    float rotationv2;

    Vector3 targetLocation;

    //6.17 홍승준 추가
    Queue up_packet = new Queue();

    // Use this for initialization
    void Start () {
        yrotation = 0;
        N_QTVIEW = 0;
        N_Character = 0;
        targetLocation = transform.position;
    }
	
	// Update is called once per frame
	void LateUpdate () {
        ChaseTarget();
        CameraLerpMove();
        Process_Packet();
    }

    void ChaseTarget() {
        if (target != null)
        {
            float currYAngle = Mathf.LerpAngle(transform.eulerAngles.y, target.transform.eulerAngles.y, smoothRotate * Time.deltaTime);
            Quaternion rotation = Quaternion.Euler(0, currYAngle, 0);

            transform.position = target.transform.position - (rotation * Vector3.forward * dist) + (Vector3.up * height);
            g_camera.transform.LookAt(target.transform);
        }
    }

    void CameraLerpMove() {
        if (target == null) {
            float currXmove = Mathf.Lerp(transform.position.x, targetLocation.x, smoothPosition * Time.deltaTime);
            float currZmove = Mathf.Lerp(transform.position.z, targetLocation.z, smoothPosition * Time.deltaTime);

            transform.position = new Vector3(currXmove, transform.position.y, currZmove);
        }
    }

    public void Zoom(bool z) {//
        if (target != null)
        {
            if (true == z) dist -= 1f;
            else dist += 1f;
        }
        else {
            if (true == z) transform.position = transform.position - (g_camera.transform.rotation * Vector3.forward * 1f);
            else transform.position = transform.position + (g_camera.transform.rotation * Vector3.forward * 1f);
        }
        targetLocation = transform.position;
    }

    public void Rotate(bool right)//
    {
        if (target == null)
        {
            if (true == right) transform.Rotate(Vector3.up, 10f);
            else transform.Rotate(Vector3.down, 10f);
        }
    }

    public void Move(float x, float y)//
    {
        if (target == null)
        {
            Vector3 ax = (x * (g_camera.transform.rotation * Vector3.right));
            Vector3 az = (y * (g_camera.transform.rotation * Vector3.forward));
            targetLocation += new Vector3(ax.x + az.x, 0, ax.z + az.z) ;
        }
    }

    public void targetChange(int CM) {
        switch (CM) {
            case CM_FREE:
                target = null;
                targetLocation = transform.position;
                break;
            case CM_QTVIEW:
                target = null;
                N_QTVIEW = (N_QTVIEW+1)%QTVIEW_POINT.Length;
                transform.position = QTVIEW_POINT[N_QTVIEW].position;
                transform.rotation = Quaternion.identity;
                g_camera.transform.rotation = QTVIEW_POINT[N_QTVIEW].rotation;
                targetLocation = transform.position;
                break;
            case CM_PLAYER:
                transform.rotation = Quaternion.identity;
                target = MGS.PC[N_Character].gameObject;
                N_Character = (N_Character + 1) % MGS.usercount;
                break;
            case CM_MONSTER:
                break;
        }
    }

    public void Recived_CameraPacket(object o) {
        Debug.Log("CameraPacket Recieve");
        lock (up_packet)
        {

            CS_CAMERA_PACKET cur = new CS_CAMERA_PACKET();
            cur = (CS_CAMERA_PACKET)o;

            up_packet.Enqueue(cur);

            
            Debug.Log("현재 큐 개수 : " + up_packet.Count);
        }
        return;
    }

    public void Process_Packet() {

        CS_CAMERA_PACKET cur;
        if (up_packet.Count < 1) { return; }
        lock (up_packet) {
            cur = (CS_CAMERA_PACKET)up_packet.Dequeue();
        }
        int x_dir,y_dir;
        x_dir = 1;
        y_dir = 1;
        switch (cur.type)
        {
            case 0://0: 줌 
                if (cur.y >= 0) { this.Zoom(false); }
                else { this.Zoom(true); }
                break;
            case 1://1:회전 
                if (cur.x >= 0) { this.Rotate(true); }
                else { this.Rotate(false); }
                break;
            case 2://2:이동 
                this.Move(cur.x, cur.y);
                break;
            case 3://3: 타겟 패드 방향(절대 값이 큰 축 위주)에 따라서 처리 <-: 캐릭터 ->: 몬스터 ^: 자유시점 v: 쿼터뷰 
                if (cur.y < 0) { y_dir = -1; }
                if (cur.x < 0) { x_dir = -1; }

                if (cur.y * y_dir > cur.x * x_dir) {
                    if (y_dir == 1) { this.targetChange(CM_FREE); }
                    else { this.targetChange(CM_QTVIEW); }
                }
                else if (cur.y * y_dir < cur.x * x_dir) {
                    if (x_dir == 1) { this.targetChange(CM_MONSTER); }
                    else { this.targetChange(CM_PLAYER); }
                }

                break;
            default:
                break;
        }

    }

}
