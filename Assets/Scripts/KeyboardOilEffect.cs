using UnityEngine;

public class KeyboardOilEffect : MonoBehaviour
{
    [SerializeField] private ParticleSystem leftParticles;
    [SerializeField] private ParticleSystem rightParticles;
    [SerializeField] private int burstCount = 4;

    public void TriggerSplash()
    {
        leftParticles?.Emit(burstCount);
        rightParticles?.Emit(burstCount);
    }
}
