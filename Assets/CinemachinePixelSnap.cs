using Cinemachine;
using UnityEngine;

public class CinemachinePixelSnap : CinemachineExtension
{
    [SerializeField] private float pixelsPerUnit = 16f;

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage,
        ref CameraState state,
        float deltaTime)
    {
        if (stage != CinemachineCore.Stage.Finalize) return;

        Vector3 pos = state.RawPosition;
        pos.x = Mathf.Round(pos.x * pixelsPerUnit) / pixelsPerUnit;
        pos.y = Mathf.Round(pos.y * pixelsPerUnit) / pixelsPerUnit;
        state.RawPosition = pos;
    }
}