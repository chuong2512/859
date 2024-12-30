using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
    public enum MovementDirection
    {
        Right,
        Left
    }

    [SerializeField] private float movementDuration;
    [SerializeField] private float speed;
    [SerializeField] private MovementDirection direction;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("MovableObject"))
        {
            var otherRigidbody = collision.gameObject.GetComponent<Rigidbody>();
            if (otherRigidbody != null)
            {
                var movementVector = Vector3.zero;

                switch (direction)
                {
                    case MovementDirection.Right:
                        movementVector = transform.right;
                        break;
                    case MovementDirection.Left:
                        movementVector = -transform.right;
                        break;
                }

                StartCoroutine(StartMovement(otherRigidbody, movementVector));
            }
        }
    }

    private IEnumerator StartMovement(Rigidbody rigidbody, Vector3 movementVector)
    {
        var timer = 0f;

        while (timer < movementDuration)
        {
            rigidbody.velocity = movementVector * speed;
            timer += Time.deltaTime;
            yield return null;
        }

        rigidbody.velocity = Vector3.zero;
    }
}