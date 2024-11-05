using UnityEngine;

public class LightIntensitySampler : MonoBehaviour
{
    public ComputeShader computeShader;
    public RenderTexture lightCaptureTexture; // Assigned to the small camera's render texture
    public static float intensity = 0f;

    private int kernelHandle;
    private ComputeBuffer resultBuffer;
    private float[] resultData = new float[1];

    void Start()
    {
        kernelHandle = computeShader.FindKernel("CSMain");

        resultBuffer = new ComputeBuffer(1, sizeof(float));
        computeShader.SetBuffer(kernelHandle, "Result", resultBuffer);
    }

    void Update()
    {
        // Reset result buffer data
        resultData[0] = 0f;
        resultBuffer.SetData(resultData);

        // Set the texture and dispatch the shader
        computeShader.SetTexture(kernelHandle, "LightTexture", lightCaptureTexture);
        computeShader.Dispatch(kernelHandle, lightCaptureTexture.width / 8, lightCaptureTexture.height / 8, 1);

        // Retrieve the average intensity result
        resultBuffer.GetData(resultData);
        intensity = resultData[0] / (lightCaptureTexture.width * lightCaptureTexture.height); // Normalize intensity
        Debug.Log(intensity);
    }

    private void OnDestroy()
    {
        resultBuffer.Release();
    }
}
