using System.Collections.Generic;
using System.Linq;
using ProgramDesignMock230211.Pieces;
using UnityEngine;

namespace ProgramDesignMock230211.Grids
{
    /// <summary>
    ///     配置可能な位置情報群
    /// </summary>
    public readonly struct PlaceablePosInfo
    {
        /// <summary>
        ///     配置するコマ色
        /// </summary>
        public readonly PieceColorKind PlaceableColorKind;

        /// <summary>
        ///     配置可能な位置の数
        /// </summary>
        public readonly int PlaceablePosCount;

        /// <summary>
        ///     配置可能な位置List
        /// </summary>
        public readonly IEnumerable<Vector2Int> PlaceablePosList;

        /// <summary>
        ///     コンストラクタ
        /// </summary>
        /// <param name="placeableColorKind">配置するコマ色</param>
        /// <param name="placeablePosList">配置可能な位置List</param>
        public PlaceablePosInfo(PieceColorKind placeableColorKind, IEnumerable<Vector2Int> placeablePosList)
        {
            PlaceableColorKind = placeableColorKind;
            PlaceablePosCount = placeablePosList.Count();
            PlaceablePosList = placeablePosList;
        }
    }
}
