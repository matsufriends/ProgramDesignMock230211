using System;
using System.Collections.Generic;
using ProgramDesignMock230211.Pieces;
using UniRx;
using UnityEngine;
using UnityEngine.Assertions;

namespace ProgramDesignMock230211.Grids
{
    /// <summary>
    /// オセロのロジック担当
    /// </summary>
    public sealed class GridModel
    {
        /// <summary>
        /// 盤面のサイズ
        /// </summary>
        private readonly Vector2Int _gridSize;

        /// <summary>
        /// 現在のプレイヤーのコマ色
        /// </summary>
        private readonly PieceColorKind _currentPlayerPieceColorKind;

        /// <summary>
        /// Grid情報を保存する2次元配列
        /// 左下を0,0とし、[x,y]で指定する
        /// </summary>
        private readonly PieceColorKind[,] _pieceColorKind2dArray;

        /// <summary>
        /// 配置可能な座標のキャッシュ
        /// </summary>
        private readonly List<Vector2Int> _cachedPlaceablePosList = new();

        /// <summary>
        /// 配置可能な位置情報を通知するSubject
        /// </summary>
        private readonly Subject<PlaceablePosInfo> _updatePlaceablePosSubject = new();

        /// <summary>
        /// 配置可能な位置情報の変更を購読するIObservable
        /// </summary>
        public IObservable<PlaceablePosInfo> OnUpdatePlaceablePos => _updatePlaceablePosSubject;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="gridSize">盤面のサイズ</param>
        public GridModel(Vector2Int gridSize)
        {
            Assert.IsTrue(gridSize.x > 0 && gridSize.y > 0);
            Assert.IsTrue(gridSize.x % 2 == 0 && gridSize.y % 2 == 0);
            _gridSize = gridSize;
            _currentPlayerPieceColorKind = PieceColorKind.Black;
            _pieceColorKind2dArray = new PieceColorKind[gridSize.x, gridSize.y];
        }

        /// <summary>
        /// ボードを初期化する
        /// </summary>
        public void InitializeBoard()
        {
            //右上と左下が黒コマでスタート
            var rightTopPos = _gridSize / 2;
            _pieceColorKind2dArray[rightTopPos.x, rightTopPos.y] = PieceColorKind.Black;
            _pieceColorKind2dArray[rightTopPos.x - 1, rightTopPos.y - 1] = PieceColorKind.Black;
            _pieceColorKind2dArray[rightTopPos.x - 1, rightTopPos.y] = PieceColorKind.White;
            _pieceColorKind2dArray[rightTopPos.x, rightTopPos.y - 1] = PieceColorKind.White;
            var placeablePosInfo = GeneratePlaceablePosInfo(PieceColorKind.Black);
            Assert.IsTrue(placeablePosInfo.PlaceablePosCount > 0);
            _updatePlaceablePosSubject.OnNext(placeablePosInfo);
        }

        /// <summary>
        /// <paramref name="pieceColorKind"/>色のコマを配置可能な位置情報郡を返す
        /// </summary>
        /// <param name="pieceColorKind">配置したいコマの色</param>
        /// <returns>配置可能な位置情報郡</returns>
        private PlaceablePosInfo GeneratePlaceablePosInfo(PieceColorKind pieceColorKind)
        {
            _cachedPlaceablePosList.Clear();
            for (var x = 0; x < _gridSize.x; x++)
            {
                for (var y = 0; y < _gridSize.y; y++)
                {
                    var pos = new Vector2Int(x, y);
                    if (IsPlaceablePos(pos, pieceColorKind))
                    {
                        _cachedPlaceablePosList.Add(pos);
                    }
                }
            }

            var placeablePosInfo = new PlaceablePosInfo(pieceColorKind, _cachedPlaceablePosList);
            return placeablePosInfo;
        }

        /// <summary>
        /// 8方向の基準ベクトル
        /// </summary>
        private static readonly Vector2Int[] s_Unit8Directions =
        {
            new(1, 1),
            new(1, 0),
            new(1, -1),
            new(0, -1),
            new(-1, -1),
            new(-1, 0),
            new(-1, 1),
            new(0, 1),
        };

        /// <summary>
        /// <paramref name="pos"/>が配列に含まれていた場合、コマ色を返す
        /// </summary>
        /// <param name="pos">判定する座標</param>
        /// <param name="pieceColorKind">取得したコマ色</param>
        /// <returns>座標が配列内だったかどうか</returns>
        public bool TryGetPieceColor(Vector2Int pos, out PieceColorKind pieceColorKind)
        {
            if (pos.x < 0 || _gridSize.x <= pos.x)
            {
                pieceColorKind = PieceColorKind.None;
                return false;
            }

            if (pos.y < 0 || _gridSize.y <= pos.x)
            {
                pieceColorKind = PieceColorKind.None;
                return false;
            }

            pieceColorKind = _pieceColorKind2dArray[pos.x, pos.y];
            return true;
        }

        /// <summary>
        /// <paramref name="pos"/>に<paramref name="putPieceColorKind"/>のコマ色を配置可能か判定する
        /// </summary>
        /// <param name="pos">判定する座標</param>
        /// <param name="putPieceColorKind">判定する色</param>
        /// <returns>配置可能かどうか</returns>
        private bool IsPlaceablePos(Vector2Int pos, PieceColorKind putPieceColorKind)
        {
            Assert.IsTrue(putPieceColorKind != PieceColorKind.None);
            Assert.IsTrue(0 <= pos.x && pos.x < _gridSize.x);
            Assert.IsTrue(0 <= pos.y && pos.y < _gridSize.y);
            var max = Mathf.Max(_gridSize.x, _gridSize.y);

            //実装方針
            foreach (var dir in s_Unit8Directions)
            {
                var hasOtherColor = false;
                var hasSameColorAfterOtherColor = false;
                for (var dif = 1; dif < max; dif++)
                {
                    if (TryGetPieceColor(pos, out var other) == false)
                    {
                        break;
                    }

                    if (other == PieceColorKind.None)
                    {
                        break;
                    }

                    if (putPieceColorKind != other)
                    {
                        hasOtherColor = true;
                        continue;
                    }

                    if (hasOtherColor && putPieceColorKind == other)
                    {
                        hasSameColorAfterOtherColor = true;
                        break;
                    }
                }

                if (hasOtherColor && hasSameColorAfterOtherColor)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
