using UnityEngine;

public class GrappleUIController : MonoBehaviour
{
    [Header("Primary Hook States")]
    public GameObject[] primaryStates; // 0–3 оставшихся

    [Header("Secondary Hook States")]
    public GameObject[] secondaryStates; // [0] — пустой, [1] — заряженный

    public void UpdatePrimaryState(int remaining)
    {
        for (int i = 0; i < primaryStates.Length; i++)
        {
            bool isActive = (i == remaining);

            bool wasActive = primaryStates[i].activeSelf;

            primaryStates[i].SetActive(isActive);

            if (isActive && !wasActive)
            {
                var fx = primaryStates[i].GetComponent<GrappleChargeEffect>();
                if (fx != null) fx.TriggerMeatSplat();
            }
        }
    }

    public void UpdateSecondaryState(bool hasCharge)
    {
        for (int i = 0; i < secondaryStates.Length; i++)
        {
            bool isActive = (i == (hasCharge ? 1 : 0));

            bool wasActive = secondaryStates[i].activeSelf;

            secondaryStates[i].SetActive(isActive);

            if (isActive && !wasActive)
            {
                var fx = secondaryStates[i].GetComponent<GrappleChargeEffect>();
                if (fx != null) fx.TriggerMeatSplat();
            }
        }
    }
}
