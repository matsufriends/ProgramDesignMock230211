using System;
using ProgramDesignMock230211.Pieces;
using UnityEngine;

namespace ProgramDesignMock230211.Grids.Views
{
    /// <summary>
    ///     Gridを可視化するViewのInterface
    /// </summary>
    public interface IGridView
    {
        /// <summary>
        ///     Grid選択を購読するIObservable
        /// </summary>
        IObservable<Vector2Int> OnGridSelected { get; }

        /// <summary>
        ///     Viewの初期化
        /// </summary>
        /// <param name="gridSize">盤面のサイズ</param>
        void Initialize(Vector2Int gridSize);

        /// <summary>
        ///     配置可能な座標を可視化する
        /// </summary>
        /// <param name="placeablePosInfo">配置可能な位置情報群</param>
        void DisplayPlaceablePos(PlaceablePosInfo placeablePosInfo);

        /// <summary>
        ///     コマの更新処理
        /// </summary>
        /// <param name="gridUpdateInfo">コマの更新情報</param>
        void UpdatePiece(GridUpdateInfo<PieceColorKind> gridUpdateInfo);
    }
}
