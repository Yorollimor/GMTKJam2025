using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    int tankUpgradeLevel = 0;
    int hookUpgradeLevel = 0;

    public Watertank[] tankUprades;
    public GameObject[] hookUpgrades;


    private void Start()
    {
        GameManager.Instance.upgradeManager = this;
    }

    public void UpgradeTank()
    {
        if (tankUpgradeLevel < tankUprades.Length - 1)
        {
            tankUpgradeLevel++;
            Watertank nT = Instantiate(tankUprades[tankUpgradeLevel], Vector3.zero, Quaternion.identity);
            GameManager.Instance.currentTank.SwapTanks(nT);
            FindAnyObjectByType<UIManager>().SetNewStoreButton(nT.storeButton);
        }
    }
    public void UpgradeHook()
    {
        if (hookUpgradeLevel < hookUpgrades.Length - 1)
        {
            hookUpgradeLevel++;
            foreach (HookTrigger t in GameManager.Instance.currentTank.moveableObjectsParent.GetComponentsInChildren<HookTrigger>(true))
            {
                Vector3 pos = t.transform.position;
                Quaternion rot = t.transform.rotation;
                Destroy(t.transform.parent.gameObject);
                GameObject nHook = Instantiate(hookUpgrades[hookUpgradeLevel], pos, rot, GameManager.Instance.currentTank.moveableObjectsParent);

            }
        }
    }
}
