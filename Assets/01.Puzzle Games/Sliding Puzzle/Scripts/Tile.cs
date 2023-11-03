using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace SlidingPuzzle
{
    public class Tile : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image imgTIleBG;
        [SerializeField] private TMP_Text tmpNumeric;

        [HideInInspector] public RectTransform rectTransform => GetComponent<RectTransform>();

        private Board board;
        private Vector3 correctPosition;

        public bool IsCorrected { private set; get; } = false;

        private int numeric;
        public int Numeric
        {
            set
            {
                numeric = value;
                tmpNumeric.text = numeric.ToString();
            }
            get => numeric;
        }

        public void Setup(Board board, int hideNumeric, int numeric)
        {
            this.board = board;
            Numeric = numeric;
            if(numeric == hideNumeric)
            {
                imgTIleBG.enabled = false;
                tmpNumeric.enabled = false;
            }
        }

        public void SetCorrectPosition()
        {
            correctPosition = rectTransform.localPosition;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            board.IsMoveTile(this);
        }

        public void OnMoveTo(Vector3 end)
        {
            StartCoroutine(MoveTo(end));
        }

        private IEnumerator MoveTo(Vector3 end)
        {
            float current = 0;
            float percent = 0;
            float moveTime = 0.1f;
            Vector3 start = rectTransform.localPosition;

            while (percent < 1)
            {
                current += Time.deltaTime;
                percent = current / moveTime;

                rectTransform.localPosition = Vector3.Lerp(start, end, percent);

                yield return null;
            }

            IsCorrected = correctPosition == rectTransform.localPosition ? true : false;

            board.IsGameOver();
        }
    }
}
