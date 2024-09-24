using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunning : MonoBehaviour
{
  [Header("Wall Running")]
  public LayerMask whatIsWall;
  public LayerMask whatIsGround;
  public float wallRunForce;
  public float maxWallRun;
  public float wallRunTimer;

  [Header("Inputs")]
  public float horizontalInput;
  public float verticalInput;

  [Header("Detection")]
  public float wallCheckDistance;
  public float minJumpHeight;
  private RaycastHit rightWallHit;
  private RaycastHit leftWallHit;
  private bool wallLeft;
  private bool wallRight;

  [Header("References")]
  public Transform orientation;
  public PlayerController pm;
  public Rigidbody rb;
  
  void Start()
  {
    rb = GetComponent<Rigidbody>();
    pm = GetComponent<PlayerController>();

  }

  void update()
  {
    checkForWall();

  }
  private void checkForWall() 
  {
    wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallCheckDistance, whatIsWall);
    wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallCheckDistance, whatIsWall);
  }

  private bool aboveGround()
  {
    return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, whatIsGround);
  }

  private void stateMachine()
  {
    horizontalInput = Input.GetAxisRaw("Horizontal");
    verticalInput = Input.GetAxisRaw("Vertical");

    if ((wallLeft || wallRight) && verticalInput > 0 && aboveGround())
    {

    }
  }

  private void startWallRun()
  {

  }
  
  private void wallRunningMovement()
  {
    
  }

  private void stopWallRun()
  {

  }
}
