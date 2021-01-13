using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMovement : MonoBehaviour
{
    PlatformerMovement movement;
    AIController aIController;
    Transform playerPosition;

    public LayerMask collisionMask;
    public string platformTag;

    float jumpDistance;
    float maxXDis;
    float maxJumpHeight;

    public float inputRate;

    void Start()
    {
        movement = GetComponentInParent<PlatformerMovement>();
        aIController = GetComponent<AIController>();

        playerPosition = GameObject.FindGameObjectsWithTag("MyPlayer")[0].transform;
        
        CalculateRays();
        StartCoroutine(UpdateInput());
    }

    IEnumerator UpdateInput()
    {
        while(!aIController.dead)
        {
            if (Vector3.Distance(transform.position, playerPosition.position) > 2)
                GetMovementInput();
            else
            {
                movement.DirectionalInput = Vector3.zero;
            }
            yield return new WaitForSeconds(inputRate);
        }
        movement.DirectionalInput = Vector3.zero;
    }

    void GetMovementInput()
    {
        State state = new State(transform.position, movement.FaceDir.x, movement.OnWall || movement.OnGround);
        //DebugActions(state);

        Action currentAction = new Action(Vector2.zero, false, 0);
        Action temp;

        if (movement.OnGround || movement.OnWall || movement.Falling)
            currentAction = ActionTurn(state);

        if (movement.OnGround)
        {
            temp = ActionWalk(state);
            if (temp.heuristic > currentAction.heuristic)
                currentAction = temp;

            temp = ActionJumpFar(state);
            if (temp.heuristic > currentAction.heuristic)
                currentAction = temp;

            temp = ActionFall(state);
            if (temp.heuristic > currentAction.heuristic)
                currentAction = temp;
        }
        if (movement.OnWall)
        {
            temp = ActionJumpUp(state);
            if (temp.heuristic > currentAction.heuristic)
                currentAction = temp;

            temp = ActionJumpFar(new State(state.position, -state.dirX, true));
            if (temp.heuristic > currentAction.heuristic)
                currentAction = temp;

            temp = ActionFallFar(state);
            if (temp.heuristic > currentAction.heuristic)
                currentAction = temp;
        }
        if (movement.Falling)
        {
            temp = ActionFallFar(state);
            if (temp.heuristic > currentAction.heuristic)
                currentAction = temp;

            temp = ActionFall(state);
            if (temp.heuristic > currentAction.heuristic)
                currentAction = temp;
        }

        movement.DirectionalInput = currentAction.directionalInput;
        if (currentAction.jump)
        {
            movement.GetCharacterMovement().collideWithPlatforms = true;
            movement.OnJumpInputDown();
        }
    }

    float CalculateHeuristic(State state)
    {
        float f1 = 1/Vector2.Distance(playerPosition.position, state.position);
        float f2 = (state.onSurface) ? .5f : 0;
        float noise = Random.Range(.95f, 1);
        return f1 * noise + f2;
    }

    Action ActionWalk(State state)
    {
        float h = CalculateHeuristic(StateWalk(state));
        return new Action(Vector2.right * state.dirX, false, h);
    }

    State StateWalk(State state)
    {
        State newState = new State(state);
        float totalDis = maxXDis;
        Vector3 rayOrigin = state.position;

        while(totalDis > 0)
        {
            float dis = Mathf.Min(totalDis, 2);
            RaycastHit2D hit = MyRayCast(rayOrigin, Vector3.right * state.dirX, dis);

            if (hit && !hit.transform.CompareTag(platformTag))
            {
                newState.position = hit.point;
                return newState;
            }

            rayOrigin.x += dis * state.dirX;
            if (!MyRayCast(rayOrigin, Vector3.down, 2))
            {
                newState.position = rayOrigin;
                return newState;
            }

            totalDis -= 2;
        }

        newState.position = rayOrigin;
        return newState;
    }

    Action ActionJumpFar(State state)
    {
        float h = CalculateHeuristic(StateJumpFar(state));
        return new Action(Vector2.right * state.dirX, true, h);
    }

    State StateJumpFar(State state)
    {
        State newState = new State(state);
        RaycastHit2D hit = MyRayCast(state.position, new Vector2(maxXDis * state.dirX, maxJumpHeight).normalized, jumpDistance);
        if (hit)
        {
            newState.position = hit.point;
            newState.onSurface = true;
            return newState;
        }
        newState.position += new Vector3(maxXDis * state.dirX, maxJumpHeight);
        newState.onSurface = false;
        //newState.onSurface = JumpFarther(new Vector2(maxXDis * state.dirX, maxJumpHeight), state.dirX);
        return newState;
    }

    Action ActionJumpUp(State state)
    {
        float h = CalculateHeuristic(StateJumpUp(state));
        return new Action(Vector2.right * state.dirX, true, h);
    }

    State StateJumpUp(State state)
    {
        State newState = new State(state);
        RaycastHit2D hit = MyRayCast(state.position, Vector2.up, maxJumpHeight);
        if (hit && !hit.transform.CompareTag(platformTag))
        {
            newState.position = hit.point;
            newState.onSurface = true;
            return newState;
        }

        newState.position.y += maxJumpHeight;
        newState.onSurface = true;
        return newState;
    }

    Action ActionFallFar(State state)
    {
        float h = CalculateHeuristic(StateFallFar(state));
        return new Action(Vector2.right * state.dirX, false, h);
    }

    State StateFallFar(State state)
    {
        State newState = new State(state);
        RaycastHit2D hit = MyRayCast(state.position, new Vector2(maxXDis * state.dirX, -maxJumpHeight).normalized, jumpDistance);
        if (hit)
        {
            newState.position = hit.point;
            newState.onSurface = true;
            return newState;
        }

        newState.position += new Vector3(maxXDis * state.dirX, -maxJumpHeight);
        newState.onSurface = false;
        return newState;
    }

    Action ActionFall(State state)
    {
        float h = CalculateHeuristic(StateFall(state));
        return new Action(Vector2.right * state.dirX + Vector2.down, false, h);
    }

    State StateFall(State state)
    {
        State newState = new State(state);
        Vector3 rayOrigin = state.position + Vector3.down * 2;
        RaycastHit2D hit = MyRayCast(rayOrigin, Vector2.down, maxJumpHeight * 2 - 2);

        if (hit)
        {
            newState.position = hit.point;
            newState.onSurface = true;
            return newState;
        }

        newState.position.y += maxJumpHeight;
        newState.onSurface = false;
        return newState;
    }

    Action ActionTurn(State state)
    {
        float h = CalculateHeuristic(StateTurn(state));
        return new Action(Vector2.right * -state.dirX, false, h);
    }

    State StateTurn(State state)
    {
        State newState = new State(state.position, state.dirX * -1, state.onSurface);
        return newState;
    }

    void CalculateRays()
    {
        float time = movement.TimeToJumpApex;
        maxJumpHeight = movement.MaxjumpHeight;
        maxXDis = movement.SprintSpeed * time;
        jumpDistance = Mathf.Sqrt(maxJumpHeight * maxJumpHeight + maxXDis * maxXDis);
    }

    public bool JumpFarther(Vector3 point, float dir)
    {

        return MyRayCast(point, new Vector2(maxXDis * dir, -maxJumpHeight), jumpDistance);
    }

    RaycastHit2D MyRayCast(Vector3 pos, Vector3 dir, float dis)
    {
        RaycastHit2D hit = Physics2D.Raycast(pos, dir, dis, collisionMask);

        /*
        if (hit)
            Debug.DrawLine(pos, hit.point, Color.red);
        else
            Debug.DrawRay(pos, dir * dis, Color.green);*/

        return hit;
    }

    void DebugActions(State state)
    {
        StateWalk(state);
        StateTurn(state);
        StateJumpFar(state);
        StateJumpUp(state);
        StateFall(state); 
        StateFallFar(state);
    }

    struct State
    {
        public Vector3 position;
        public float dirX;
        public bool onSurface;

        public State(Vector3 pos, float d , bool os)
        {
            position = pos;
            dirX = d;
            onSurface = os;
        }

        public State(State s)
        {
            position = s.position;
            dirX = s.dirX;
            onSurface = s.onSurface;
        }
    }

    struct Action
    {
        public Vector2 directionalInput;
        public bool jump;
        public float heuristic;

        public Action(Vector2 di, bool j, float h)
        {
            directionalInput = di;
            jump = j;
            heuristic = h;
        }
    }
}
