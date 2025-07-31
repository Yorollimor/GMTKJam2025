using System;
using UnityEngine;

public class SolverTester : MonoBehaviour
{
    [SerializeField] 
    private Solver solver;

    [SerializeField] private float magScale = 100;
    

    void Update()
    {
        var res = solver.GetFluidVector(transform.position);
        transform.localScale = new Vector3(1, Mathf.Clamp(res.magnitude*magScale,0.5f,100), 1);
        
        float angle = Mathf.Atan2(res.x, res.y) * -Mathf.Rad2Deg + 90;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
