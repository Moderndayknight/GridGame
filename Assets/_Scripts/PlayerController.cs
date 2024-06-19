using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
   float TICK_TIME_SECS = 0.25f;

   [SerializeField] GridManager _gridManager;
   [SerializeField] LevelGenerator _levelGenerator;
   private int _boundaryWidth;
   private int _boundaryHeight;
   private float _tickTimer;
   private bool _newTick;

   // Start is called before the first frame update
   void Start()
   {
      _tickTimer = 0;
      _newTick = false;
      _boundaryWidth = _gridManager.GetWidth();
      _boundaryHeight = _gridManager.GetHeight();
   }

   // Update is called once per frame
   void Update()
   {
      _tickTimer += Time.deltaTime;

      // Check for win
      _levelGenerator.CheckForWin(transform.position);

      // Handle user inputs
      HandleInputs();
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

   private bool HandleGameTickTimer()
   {
      bool newTick = _tickTimer > TICK_TIME_SECS;

      if (newTick)
      {
         // Reset timer if this frame is a new game tick
         _tickTimer = 0;
      }

      return newTick;
   }

   private void HandleInputs()
   {
      _newTick = HandleGameTickTimer();

      if (Input.GetKeyDown(KeyCode.W) || (Input.GetKey(KeyCode.W) && _newTick))
      {
         _tickTimer = 0.0f;
         Move(new Vector3(0.0f, 1.0f, 0.0f));
      }
      else if (Input.GetKeyDown(KeyCode.A) || (Input.GetKey(KeyCode.A) && _newTick))
      {
         _tickTimer = 0.0f;
         Move(new Vector3(-1.0f, 0.0f, 0.0f));
      }
      else if (Input.GetKeyDown(KeyCode.S) || (Input.GetKey(KeyCode.S) && _newTick))
      {
         _tickTimer = 0.0f;
         Move(new Vector3(0.0f, -1.0f, 0.0f));
      }
      else if (Input.GetKeyDown(KeyCode.D) || (Input.GetKey(KeyCode.D) && _newTick))
      {
         _tickTimer = 0.0f;
         Move(new Vector3(1.0f, 0.0f, 0.0f));
      }
      else if (Input.GetKeyDown(KeyCode.R))
      {
         _levelGenerator.Reset();
      }
      else if (Input.GetKeyDown(KeyCode.KeypadPlus))
      {
         Debug.Log("+");
         _levelGenerator.GenerateLevel(_levelGenerator.GetCurrentLevel() + 1);
      }
      else if (Input.GetKeyDown(KeyCode.KeypadMinus))
      {
         Debug.Log("-");
         _levelGenerator.GenerateLevel(_levelGenerator.GetCurrentLevel() - 1);
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
