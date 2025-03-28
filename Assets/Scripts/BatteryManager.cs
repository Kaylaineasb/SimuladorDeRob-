using UnityEngine;
using UnityEngine.UI;
//controla a bateria do robô e atualiza a UI
public class BatteryManager : MonoBehaviour
{
  public int maxBattery = 100; // Capacidade máxima da bateria
    public int currentBattery; // Bateria atual
    public Image batteryOverlay; // A cápsula que mostra a quantidade de bateria restante

    private void Start()
    {
        currentBattery = maxBattery; // Começa com a bateria cheia
        UpdateBatteryUI(); // Atualiza a UI ao iniciar
    }

    public void DecreaseBattery(int amount)
    {
        currentBattery -= amount; // Diminui a bateria
        currentBattery = Mathf.Clamp(currentBattery, 0, maxBattery); // Garante que a bateria não fique negativa

        UpdateBatteryUI(); // Atualiza a UI após diminuir a bateria
    }

    public void RechargeBattery()
    {
        currentBattery = maxBattery; // Recarrega a bateria para o máximo
        UpdateBatteryUI(); // Atualiza a UI após recarregar
    }

    public void UpdateBatteryUI()
    {
        if (batteryOverlay != null)
        {
            // Calcula a nova escala com base na porcentagem da bateria restante
            float fillAmount = (float)currentBattery / maxBattery; // Porcentagem da bateria
            batteryOverlay.rectTransform.localScale = new Vector3(fillAmount, 1, 1); // Ajusta a escala da barra de bateria
        }
    }
}