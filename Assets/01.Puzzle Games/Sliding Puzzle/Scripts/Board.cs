using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SlidingPuzzle
{
    public class Board : MonoBehaviour
    {
        [SerializeField] private GameObject objTilePrefab;
        [SerializeField] private Transform tilesParent;

        private List<Tile> tileList;

        private Vector2Int puzzleSize = new Vector2Int(4, 4);
        private float neighborTileDistance = 191;

        public Vector3 EmptyTilePosition { set; get; }
        public int Playtime { private set; get; } = 0;
        public int MoveCount { private set; get; } = 0;

        Coroutine playTimeCoroutine;
        IEnumerator Start()
        {
            tileList = new List<Tile>();

            SpawnTiles();

            LayoutRebuilder.ForceRebuildLayoutImmediate(tilesParent.GetComponent<RectTransform>());

            yield return new WaitForEndOfFrame();

            tileList.ForEach(x => x.SetCorrectPosition());

            StartCoroutine(OnShuffle());
            playTimeCoroutine = StartCoroutine(CalculatePlaytime());
        }

        private void SpawnTiles()
        {
            for (int y = 0; y < puzzleSize.y; ++y)
            {
                for (int x = 0; x < puzzleSize.x; ++x)
                {
                    GameObject clone = Instantiate(objTilePrefab, tilesParent);
                    Tile tile = clone.GetComponent<Tile>();

                    tile.Setup(this, puzzleSize.x * puzzleSize.y, y * puzzleSize.x  + x + 1);
                    clone.name = "Tile - " + (y * puzzleSize.x + x + 1).ToString();

                    tileList.Add(tile);
                }
            }
        }

        private IEnumerator OnShuffle()
        {
            float current = 0;
            float percent = 0;
            float time = 1.5f;

            while (percent < 1)
            {
                current += Time.deltaTime;
                percent = current / time;

                int index = Random.Range(0, puzzleSize.x * puzzleSize.y);
                tileList[index].transform.SetAsLastSibling();

                yield return null;
            }

            EmptyTilePosition = tileList[tileList.Count - 1].rectTransform.localPosition;
        }

        public void IsMoveTile(Tile tile)
        {
            if(Vector3.Distance(EmptyTilePosition, tile.rectTransform.localPosition) == neighborTileDistance)
            {
                Vector3 goalPosition = EmptyTilePosition;

                EmptyTilePosition = tile.rectTransform.localPosition;

                tile.OnMoveTo(goalPosition);

                MoveCount++;
            }
        }

        public void IsGameOver()
        {
            List<Tile> tiles = tileList.FindAll(x => x.IsCorrected == true);

            if(tiles.Count == puzzleSize.x * puzzleSize.y - 1)
            {
                Debug.Log($"Game Clear\nPlaytime = {Playtime}\nMoveCount = {MoveCount}");
                StopCoroutine(playTimeCoroutine);
            }
        }

        private IEnumerator CalculatePlaytime()
        {
            while (true)
            {
                Playtime++;

                yield return new WaitForSeconds(1);
            }
        }
    }
}
