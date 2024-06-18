using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
   public Texture2D levelImage;
   public ColorPrefabMapStructure[] colorMappings;
   [SerializeField] GameObject player;
   private GameObject[,] _map;

   // Start is called before the first frame update
   void Start()
   {
      GenerateLevel();
   }

   void GenerateLevel()
   {
      _map = new GameObject[levelImage.width, levelImage.height];

      for (int x = 0; x < levelImage.width; ++x)
      {
         for (int y = 0; y < levelImage.height; ++y)
         {
            _map[x, y] = GenerateTile(x, y);
         }
      }
   }

   GameObject GenerateTile(int x, int y)
   {
      GameObject generatedObject = new GameObject();

      Color pixelColor = levelImage.GetPixel(x, y);

      if (pixelColor.a == 0)
      {
         // The pixel is transparent so we ignore it
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
               generatedObject = player;
            }
            else
            {
               // Generate everything else
               Vector3 position = new Vector3(x, y, -1);
               generatedObject = Instantiate(colorMapping._prefab, position, Quaternion.identity, transform);
            }
         }
      }

      return generatedObject;
   }

   public bool MoveObjects(Vector3 playerPosition, Vector3 movement)
   {
      bool allowable = false;

      Vector3 newPosition = playerPosition + movement;

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

         if (objectInDesiredTile.CompareTag("Box"))
         {
            bool boxCanMove = MoveObjects(objectInDesiredTile.transform.position, movement);

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

         bool positionInsideWall = _map[(int)newPosition.x, (int)newPosition.y].CompareTag("Wall");
         allowable = !positionInsideWall;


      }

      return allowable;
   }
}
