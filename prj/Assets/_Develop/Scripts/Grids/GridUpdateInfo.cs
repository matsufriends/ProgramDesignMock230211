using System;
using UnityEngine;

namespace ProgramDesignMock230211.Grids
{
    /// <summary>
    ///     Gridの更新情報群
    /// </summary>
    public readonly struct GridUpdateInfo<T> where T : Enum
    {
        /// <summary>
        ///     Grid座標
        /// </summary>
        public readonly Vector2Int GridPos;

        /// <summary>
        ///     Gridの値
        /// </summary>
        public readonly T GridValue;

        /// <summary>
        ///     コンストラクタ
        /// </summary>
        /// <param name="gridPos">Grid座標</param>
        /// <param name="gridValue">Gridの値</param>
        public GridUpdateInfo(Vector2Int gridPos, T gridValue)
        {
            GridPos = gridPos;
            GridValue = gridValue;
        }
    }
}
