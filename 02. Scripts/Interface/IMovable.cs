using UnityEngine;

public interface IMovable
{
    float MovementSpeed { get; set; }
    float SprintSpeed{get;set;}
    void Move(Vector2 direction);
}