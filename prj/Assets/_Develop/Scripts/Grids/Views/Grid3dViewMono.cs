using System;
using System.Collections.Generic;
using ProgramDesignMock230211.Markers;
using ProgramDesignMock230211.Pieces;
using UniRx;
using UnityEngine;
using UnityEngine.Pool;

namespace ProgramDesignMock230211.Grids.Views
{
    /// <summary>
    ///     オセロの可視化部分
    /// </summary>
    public sealed class Grid3dViewMono : MonoBehaviour, IGridView
    {
        /// <summary>
        ///     BoardのColliderのLayer
        /// </summary>
        [SerializeField] private LayerMask _boardColliderLayer;

        /// <summary>
        ///     配置可能位置を可視化するMakerのPrefab
        /// </summary>
        [SerializeField] private MarkerMono _markerPrefab;

        /// <summary>
        ///     コマのPrefab
        /// </summary>
        [SerializeField] private PieceMono _piecePrefab;

        /// <summary>
        ///     生成したMakerのキャッシュ
        /// </summary>
        private readonly List<MarkerMono> _cachedGeneratedMarkerList = new();

        /// <summary>
        ///     MarkerのObjectPool
        /// </summary>
        private ObjectPool<MarkerMono> _markerObjectPool;

        /// <summary>
        ///     コマの2次元配列
        /// </summary>
        private PieceMono[,] _piece2dArray;

        /// <summary>
        ///     Raycastに使用するCamera
        /// </summary>
        private Camera _rayCamera;

        /// <summary>
        ///     Gridの選択を通知するSubject
        /// </summary>
        private readonly Subject<Vector2Int> _gridSelectSubject = new();

        /// <inheritdoc />
        public IObservable<Vector2Int> OnGridSelected => _gridSelectSubject;

        /// <summary>
        ///     BoardのCollider検知するRayの最大長
        /// </summary>
        private const int DetectBoardRayMaxDistance = 100;

        /// <inheritdoc />
        public void Initialize(Vector2Int gridSize)
        {
            _piece2dArray = new PieceMono[gridSize.x, gridSize.y];
        }

        /// <summary>
        ///     Awake関数
        /// </summary>
        private void Awake()
        {
            _markerObjectPool = new ObjectPool<MarkerMono>(() => Instantiate(_markerPrefab), x => x.gameObject.SetActive(true),
                x => x.gameObject.SetActive(false));
            _rayCamera = Camera.main;
        }

        /// <summary>
        ///     Update関数
        /// </summary>
        private void Update()
        {
            if (Input.GetMouseButtonDown(0) == false)
            {
                return;
            }

            var mouseRay = _rayCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(mouseRay, out var raycastHitInfo, DetectBoardRayMaxDistance, _boardColliderLayer.value))
            {
                var gridPos = CovertWorldPosToGridPos(raycastHitInfo.point);
                _gridSelectSubject.OnNext(gridPos);
            }
        }

        /// <summary>
        ///     OnDestroy関数
        /// </summary>
        private void OnDestroy()
        {
            _markerObjectPool?.Clear();
        }

        /// <inheritdoc />
        void IGridView.DisplayPlaceablePos(PlaceablePosInfo placeablePosInfo)
        {
            _markerObjectPool.Clear();
            foreach (var cache in _cachedGeneratedMarkerList)
            {
                _markerObjectPool.Release(cache);
            }

            _cachedGeneratedMarkerList.Clear();
            if (placeablePosInfo.PlaceablePosCount == 0)
            {
                return;
            }

            foreach (var pos in placeablePosInfo.PlaceablePosList)
            {
                var marker = _markerObjectPool.Get();
                marker.SetWorldPos(ConvertGridPosToWorldPos(pos));
                _cachedGeneratedMarkerList.Add(marker);
            }
        }

        /// <inheritdoc />
        void IGridView.UpdatePiece(PieceUpdateInfo pieceUpdateInfo)
        {
            var pos = pieceUpdateInfo.PlacePos;
            if (_piece2dArray[pos.x, pos.y] == null)
            {
                _piece2dArray[pos.x, pos.y] = Instantiate(_piecePrefab);
            }

            var worldPos = ConvertGridPosToWorldPos(pieceUpdateInfo.PlacePos);
            _piece2dArray[pos.x, pos.y].UpdatePiece(worldPos, pieceUpdateInfo.PieceColorKind);
        }

        /// <summary>
        ///     Grid上での座標をワールド座標に変換する
        /// </summary>
        /// <param name="gridPos">Grid上での座標</param>
        /// <returns>ワールド座標</returns>
        private static Vector3 ConvertGridPosToWorldPos(Vector2Int gridPos)
        {
            return new Vector3(gridPos.x, 0, gridPos.y);
        }

        /// <summary>
        ///     ワールド座標をGrid上での座標に変換する
        /// </summary>
        /// <param name="worldPos">ワールド座標</param>
        /// <returns>Grid上での座標</returns>
        private static Vector2Int CovertWorldPosToGridPos(Vector3 worldPos)
        {
            return new Vector2Int(Mathf.RoundToInt(worldPos.x), Mathf.RoundToInt(worldPos.z));
        }
    }
}
