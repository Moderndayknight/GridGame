using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
   private int _currentLevel;
   public Texture2D[] _levels;
   public ColorPrefabMapStructure[] colorMappings;
   [SerializeField] GameObject player;
   private GameObject[,] _map;

   // Start is called before the first frame update
   void Start()
   {
      // Start at level 1
      _currentLevel = 7;

      GenerateLevel();
   }

   void GenerateLevel()
   {
      _map = new GameObject[_levels[_currentLevel - 1].width, _levels[_currentLevel - 1].height];

      for (int x = 0; x < _levels[_currentLevel - 1].width; ++x)
      {
         for (int y = 0; y < _levels[_currentLevel - 1].height; ++y)
         {
            _map[x, y] = GenerateTile(x, y);
         }
      }
   }

   GameObject GenerateTile(int x, int y)
   {
      GameObject generatedObject = new GameObject();

      Color pixelColor = _levels[_currentLevel - 1].GetPixel(x, y);

      if (pixelColor.a == 0)
      {
         // The pixel is transparent so we ignore it
         generatedObject.name = "empty";
         return generatedObject;
      }

      foreach (ColorPrefabMapStructure colorMapping in colorMappings)
      {
         if (colorMapping._color.Equals(pixelColor))
         {
            Color PLAYER_COLOR = Color.blue;
            if (pixelColor == PLAYER_COLOR)
            {
               // Move the player to starting position
               player.transform.position = new Vector3(x, y, -1);
            }
            else
            {
               // Generate everything else
               Vector3 position = new Vector3(x, y, -1);
               generatedObject = Instantiate(colorMapping._prefab, position, Quaternion.identity, transform);
               generatedObject.name = string.Format("Object {0} {1}", x, y);
            }
         }
      }

      return generatedObject;
   }

   public bool MoveObjects(Vector3 oldPosition, Vector3 movement, bool boxMovement = false)
   {
      bool allowable = false;

      Vector3 newPosition = oldPosition + movement;

      int MAP_WIDTH_INDEX = 0;
      int MAP_HEIGHT_INDEX = 1;
      bool positionIsInBounds = newPosition.x >= 0
         && newPosition.x < _map.GetLength(MAP_WIDTH_INDEX)
         && newPosition.y >= 0
         && newPosition.y < _map.GetLength(MAP_HEIGHT_INDEX);

      if (positionIsInBounds)
      {
         GameObject objectInDesiredTile = _map[(int)newPosition.x, (int)newPosition.y];
         if (objectInDesiredTile.CompareTag("Wall"))
         {
            return false;
         }

         if (objectInDesiredTile.CompareTag("Box") && !boxMovement)
         {
            bool boxCanMove = MoveObjects(objectInDesiredTile.transform.position, movement, true);

            if (!boxCanMove)
            {
               return false;
            }

            objectInDesiredTile.transform.position += movement;
            // Clear out box from this tile
            _map[(int)newPosition.x, (int)newPosition.y] = new GameObject();
            // Move box to next tile
            _map[(int)objectInDesiredTile.transform.position.x, (int)objectInDesiredTile.transform.position.y] = objectInDesiredTile;

         }
         else if (objectInDesiredTile.CompareTag("Box"))
         {
            // Only allow first iteration to move box (can't push two boxes at once)
            return false;
         }

         if (objectInDesiredTile.CompareTag("Goal") && boxMovement)
         {
            return false;
         }

         bool positionInsideWall = _map[(int)newPosition.x, (int)newPosition.y].CompareTag("Wall");
         allowable = !positionInsideWall;


      }

      return allowable;
   }

   private void ClearLevel()
   {
      // Delete all the gameObjects
      for (int i = 0; i < _map.GetLength(0); ++i)
      {
         for (int j = 0; j < _map.GetLength(1); ++j)
         {
            DestroyImmediate(_map[i, j]);
         }
      }
   }

   public void GenerateLevel(int levelNumber)
   {
      if (levelNumber <= 0 || levelNumber > _levels.Length)
      {
         Debug.Log("Roll credits");

         // Reset back to first level
         levelNumber = 1;
      }

      // Delete all the gameObjects
      ClearLevel();

      _currentLevel = levelNumber;

      // Generate new empty map
     _map = new GameObject[_levels[_currentLevel - 1].width, _levels[_currentLevel - 1].height];

      // Load level
      GenerateLevel();
   }

   private bool OnGoal(Vector3 position)
   {
      return _map[(int)position.x, (int)position.y].CompareTag("Goal");
   }

   public bool CheckForWin(Vector3 playerPosition)
   {
      bool win = false;

      if (OnGoal(playerPosition))
      {
         win = true;

         GenerateLevel(_currentLevel + 1);
      }

      return win;
   }

   public void Reset()
   {
      ClearLevel();
      GenerateLevel();
   }

   public int GetCurrentLevel()
   {
      return _currentLevel;
   }
}
