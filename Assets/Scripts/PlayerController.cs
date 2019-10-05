using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float Speed = 0.1f;
    public LayerMask CollideMask;

    private BoxCollider2D _collider;
    private ContactFilter2D _contactFilter;

    void Start()
    {
        _collider = GetComponent<BoxCollider2D>();
        _contactFilter.SetLayerMask(CollideMask);
        GetComponent<Rigidbody2D>().freezeRotation = true;
    }

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        if ((x != 0) || (y != 0))
        {
            var dir = new Vector2(x, y) * Speed;

            var pos = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);
            GetComponent<Rigidbody2D>().MovePosition(pos + dir);

            //RaycastHit2D[] hits = new RaycastHit2D[10];
            //int numHits = _collider.Cast(dir.normalized, _contactFilter, hits, dir.magnitude);
            ////int numHits = Physics2D.BoxCastNonAlloc(transform.position, _collider.size, 0.0f, dir.normalized, hits, dir.magnitude, CollideMask);
            ////if (numHits > 0)
            ////{
            ////    var contacts = new ContactPoint2D[30];
            ////    int numContacts = _collider.GetContacts(contacts);
            ////}
            //for (int i = 0; i < numHits; ++i)
            //{
            //    var hit = hits[i];
            //    if (hit.collider is CompositeCollider2D compositeCollider)
            //    {
            //        compositeCollider.pol

            //        //// Composite collider is not convex, reports a single hit for the entire collider
            //        //// even if multiple edges are colliding, so need manual processing
            //        //var contacts = new ContactPoint2D[30];
            //        //var contactFilter = new ContactFilter2D();
            //        //contactFilter.SetLayerMask(LayerMask.NameToLayer("Player"));
            //        ////int numContacts = compositeCollider.GetContacts(contactFilter, contacts);
            //        //int numContacts = compositeCollider.GetContacts(contacts);
            //        //for (int j = 0; j < numContacts; ++j)
            //        //{
            //        //    var contact = contacts[j];
            //        //    var dot = Vector2.Dot(contact.normal, dir);
            //        //    if (dot < 0)
            //        //    {
            //        //        Vector2 slide = dir - dot * contact.normal;
            //        //       // dir = dir * contact.fraction + slide;
            //        //    }
            //        //}
            //    }
            //    else
            //    {
            //        var dot = Vector2.Dot(hit.normal, dir);
            //        if (dot < 0)
            //        {
            //            Vector2 slide = dir - dot * hit.normal;
            //            dir = dir * hit.fraction + slide;
            //        }
            //    }
            //}
            //var delta = new Vector3(dir.x, dir.y, 0);
            //gameObject.transform.position += delta;
        }
    }
}
