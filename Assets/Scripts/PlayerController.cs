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

    public Dictionary<GravityDirection, Vector3> GravityDirectionMap;
    private Rigidbody rb;
    public Transform FollowTarget;
    private Quaternion FinalRoation;
    public GameObject FlipingIndicator;
    public GameObject VisualParent;
    private Vector3 Right;
    private bool IsFlipping = false;
    
    public float rotationSpeed = 5f; 
    public float verticalAngleLimit = 80f;  
    private float horizontalRotation = 0f;
    private float verticalRotation = 0f;
    public float moveSpeed=0.5f;
    public float turnSpeed=0.5f;
    private Vector3 CamForwardPlanar;
    public GameObject FollowCamera;
    private Vector3 ForwardDir;
    private Vector3[] WorldDirections = new Vector3[]
    {
        Vector3.forward,  
        Vector3.back,    
        Vector3.left,     
        Vector3.right,    
        Vector3.up,
        Vector3.down
    };

    private Vector3 ChoosenDirection=new Vector3(0,0,1);

    private Vector3 CurrentGravityDirection=new Vector3(0,0,1);
    private Vector3 ArrowDirection;
    private Vector3 Dir;
    private Vector3 FollowCameraLocalPosition;
    
    

    public float CameraBoomDistance;


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
        
        
     
     
       
        
        


        
        


        if (Input.GetKeyDown(KeyCode.G))
        {
            CurrentGravityDirection=ChoosenDirection;
        }

        transform.up =Vector3.Lerp(transform.up ,-CurrentGravityDirection.normalized,Time.deltaTime*10f);
        if(Vector3.Angle(transform.up ,-CurrentGravityDirection.normalized)<5f )
        {
            transform.up = -CurrentGravityDirection.normalized;
        }
        
        CamForwardPlanar = Vector3.Cross(FollowTarget.right,transform.up );
        

     
        
        rb.AddForce(CurrentGravityDirection*98f);
        
     HandleCameraRotation();
     MovePlayer();

  
  

      CalculateGravityDirection();
      Debug.Log(ChoosenDirection);
     

      




    }
    
    void OnDrawGizmos()
    {
        DrawArrow(transform.position, Right, Color.green, 10f);
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
        
        
        
        Vector3 moveDirection= ForwardDir* vertical+(-Right)*horizontal;

        moveDirection.Normalize();

        if (moveDirection.magnitude >= 0.1f)
        {
            
            rb.MovePosition(transform.position + moveDirection * moveSpeed * Time.deltaTime);
          //  rb.AddForce(moveDirection*100);
           
                 Quaternion targetRotation = Quaternion.LookRotation(moveDirection, -CurrentGravityDirection);
            VisualParent.transform.rotation=targetRotation;
         //   rb.linearVelocity = new Vector3(rb.linearVelocity.x * CurrentGravityDirection.x, rb.linearVelocity.y * CurrentGravityDirection.y, rb.linearVelocity.z * CurrentGravityDirection.z);
         rb.linearVelocity = Vector3.zero;
        }
        
        
        
    }
    void DrawArrow(Vector3 position, Vector3 direction, Color color, float size = 1f)
    {
        Gizmos.color = color;

        Vector3 arrowHead = position + direction.normalized * size;

        Gizmos.DrawLine(position, arrowHead);

        // Draw the arrow head (triangle)
        float arrowHeadSize = 1f * size;
        Vector3 arrowHeadLeft = arrowHead - Quaternion.LookRotation(direction) * Vector3.left * arrowHeadSize;
        Vector3 arrowHeadRight = arrowHead - Quaternion.LookRotation(direction) * Vector3.right * arrowHeadSize;

        Gizmos.DrawLine(arrowHead, arrowHeadLeft);
        Gizmos.DrawLine(arrowHead, arrowHeadRight);
    }


}