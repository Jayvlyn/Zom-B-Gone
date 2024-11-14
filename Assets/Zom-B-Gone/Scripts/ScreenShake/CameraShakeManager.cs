using Cinemachine;
using UnityEngine;

public class CameraShakeManager : MonoBehaviour
{
    public static CameraShakeManager instance;

    [SerializeField] private float globalShakeForce = 1f;
    [SerializeField] private CinemachineImpulseListener impulseListener;

    private CinemachineImpulseDefinition impulseDefinition;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void CameraShake(CinemachineImpulseSource impulseSource)
    {
        impulseSource.GenerateImpulseWithForce(globalShakeForce);
    }

    public void CameraShake(CinemachineImpulseSource impulseSource, ScreenShakeProfile profile)
    {

        SetupCameraShakeSettings(impulseSource, profile);

        impulseSource.GenerateImpulseWithForce(profile.impactForce);
    }

    public void CameraShake(CinemachineImpulseSource impulseSource, ScreenShakeProfile profile, Vector3 velocity)
    {

        SetupCameraShakeSettings(impulseSource, profile, velocity);

        impulseSource.GenerateImpulseWithForce(profile.impactForce);
    }


    private ScreenShakeProfile latestProfile;
    private void SetupCameraShakeSettings(CinemachineImpulseSource impulseSource, ScreenShakeProfile profile)
    {
        if (profile == latestProfile) return;
        latestProfile = profile;

        impulseDefinition = impulseSource.m_ImpulseDefinition;

        impulseDefinition.m_ImpulseDuration = profile.impactTime;
        impulseSource.m_DefaultVelocity = profile.defaultVelocity;
        impulseDefinition.m_CustomImpulseShape = profile.impulseCurve;

        impulseListener.m_ReactionSettings.m_AmplitudeGain = profile.listenerAmplitude;
        impulseListener.m_ReactionSettings.m_FrequencyGain = profile.listenerFrequency;
        impulseListener.m_ReactionSettings.m_Duration = profile.listenerDuration;
    }

    private void SetupCameraShakeSettings(CinemachineImpulseSource impulseSource, ScreenShakeProfile profile, Vector3 velocity)
    {
        impulseSource.m_DefaultVelocity = velocity;

        if (profile == latestProfile) return;
        latestProfile = profile;

        impulseDefinition = impulseSource.m_ImpulseDefinition;

        impulseDefinition.m_ImpulseDuration = profile.impactTime;
        impulseDefinition.m_CustomImpulseShape = profile.impulseCurve;

        impulseListener.m_ReactionSettings.m_AmplitudeGain = profile.listenerAmplitude;
        impulseListener.m_ReactionSettings.m_FrequencyGain = profile.listenerFrequency;
        impulseListener.m_ReactionSettings.m_Duration = profile.listenerDuration;
    }
}
