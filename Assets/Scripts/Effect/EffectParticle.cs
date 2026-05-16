using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class EffectParticle : EffectBase
{
    private ParticleSystem _particle;

    private void Awake()
    {
        _particle = GetComponent<ParticleSystem>();
    }

    protected override void OnPlay()
    {
        _particle.Clear();
        _particle.Play();
    }

    protected override void OnStop()
    {
        _particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    // ParticleSystem Main 모듈의 Stop Action을 Callback으로 설정해야 호출됨
    private void OnParticleSystemStopped() => OnComplete();
}
