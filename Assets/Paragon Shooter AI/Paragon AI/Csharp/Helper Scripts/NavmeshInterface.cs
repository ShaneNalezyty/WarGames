using UnityEngine;
using System.Collections;



/*interface INavmeshInterface
{	    
    void SetDestination();	
    bool ReachedDestination(Vector3 v);
    bool PathPartial();
    bool PathPending();
    Vector3[] GetNavmeshVertices();	
	
    void SetSpeed(float f);
    float GetSpeed();  
    void SetAcceleration(float f);
    float GetAcceleration();  
    void SetStoppingDistance(float f);
    float GetStoppingDistance();   
    float GetRemainingDistance();  
}*/


//public class NavmeshInterface : IEquatable<INavmeshInterface> 
namespace ParagonAI {
    public class NavmeshInterface : MonoBehaviour {
        NavMeshAgent agent;

        public virtual void Initialize(GameObject gameObject) {
            if (gameObject.GetComponent<NavMeshAgent>() != null) {
                agent = gameObject.GetComponent<NavMeshAgent>();
            } else {
                Debug.Log("No Agent Found!");
            }
        }

        public virtual void SetDestination(Vector3 v) {
            agent.SetDestination(v);
        }

        public virtual bool ReachedDestination() {
            return (agent.remainingDistance != Mathf.Infinity && agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance <= 0 && !agent.pathPending);
        }

        public virtual bool PathPartial() {
            return (agent.pathStatus == NavMeshPathStatus.PathPartial);
        }

        public virtual Vector3 GetDesiredVelocity() {
            return agent.desiredVelocity;
        }

        public virtual bool PathPending() {
            return agent.pathPending;
        }

        public virtual bool HasPath() {
            return agent.hasPath;
        }

        public virtual Vector3[] GetNavmeshVertices() {
            return NavMesh.CalculateTriangulation().vertices;
        }
        public virtual void SetSpeed(float f) {
            agent.speed = f;
        }

        public virtual float GetSpeed() {
            return agent.speed;
        }

        public virtual void SetAcceleration(float f) {
            agent.acceleration = f;
        }
        public virtual float GetAcceleration() {
            return agent.acceleration;
        }
        public virtual void SetStoppingDistance(float f) {
            agent.stoppingDistance = f;
        }

        public virtual float GetStoppingDistance() {
            return agent.stoppingDistance;
        }

        public virtual float GetRemainingDistance() {
            return agent.remainingDistance;
        }
    }
}
