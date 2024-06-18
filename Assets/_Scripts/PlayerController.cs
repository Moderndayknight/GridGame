using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
   [SerializeField] GridManager _gridManager;
   [SerializeField] LevelGenerator _levelGenerator;
   private int _boundaryWidth;
   private int _boundaryHeight;

   // Start is called before the first frame update
   void Start()
   {
      _boundaryWidth = _gridManager.GetWidth();
      _boundaryHeight = _gridManager.GetHeight();
   }

   // Update is called once per frame
   void Update()
   {
      HandleWasd();
   }

   private void Move(Vector3 movement)
   {
      // Boundary check and move objects
      bool playerCanMove = _levelGenerator.MoveObjects(transform.position, movement);

      if (playerCanMove)
      {
         // Then set player position
         transform.position += movement;
      }
   }

   private void HandleWasd()
   {
      if (Input.GetKeyDown(KeyCode.W))
      {
         Move(new Vector3(0.0f, 1.0f, 0.0f));
      }
      else if (Input.GetKeyDown(KeyCode.A))
      {
         Move(new Vector3(-1.0f, 0.0f, 0.0f));
      }
      else if (Input.GetKeyDown(KeyCode.S))
      {
         Move(new Vector3(0.0f, -1.0f, 0.0f));
      }
      else if (Input.GetKeyDown(KeyCode.D))
      {
         Move(new Vector3(1.0f, 0.0f, 0.0f));
      }
   }

   bool CheckPositionAllowable(Vector3 position)
   {
      bool positionIsInBounds = position.x >= 0
         && position.x < _boundaryWidth
         && position.y >= 0
         && position.y < _boundaryHeight;



      return positionIsInBounds;
   }
}
