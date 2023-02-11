using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace ProgramDesignMock230211.Markers
{
    /// <summary>
    ///     MarkerのObjectPoolを管理するクラス
    /// </summary>
    public class MarkerObjectPoolMono : MonoBehaviour
    {
        /// <summary>
        ///     配置可能位置を可視化するMakerのPrefab
        /// </summary>
        [SerializeField] private MarkerMono _markerPrefab;

        /// <summary>
        ///     MarkerのObjectPool
        /// </summary>
        private ObjectPool<MarkerMono> _markerObjectPool;

        /// <summary>
        ///     生成したMakerのキャッシュ
        /// </summary>
        private readonly List<MarkerMono> _cachedGeneratedMarkerList = new();

        /// <summary>
        ///     Awake関数
        /// </summary>
        private void Awake()
        {
            _markerObjectPool = new ObjectPool<MarkerMono>(() => Instantiate(_markerPrefab, transform), x => x.gameObject.SetActive(true),
                x => x.gameObject.SetActive(false));
        }

        /// <summary>
        ///     OnDestroy関数
        /// </summary>
        private void OnDestroy()
        {
            _markerObjectPool?.Clear();
        }

        public void ReleaseAll()
        {
            foreach (var cache in _cachedGeneratedMarkerList)
            {
                _markerObjectPool.Release(cache);
            }

            _cachedGeneratedMarkerList.Clear();
        }

        public MarkerMono Get()
        {
            var marker = _markerObjectPool.Get();
            _cachedGeneratedMarkerList.Add(marker);
            return marker;
        }
    }
}
