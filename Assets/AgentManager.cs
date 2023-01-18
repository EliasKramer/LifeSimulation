using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum AgentBehaviour
{
    Flee = 1,
    Hunt = 2
}
public struct Agent
{
    public Vector2 position;
    public float angle;
    public float speed;
    public Color color;
    public AgentBehaviour behaviour;

    public Agent(Vector2 position, float angle, float speed, Color color, AgentBehaviour behaviour)
    {
        this.position = position;
        this.angle = angle;
        this.speed = speed;
        this.color = color;
        this.behaviour = behaviour;
    }
}

namespace Assets
{
    public class AgentManager
    {
        private List<Agent> agents;

        public AgentManager()
        {
            Vector2 startPos = new Vector2(Screen.width / 2, Screen.height / 2);

            Agent fleeingAgent = new Agent(
                    startPos,
                    UnityEngine.Random.Range(0f, Mathf.PI * 2),
                    2f,
                    Color.green,
                    AgentBehaviour.Flee);

            Agent huntingAgent = new Agent(
                    startPos,
                    UnityEngine.Random.Range(0f, Mathf.PI * 2),
                    .5f,
                    Color.red,
                    AgentBehaviour.Hunt);


            agents = new List<Agent>()
            {
                fleeingAgent,
                fleeingAgent,
                fleeingAgent,
                fleeingAgent,
                fleeingAgent,
                fleeingAgent,
                fleeingAgent,
                fleeingAgent,
                fleeingAgent,
                fleeingAgent,
                fleeingAgent,
                huntingAgent
            };
        }
        public Agent GetRandomAgent()
        {
            Agent retVal =
                GetCopyOfAgent(
                    agents.Count == 0 ?
                    agents[0] :
                    agents[UnityEngine.Random.Range(0, agents.Count)]
                    );
            retVal.position = new Vector2(UnityEngine.Random.Range(0, Screen.width), UnityEngine.Random.Range(0, Screen.height));

            retVal.angle = UnityEngine.Random.Range(0f, Mathf.PI * 2);

            return retVal;
        }
        public Agent GetCopyOfAgent(Agent agent)
        {
            return new Agent(agent.position, agent.angle, agent.speed, agent.color, agent.behaviour);
        }
        public static int GetSingleAgentByteSize()
        {
            return sizeof(float) * 8 + sizeof(int) * 1;
        }
    }
}
