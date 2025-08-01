using UnityEngine;

public class HookStackManager : MonoBehaviour
{
    public int currentStackCount = 0;
    public Vector3 stackOffset = new Vector3(0, 0.05f, 0);

    public Vector3 GetNextStackPosition()
    {
        return stackOffset * currentStackCount;
    }

    public void IncrementStack()
    {
        currentStackCount++;
    }

    public void DecrementStack()
    {
        currentStackCount--;
    }
}