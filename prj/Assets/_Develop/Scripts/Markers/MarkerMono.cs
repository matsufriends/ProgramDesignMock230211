using UnityEngine;

namespace ProgramDesignMock230211.Markers
{
    /// <summary>
    ///     配置可能位置を可視化するMonoBehaviour
    /// </summary>
    public class MarkerMono : MonoBehaviour
    {
        /// <summary>
        ///     ワールド座標を指定する
        /// </summary>
        /// <param name="markerScale">スケール</param>
        /// <param name="worldPos">ワールド座標</param>
        public void SetWorldPos(float markerScale, Vector3 worldPos)
        {
            transform.localScale = Vector3.one * markerScale;
            transform.position = worldPos;
        }
    }
}
