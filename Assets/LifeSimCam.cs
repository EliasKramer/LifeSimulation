using Assets;
using UnityEngine;
public class LifeSimCam : MonoBehaviour
{
    [SerializeField]
    public ComputeShader lifeSimShader;
    
    private RenderTexture renderTexture;
    
    private const int threads = 128;
    private int numberOfAgents = threads * 1;

    public Agent[] agents;
    private ComputeBuffer agentBuffer;

    public void Awake()
    {
        renderTexture = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
        renderTexture.enableRandomWrite = true;
        renderTexture.Create();
    }
    public void Start()
    {
        initAgents();
        setConstants();
    }

    private void setConstants()
    {
        lifeSimShader.SetFloat("pi", Mathf.PI);
    }

    private void initAgents()
    {
        agents = new Agent[numberOfAgents];
        AgentManager manager = new AgentManager();
        for (int i = 0; i < agents.Length; i++)
        {
            agents[i] = manager.GetRandomAgent();
        }
        agentBuffer = new ComputeBuffer(numberOfAgents, AgentManager.GetSingleAgentByteSize());
        agentBuffer.SetData(agents);
        lifeSimShader.SetBuffer(lifeSimShader.FindKernel("UpdateAgents"), "agents", agentBuffer);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(renderTexture, destination);
    }   
    private void FixedUpdate()
    {
        UpdateAgents();
    }
    private void UpdateAgents()
    {
        int kernel = lifeSimShader.FindKernel("UpdateAgents");
        lifeSimShader.SetFloat("sizeX", Screen.width);
        lifeSimShader.SetFloat("sizeY", Screen.height);
        lifeSimShader.SetFloat("deltaTime", Time.deltaTime);
        lifeSimShader.SetInt("agentCount", numberOfAgents);
        lifeSimShader.SetTexture(kernel, "Result", renderTexture);
        int workgroups = Mathf.CeilToInt(numberOfAgents / threads);
        lifeSimShader.Dispatch(kernel, workgroups, 1, 1);
    }
    private void OnDestroy()
    {
        agentBuffer.Dispose();
    }
}
