using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
   [SerializeField] private int _width, _height;
   [SerializeField] private Tile _tilePrefab;
   [SerializeField] private Transform _camera;

   private void Start()
   {
      GenerateGrid();
   }

   void GenerateGrid()
   {
      for (int x = 0; x < _width; ++x)
      {
         for (int y = 0; y < _height; ++y)
         {
            Tile spawnedTile = Instantiate(_tilePrefab, new Vector3(x, y), Quaternion.identity);
            spawnedTile.name = $"Tile {x}, {y}";

            bool xEvenTile = x % 2 == 0;
            bool yEvenTile = y % 2 == 0;

            var isOffset = (xEvenTile && !yEvenTile) || (yEvenTile && !xEvenTile);
            spawnedTile.Init(isOffset);
         }
      }

      _camera.transform.position = new Vector3((float)_width / 2.0f - 0.5f, (float)_height / 2.0f - 0.5f, -10.0f);
   }

   public int GetWidth()
   {
      return _width;
   }

   public int GetHeight()
   {
      return _height;
   }
}
