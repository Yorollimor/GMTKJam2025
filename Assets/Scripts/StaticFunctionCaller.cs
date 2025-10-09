using UnityEngine;

public class StaticFunctionCaller : MonoBehaviour
{
    public void Watertank_SpurtPressed()
    {
        GameManager.Instance.currentTank.PressSpurtButton();
    }
    public void Watertank_SpurtReleased()
    {
        GameManager.Instance.currentTank.ReleaseSpurtButton();
    }
    public void Watertank_Grabed()
    {
        GameManager.Instance.currentTank.IsGrabed();
    }
    public void Watertank_Relesed()
    {
        GameManager.Instance.currentTank.IsReleased();
    }
    public void Watertank_Moved()
    {
        GameManager.Instance.currentTank.MoveGrabbed();
    }
}
