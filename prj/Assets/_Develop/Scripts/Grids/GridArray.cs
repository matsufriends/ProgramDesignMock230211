using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Assertions;

namespace ProgramDesignMock230211.Grids
{
    /// <summary>
    ///     Gridの配列を管理するクラス
    /// </summary>
    public sealed class GridArray<T> where T : Enum
    {
        /// <summary>
        ///     盤面のサイズ
        /// </summary>
        public readonly Vector2Int GridSize;

        /// <summary>
        ///     盤面のすべての座標
        /// </summary>
        public readonly IReadOnlyCollection<Vector2Int> AllGridPosses;

        /// <summary>
        ///     Grid情報を保存する2次元配列
        ///     左下を0,0とし、[x,y]で指定する
        /// </summary>
        private readonly T[,] _value2dArray;

        /// <summary>
        ///     Gridの変更を通知するSubject
        /// </summary>
        private readonly Subject<GridUpdateInfo<T>> _updateGridSubject = new();

        /// <summary>
        ///     Gridの変更を購読するIObservable
        /// </summary>
        public IObservable<GridUpdateInfo<T>> OnUpdateGrid => _updateGridSubject;

        /// <summary>
        ///     コンストラクタ
        /// </summary>
        /// <param name="gridSize">盤面のサイズ</param>
        public GridArray(Vector2Int gridSize)
        {
            Assert.IsTrue(gridSize.x > 0 && gridSize.y > 0);
            Assert.IsTrue(gridSize.x % 2 == 0 && gridSize.y % 2 == 0);
            GridSize = gridSize;
            _value2dArray = new T[gridSize.x, gridSize.y];
            var allGridPosList = new List<Vector2Int>();
            for (var x = 0; x < gridSize.x; x++)
            {
                for (var y = 0; y < gridSize.y; y++)
                {
                    allGridPosList.Add(new Vector2Int(x, y));
                }
            }

            AllGridPosses = allGridPosList;
        }

        /// <summary>
        ///     配列を更新する
        /// </summary>
        /// <param name="gridPos">Grid座標</param>
        /// <param name="value">Gridの値</param>
        public void UpdateGrid(Vector2Int gridPos, T value)
        {
            Assert.IsTrue(0 <= gridPos.x && gridPos.x < GridSize.x);
            Assert.IsTrue(0 <= gridPos.y && gridPos.y < GridSize.y);
            _value2dArray[gridPos.x, gridPos.y] = value;
            _updateGridSubject.OnNext(new GridUpdateInfo<T>(gridPos, value));
        }

        /// <summary>
        ///     <paramref name="gridPos" />が配列に含まれていた場合、値を返す
        /// </summary>
        /// <param name="gridPos">判定するGrid座標</param>
        /// <param name="value">取得した色</param>
        /// <returns>座標が配列内だったかどうか</returns>
        public bool TryGetPieceColor(Vector2Int gridPos, out T value)
        {
            if (gridPos.x < 0 || GridSize.x <= gridPos.x)
            {
                value = default;
                return false;
            }

            if (gridPos.y < 0 || GridSize.y <= gridPos.y)
            {
                value = default;
                return false;
            }

            value = _value2dArray[gridPos.x, gridPos.y];
            return true;
        }
    }
}
