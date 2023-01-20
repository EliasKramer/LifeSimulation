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
    public float foodCapacity;
    public float maxFoodCapacity;
    public AgentBehaviour behaviour;
    public float viewRange;
    public int dead;

    public Agent(
        Vector2 position, 
        float angle, 
        float speed, 
        Color color,
        float foodCapacity,
        float maxFoodCapacity,
        AgentBehaviour behaviour, 
        float viewRange, 
        int dead = 0)
    {
        this.position = position;
        this.angle = angle;
        this.speed = speed;
        this.color = color;
        this.foodCapacity = foodCapacity;
        this.maxFoodCapacity = maxFoodCapacity;
        this.behaviour = behaviour;
        this.viewRange = viewRange;
        this.dead = dead;
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
                    UnityEngine.Random.Range(1f, 2f),
                    Color.green,
                    0,
                    0,
                    AgentBehaviour.Flee,
                    UnityEngine.Random.Range(100f, 200f)
                    );

            Agent huntingAgent = new Agent(
                    startPos,
                    UnityEngine.Random.Range(0f, Mathf.PI * 2),
                    UnityEngine.Random.Range(1f, 2f),
                    Color.red,
                    0,
                    0,
                    AgentBehaviour.Hunt,
                    UnityEngine.Random.Range(100f, 200f)
                    );


            agents = new List<Agent>();

            //add 1000 fleeing agents
            for (int i = 0; i < 1000; i++)
            {
                agents.Add(fleeingAgent);
            }

            for (int i = 0; i < 1; i++)
            {
                agents.Add(huntingAgent);
            }
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
            retVal.viewRange = UnityEngine.Random.Range(100f, 200f);

            retVal.foodCapacity = UnityEngine.Random.Range(50f, 100f);
            retVal.maxFoodCapacity = retVal.foodCapacity;

            retVal.speed = UnityEngine.Random.Range(1f, 3f);
            return retVal;
        }
        public Agent GetCopyOfAgent(Agent agent)
        {
            return new Agent(
                agent.position, 
                agent.angle, 
                agent.speed, 
                agent.color,
                agent.foodCapacity,
                agent.maxFoodCapacity,
                agent.behaviour, 
                agent.viewRange);
        }
        public static int GetSingleAgentByteSize()
        {
            return sizeof(float) * 11 + sizeof(int) * 2;
        }
    }
}
