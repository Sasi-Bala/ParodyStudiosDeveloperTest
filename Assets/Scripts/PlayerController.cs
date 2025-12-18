using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum GravityDirection
{
    Forward,
    Backward,
    Left,
    Right,

};
public class PlayerController : MonoBehaviour
{
    //world directions to choose from as gravity directions are discreate
    private Vector3[] WorldDirections = new Vector3[]
    {
        Vector3.forward,  
        Vector3.back,    
        Vector3.left,     
        Vector3.right,    
        Vector3.up,
        Vector3.down
    };

    //Shared variables that need to be access by multiple functions
  private Rigidbody rb;
  private Vector3 Right;
  private float horizontalRotation = 0f;
  private float verticalRotation = 0f;
  private Vector3 CamForwardPlanar;
  private Vector3 ForwardDir;
  private Vector3 ChoosenDirection=new Vector3(0,-1,0);
  private Vector3 CurrentGravityDirection=new Vector3(0,-1,0);
  private Vector3 ArrowDirection;
  private Vector3 Dir;
  private Vector3 FollowCameraLocalPosition;
  private float CameraBoomDistance;
  private Quaternion FinalRoation;
  
  
  //variables that need to be passed to other scripts (animation controller)
  public Vector3 moveDirection;
  public bool IsGrounded = true;
    
    
  //variables that can be adjusted in inspector and assigned in inspector
    public GameObject DefaultCameraPosition;
    public Dictionary<GravityDirection, Vector3> GravityDirectionMap;
    public Transform FollowTarget;
    public GameObject VisualParent;
    public GameObject HoloGramParent;
    public bool IsJumping=false;
    public float rotationSpeed = 5f; 
    public float verticalAngleLimit = 80f;  
    public float moveSpeed=0.5f;
    public float JumpForce=-900;
    public float turnSpeed=0.5f;
    public GameObject FollowCamera;
    


