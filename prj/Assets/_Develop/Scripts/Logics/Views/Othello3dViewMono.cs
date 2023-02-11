using System;
using ProgramDesignMock230211.Boards;
using ProgramDesignMock230211.Grids;
using ProgramDesignMock230211.Markers;
using ProgramDesignMock230211.Pieces;
using UnityEngine;

namespace ProgramDesignMock230211.Logics.Views
{
    /// <summary>
    ///     Othello MVPのView 3d部分
    /// </summary>
    public sealed class Othello3dViewMono : MonoBehaviour, IOthelloView
    {
        /// <summary>
        ///     コマのPrefab
        /// </summary>
        [SerializeField] private PieceMono _piecePrefab;

        /// <summary>
        ///     MarkerのObjectPool
        /// </summary>
        [SerializeField] private MarkerObjectPoolMono _markerObjectPool;

        /// <summary>
        ///     Board
        /// </summary>
        [SerializeField] private BoardMono _board;

        /// <summary>
        ///     コマの2次元配列
        /// </summary>
        private PieceMono[,] _piece2dArray;

        /// <inheritdoc />
        public IObservable<Vector2Int> OnGridSelected => _board.OnGridSelected;

        /// <inheritdoc />
        public void Initialize(Vector2Int gridSize)
        {
            _board.Initialize(gridSize);
            _piece2dArray = new PieceMono[gridSize.x, gridSize.y];
        }

        /// <inheritdoc />
        void IOthelloView.DisplayPlaceablePos(PlaceablePosInfo placeablePosInfo)
        {
            _markerObjectPool.HideAllMarker();
            if (placeablePosInfo.PlaceablePosCount == 0)
            {
                return;
            }

            foreach (var pos in placeablePosInfo.PlaceablePosList)
            {
                _markerObjectPool.GetMarker().SetWorldPos(_board.PieceScale, _board.ConvertGridPosToWorldPos(pos));
            }
        }

        /// <inheritdoc />
        void IOthelloView.UpdatePiece(GridUpdateInfo<PieceColorKind> gridUpdateInfo)
        {
            var pos = gridUpdateInfo.GridPos;
            if (_piece2dArray[pos.x, pos.y] == null)
            {
                _piece2dArray[pos.x, pos.y] = Instantiate(_piecePrefab, _board.transform);
            }

            var worldPos = _board.ConvertGridPosToWorldPos(gridUpdateInfo.GridPos);
            _piece2dArray[pos.x, pos.y].UpdatePiece(_board.PieceScale, worldPos, gridUpdateInfo.GridValue);
        }
    }
}
