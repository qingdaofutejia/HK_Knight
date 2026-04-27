using Photon.Pun;
using UnityEngine;


public class MonsterNetwork : MonoBehaviourPun, IPunObservable
{
    public float smoothSpeed = 40f;

    private Vector3 networkPosition;
    private Vector3 networkScale;
    private Vector2 networkVelocity;

    private Rigidbody2D rb;

    void Awake()
    {
        networkPosition = transform.position;
        networkScale = transform.localScale;
        networkVelocity = Vector2.zero;
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            if (rb != null)
            {
                rb.velocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Kinematic;
                rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            }
        }
    }

    void Update()
    {
        if (photonView.IsMine)
            return;

        Vector3 targetPos = networkPosition + (Vector3)(networkVelocity * 0.02f);
        float distance = Vector3.Distance(transform.position, targetPos);

        if (distance > 1.5f)
        {
            transform.position = targetPos;
        }
        else
        {
            transform.position = Vector3.Lerp(
                transform.position,
                targetPos,
                Time.deltaTime * smoothSpeed
            );
        }

        Vector3 scale = transform.localScale;
        scale.x = networkScale.x >= 0 ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.localScale);
            stream.SendNext(rb != null ? rb.velocity : Vector2.zero);
        }
        else
        {
            networkPosition = (Vector3)stream.ReceiveNext();
            networkScale = (Vector3)stream.ReceiveNext();
            networkVelocity = (Vector2)stream.ReceiveNext();
        }
    }
}
