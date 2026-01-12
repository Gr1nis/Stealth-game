using UnityEngine;

public class Inventory : MonoBehaviour
{
    private int keycards = 0;

    public void AddKeycard()
    {
        keycards++;
        Debug.Log("Ключ-карта получена. Всего: " + keycards);
    }

    public int KeycardCount() => keycards;

    public bool HasEnoughKeycards(int required) => keycards >= required;
}
