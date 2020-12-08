using System.Collections;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{

    [SerializeField] private float shootInterval = 3f;
    [SerializeField] private Vector2 shotDirection = Vector2.left;
    [SerializeField] private float shotForce = 10f;

    private GameObject _bullet;
    private Bounds _bound;

    private void Awake()
    {
        _bound = GetComponent<BoxCollider2D>().bounds;
        _bullet = Resources.Load<GameObject>("Prefabs/Tiro");
    }

    private void OnBecameVisible()
    {
        StartCoroutine(Shooting());
    }
    
    private IEnumerator Shooting()
    {
        while (true)
        {
            yield return new WaitForSeconds(shootInterval);
            var position = transform.position;
            var shotPosition = new Vector2(position.x + (_bound.size.x *shotDirection.x) , position.y);
            var newBullet = Instantiate(_bullet, shotPosition , Quaternion.identity);

            newBullet.GetComponent<Rigidbody2D>().velocity = shotDirection * shotForce;

        }
    }
    
}
