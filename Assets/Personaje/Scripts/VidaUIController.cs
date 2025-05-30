using UnityEngine;
using UnityEngine.UI;

public class VidaUIController : MonoBehaviour
{
    public Image barraVida;

    public void ActualizarVida(int vidaActual, int vidaMaxima)
    {
        float fill = (float)vidaActual / vidaMaxima;
        barraVida.fillAmount = fill;
    }
}