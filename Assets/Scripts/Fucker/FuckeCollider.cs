using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof (Collider2D))]
public class FuckeCollider : MonoBehaviour
{
    public ObjectPool<List<ContactPoint2D>> contactPointPool = new ObjectPool<List<ContactPoint2D>>(() => new List<ContactPoint2D>(), v => v.Do(l => l.Clear()));
    public Dictionary<Collider2D, (List<ContactPoint2D>, MutableBox<int>)> contacts = new Dictionary<Collider2D, (List<ContactPoint2D>, MutableBox<int>)>();

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(!contacts.ContainsKey(collision.collider)) contacts.Add(collision.collider, (contactPointPool.GetObject(), 0));
        collision.GetContacts(contacts[collision.collider].Item1);
        contacts[collision.collider].Item2.value = collision.contactCount;
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!contacts.ContainsKey(collision.collider)) contacts.Add(collision.collider, (contactPointPool.GetObject(), 0));
        collision.GetContacts(contacts[collision.collider].Item1);
        contacts[collision.collider].Item2.value = collision.contactCount;
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        if (contacts.ContainsKey(collision.collider))
        {
            contactPointPool.PutObject(contacts[collision.collider].Item1);
            contacts.Remove(collision.collider);
        }
    }

    private void OnDrawGizmos()
    {
        var color = Gizmos.color;
        Gizmos.color = (Color.cyan * 0.5f).Do((ref Color c) => c.a = 1);
        foreach (var (points, count) in contacts.Values)
        {
            for (int i = 0; i < count; i++)
            {
                var contact = points[i];
                Gizmos.DrawRay(contact.point, contact.normal * 10);
            }
        }
        Gizmos.color = color;
    }
}
