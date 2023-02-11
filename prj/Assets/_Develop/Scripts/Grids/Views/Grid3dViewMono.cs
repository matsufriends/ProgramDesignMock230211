using System.Collections.Generic;
using ProgramDesignMock230211.Markers;
using UnityEngine;
using UnityEngine.Pool;

namespace ProgramDesignMock230211.Grids.Views
{
    /// <summary>
    /// オセロの可視化部分
    /// </summary>
    public sealed class Grid3dViewMono : MonoBehaviour, IGridView
    {
        /// <summary>
        /// 配置可能位置を可視化するMakerのPrefab
        /// </summary>
        [SerializeField] private MarkerMono _markerPrefab;

        /// <summary>
        /// 生成したMakerのキャッシュ
        /// </summary>
        private readonly List<MarkerMono> _cachedGeneratedMarkerList = new();

        /// <summary>
        /// MarkerのObjectPool
        /// </summary>
        private ObjectPool<MarkerMono> _markerObjectPool;

        /// <summary>
        /// Awake関数
        /// </summary>
        private void Awake()
        {
            _markerObjectPool = new ObjectPool<MarkerMono>(() => Instantiate(_markerPrefab));
        }

        /// <summary>
        /// OnDestroy関数
        /// </summary>
        private void OnDestroy()
        {
            _markerObjectPool.Clear();
        }

        /// <inheritdoc />
        void IGridView.DisplayPlaceablePos(PlaceablePosInfo placeablePosInfo)
        {
            _markerObjectPool.Clear();
            foreach (var cache in _cachedGeneratedMarkerList)
            {
                _markerObjectPool.Release(cache);
            }

            foreach (var pos in placeablePosInfo.PlaceablePosList)
            {
                var marker = _markerObjectPool.Get();
                marker.SetWorldPos(ConvertGridPosToWorldPos(pos));
                _cachedGeneratedMarkerList.Add(marker);
            }
        }

        /// <summary>
        /// Grid上での座標をワールド座標に変換する
        /// </summary>
        /// <param name="gridPos">Grid上での2d座標</param>
        /// <returns>ワールド座標</returns>
        private static Vector3 ConvertGridPosToWorldPos(Vector2Int gridPos)
        {
            return new Vector3(gridPos.x, gridPos.y, 0);
        }
    }
}