    // Awake is called when the script instance is being loaded so we get all the default variables instead of assigning in the inspector 
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        CameraBoomDistance= Vector3.Distance(FollowCamera.transform.position, FollowTarget.position);
    }

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        CameraBoomDistance= Vector3.Distance(FollowCamera.transform.position, FollowTarget.position);
        FollowCameraLocalPosition = FollowCamera.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
       
        MovePlayer();
        AdjustCameraboom();
        CheckAndFlipGravity();
        HandleCameraRotation();
        CalculateGravityDirection();
        CheckForGround();
        
    }

    void CheckAndFlipGravity()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter)|| Input.GetKeyDown (KeyCode.Return))
        {
            HoloGramParent.SetActive(false);
            CurrentGravityDirection=ChoosenDirection;
        }

        transform.up =Vector3.Lerp(transform.up ,-CurrentGravityDirection.normalized,Time.deltaTime*10f);
        if(Vector3.Angle(transform.up ,-CurrentGravityDirection.normalized)<5f )
        {
            transform.up = -CurrentGravityDirection.normalized;
        }
        
        CamForwardPlanar = Vector3.Cross(FollowTarget.right,transform.up );
    }


    void CheckForGround()
    {
        
        Physics.Raycast(transform.position,CurrentGravityDirection, out RaycastHit hitInfo, 1.35f );
        IsGrounded= hitInfo.collider != null;
      
    }

    void AdjustCameraboom()
    {
        Vector3 DirectionToCamera = (FollowCamera.transform.position - FollowTarget.position).normalized;
        Physics.Raycast(FollowTarget.transform.position, DirectionToCamera, out RaycastHit hitInfo, CameraBoomDistance );
        if(hitInfo.collider!=null)
        {
            float distanceToCamera = hitInfo.distance;
            FollowCamera.transform.position =  Vector3.Lerp(FollowTarget.transform.position,DefaultCameraPosition.transform.position, distanceToCamera / CameraBoomDistance);
        }
        else
        {
            FollowCamera.transform.position = DefaultCameraPosition.transform.position;
        }
    }


    Vector3 CalculateGravityDirection()
    {
    
        Vector3 ArrowDirection= CalculateArrowDirection();
        
        
        if (ArrowDirection==Vector3.zero)
        {
            return Vector3.zero;
        }
        
        for(int i=0;i<WorldDirections.Length;i++)
        {
            float dotProduct = Vector3.Dot(WorldDirections[i], ArrowDirection);
            
         
            if (dotProduct > 0.5)
            {
                HoloGramParent.transform.up= WorldDirections[i];
                HoloGramParent.SetActive(true);
                ChoosenDirection = WorldDirections[i];
                
                
                return WorldDirections[i];
                

            }
        }

        return Vector3.zero;
    }

    void HandleCameraRotation()
    {
        

       
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");


        Vector3 gravityDirection =CurrentGravityDirection; 

    // Adjust Forward Direction based on gravity
        Vector3 ForwardDir = FollowTarget.transform.forward;
        ForwardDir.y = 0;  // We ignore the vertical component for movement calculations

    // Adapt movement directions when gravity flips (gravity direction should change when gravity is flipped)
        if (gravityDirection != Vector3.down) {
            // If gravity is not "down", adjust movement directions accordingly.
            // Here we rotate the forward direction based on gravity direction.
            // This allows the character to adapt to a gravity flip.
            Quaternion gravityRotation = Quaternion.FromToRotation(Vector3.down, gravityDirection);
            ForwardDir = gravityRotation * ForwardDir;
        }

// Apply mouse input to camera rotation
        horizontalRotation += mouseX * rotationSpeed;
        verticalRotation -= mouseY * rotationSpeed;

// Clamp the vertical rotation to avoid flipping the camera upside down
        verticalRotation = Mathf.Clamp(verticalRotation, -verticalAngleLimit, verticalAngleLimit);

// Apply the camera rotation
        FollowTarget.localRotation = Quaternion.Euler(verticalRotation, horizontalRotation, 0f);

// Lock the cursor and hide it
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        
    }





    
  Vector3  CalculateArrowDirection()
    {
      
        if (Input.GetKey(KeyCode.DownArrow))
        {
            return CamForwardPlanar*-1;
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            return CamForwardPlanar;
        }
        else   if (Input.GetKey(KeyCode.LeftArrow))
        {
            return FollowTarget.right * -1;
        }
        else  if (Input.GetKey(KeyCode.RightArrow))
        {
            return FollowTarget.right;
        }
        else
        {
            return Vector3.zero;
        }
        
    }
    private void MovePlayer()
    {
        
        float horizontal = Input.GetAxis("Horizontal"); // A/D, Left/Right Arrow
        float vertical = Input.GetAxis("Vertical");     // W/S, Up/Down Arrow


        ForwardDir= Vector3.ProjectOnPlane(FollowCamera.transform.forward, CurrentGravityDirection);

        
         Right = Vector3.Cross(ForwardDir, -CurrentGravityDirection);
        
        
        
         moveDirection= ForwardDir* vertical+(-Right)*horizontal;

        moveDirection.Normalize();

        if (moveDirection.magnitude >= 0.1f)
        {   
            if(!IsJumping)
            {
            rb.linearVelocity= (moveDirection * moveSpeed);           
            rb.AddForce(CurrentGravityDirection*980f);
            
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection, -CurrentGravityDirection);
            VisualParent.transform.rotation=targetRotation;
         
            }
        }

      
        
        else
        {
            if(!IsJumping)
            rb.linearVelocity = CurrentGravityDirection*9.8f;
            
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(IsGrounded)
            {
           
            IsJumping = true;
            rb.AddForce(CurrentGravityDirection*JumpForce);
            StopAllCoroutines();
            StartCoroutine(TurnOffJump());
            }
          
        }   
        
        
        
    }

    //disable gravity for a short duration to avoid immediate re-grounding after jump
    IEnumerator TurnOffJump()
    {
        
        yield return new WaitForSeconds(0.3f);
        IsJumping = false;
    }

    



}