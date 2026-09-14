using UnityEngine;
using UnityEngine.UI;

public class UIMoving : MonoBehaviour
{
    [SerializeField] Button left;
    [SerializeField] Button right;
    [SerializeField] Button up;
    [SerializeField] Button down;
    [SerializeField] float movement;

    void Start()
    {
        if (left != null) left.onClick.AddListener(() => MoveMe(new Vector3(-movement, 0)));
        if (right != null) right.onClick.AddListener(() => MoveMe(new Vector3(movement, 0)));
        if (up != null) up.onClick.AddListener(() => MoveMe(new Vector3(0, movement)));
        if (down != null) down.onClick.AddListener(() => MoveMe(new Vector3(0, -movement)));

        void MoveMe(Vector3 direction)
        {
            this.transform.position += (direction);
        }
    }
}