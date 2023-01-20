using Assets;
using System;
using System.Linq;
using UnityEngine;
public class LifeSimCam : MonoBehaviour
{
    [SerializeField]
    public ComputeShader lifeSimShader;
    
    private RenderTexture renderTexture;
    
    private const int threads = 1024;
    private int numberOfAgents = threads * 8;

    [SerializeField]
    float randomAngleRange;
    [SerializeField]
    float randomMult;
    [SerializeField]
    float blurrMultiplier;

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
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(renderTexture, destination);
    }
    private void FixedUpdate()
    {
        UpdateAgents();
        KillPrey();
        Blurr();
    }

    private void Blurr()
    {
        int kernel = lifeSimShader.FindKernel("Blurr");
        lifeSimShader.SetTexture(kernel, "Result", renderTexture);
        int workgroupsX = Mathf.CeilToInt(Screen.width / 8.0f);
        int workgroupsY = Mathf.CeilToInt(Screen.height / 8.0f);
        lifeSimShader.Dispatch(kernel, workgroupsX, workgroupsY, 1);
    }

    private void UpdateAgents()
    {
        int kernel = lifeSimShader.FindKernel("UpdateAgents");
        agentBuffer.SetData(agents);
        lifeSimShader.SetBuffer(kernel, "agents", agentBuffer);

        lifeSimShader.SetFloat("sizeX", Screen.width);
        lifeSimShader.SetFloat("sizeY", Screen.height);
        lifeSimShader.SetFloat("randomAngleRange", randomAngleRange * Mathf.Deg2Rad);
        lifeSimShader.SetFloat("randomMult", randomMult);
        lifeSimShader.SetFloat("deltaTime", Time.deltaTime);
        lifeSimShader.SetFloat("blurrMultiplier", blurrMultiplier);
        lifeSimShader.SetInt("agentCount", numberOfAgents);
        lifeSimShader.SetTexture(kernel, "Result", renderTexture);
        int workgroups = Mathf.CeilToInt(numberOfAgents / threads);
        lifeSimShader.Dispatch(kernel, workgroups, 1, 1);

        agentBuffer.GetData(agents);

        //delete all blue agents
        //agents = agents.Where(x => x.color.b != 1).ToArray();
    }
    private void KillPrey()
    {
        int agentsDeadBefore1 = agents.Where(x => x.dead == 1).Count();
        int kernel = lifeSimShader.FindKernel("KillPrey");
        lifeSimShader.SetBuffer(kernel, "agents", agentBuffer);
        lifeSimShader.SetTexture(kernel, "Result", renderTexture);
        
        int workgroups = Mathf.CeilToInt(numberOfAgents / threads);
        lifeSimShader.Dispatch(kernel, workgroups, 1, 1);

        agentBuffer.GetData(agents);

        Debug.Log("dead1: " + agents.Where(x => x.dead == 1).Count() + " , dead before: " +  agentsDeadBefore1);
    }
    private void OnDestroy()
    {
        agentBuffer.Dispose();
    }
}
