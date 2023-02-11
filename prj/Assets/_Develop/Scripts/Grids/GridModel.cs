using System;
using System.Collections.Generic;
using ProgramDesignMock230211.Pieces;
using UniRx;
using UnityEngine;
using UnityEngine.Assertions;

namespace ProgramDesignMock230211.Grids
{
    /// <summary>
    ///     オセロのロジック担当
    /// </summary>
    public sealed class GridModel
    {
        /// <summary>
        ///     8方向の基準ベクトル
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
        ///     盤面のサイズ
        /// </summary>
        private readonly Vector2Int _gridSize;

        /// <summary>
        ///     現在のプレイヤーのコマ色
        /// </summary>
        private PieceColorKind _currentPlayerPieceColorKind;

        /// <summary>
        ///     Grid情報を保存する2次元配列
        ///     左下を0,0とし、[x,y]で指定する
        /// </summary>
        private readonly PieceColorKind[,] _pieceColorKind2dArray;

        /// <summary>
        ///     配置可能な位置情報を通知するSubject
        /// </summary>
        private readonly Subject<PlaceablePosInfo> _updatePlaceablePosSubject = new();

        /// <summary>
        ///     コマの配置を通知するSubject
        /// </summary>
        private readonly Subject<PieceUpdateInfo> _updatePieceSubject = new();

        /// <summary>
        ///     配置可能な位置情報の変更を購読するIObservable
        /// </summary>
        public IObservable<PlaceablePosInfo> OnUpdatePlaceablePos => _updatePlaceablePosSubject;

        /// <summary>
        ///     コマの配置を購読するIObservable
        /// </summary>
        public IObservable<PieceUpdateInfo> OnUpdatePiece => _updatePieceSubject;

        /// <summary>
        ///     コンストラクタ
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
        ///     ボードを初期化する
        /// </summary>
        public void InitializeBoard()
        {
            //右上と左下が黒コマでスタート
            var rightTopPos = _gridSize / 2;
            UpdatePieceArray(rightTopPos, PieceColorKind.Black);
            UpdatePieceArray(rightTopPos - Vector2Int.one, PieceColorKind.Black);
            UpdatePieceArray(rightTopPos + Vector2Int.left, PieceColorKind.White);
            UpdatePieceArray(rightTopPos + Vector2Int.down, PieceColorKind.White);
            UpdatePlaceablePos(GeneratePlaceablePosInfo(_currentPlayerPieceColorKind));
        }

        /// <summary>
        ///     配置可能座標を更新する
        /// </summary>
        private void UpdatePlaceablePos(PlaceablePosInfo placeablePosInfo)
        {
            _updatePlaceablePosSubject.OnNext(placeablePosInfo);
        }

        /// <summary>
        ///     <paramref name="pieceColorKind" />色のコマを配置可能な位置情報郡を返す
        /// </summary>
        /// <param name="pieceColorKind">配置したいコマの色</param>
        /// <returns>配置可能な位置情報郡</returns>
        private PlaceablePosInfo GeneratePlaceablePosInfo(PieceColorKind pieceColorKind)
        {
            var placeablePosList = new List<Vector2Int>();
            for (var x = 0; x < _gridSize.x; x++)
            {
                for (var y = 0; y < _gridSize.y; y++)
                {
                    var pos = new Vector2Int(x, y);
                    if (IsPlaceablePos(pos, pieceColorKind, out _))
                    {
                        placeablePosList.Add(pos);
                    }
                }
            }

            var placeablePosInfo = new PlaceablePosInfo(pieceColorKind, placeablePosList);
            return placeablePosInfo;
        }

        /// <summary>
        ///     コマの配列を更新する
        /// </summary>
        /// <param name="placePos">配置する座標</param>
        /// <param name="pieceColorKind">配置するコマ色</param>
        private void UpdatePieceArray(Vector2Int placePos, PieceColorKind pieceColorKind)
        {
            Assert.IsTrue(0 <= placePos.x && placePos.x < _gridSize.x);
            Assert.IsTrue(0 <= placePos.y && placePos.y < _gridSize.y);
            _pieceColorKind2dArray[placePos.x, placePos.y] = pieceColorKind;
            _updatePieceSubject.OnNext(new PieceUpdateInfo(placePos, pieceColorKind));
        }

        /// <summary>
        ///     <paramref name="pos" />が配列に含まれていた場合、コマ色を返す
        /// </summary>
        /// <param name="pos">判定する座標</param>
        /// <param name="pieceColorKind">取得したコマ色</param>
        /// <returns>座標が配列内だったかどうか</returns>
        private bool TryGetPieceColor(Vector2Int pos, out PieceColorKind pieceColorKind)
        {
            if (pos.x < 0 || _gridSize.x <= pos.x)
            {
                pieceColorKind = PieceColorKind.None;
                return false;
            }

            if (pos.y < 0 || _gridSize.y <= pos.y)
            {
                pieceColorKind = PieceColorKind.None;
                return false;
            }

            pieceColorKind = _pieceColorKind2dArray[pos.x, pos.y];
            return true;
        }

        /// <summary>
        ///     コマの配置を試みる
        ///     配置可能の場合、間のコマをひっくり返す
        /// </summary>
        /// <param name="placePos">配置する座標</param>
        /// <returns>配置できたかどうか</returns>
        public bool TryPlacePiece(Vector2Int placePos)
        {
            if (_currentPlayerPieceColorKind == PieceColorKind.None)
            {
                return false;
            }

            if (IsPlaceablePos(placePos, _currentPlayerPieceColorKind, out var turnOverPosses))
            {
                foreach (var pos in turnOverPosses)
                {
                    UpdatePieceArray(pos, _currentPlayerPieceColorKind);
                }

                UpdatePieceArray(placePos, _currentPlayerPieceColorKind);
                return true;
            }

            return false;
        }

        /// <summary>
        ///     ターンを更新する
        ///     どちらも置けない場合、falseを返す
        /// </summary>
        /// <returns>ターンを更新できたか</returns>
        public bool TryChangeTurn()
        {
            var otherPlaceablePos = GeneratePlaceablePosInfo(_currentPlayerPieceColorKind.OppositeColorKind());
            if (otherPlaceablePos.PlaceablePosCount > 0)
            {
                _currentPlayerPieceColorKind = _currentPlayerPieceColorKind.OppositeColorKind();
                UpdatePlaceablePos(otherPlaceablePos);
                return true;
            }

            var ownPlaceablePos = GeneratePlaceablePosInfo(_currentPlayerPieceColorKind);
            if (ownPlaceablePos.PlaceablePosCount > 0)
            {
                UpdatePlaceablePos(ownPlaceablePos);
                return true;
            }

            UpdatePlaceablePos(default);
            return false;
        }

        /// <summary>
        ///     <paramref name="pos" />に<paramref name="putPieceColorKind" />のコマ色を配置可能か判定する
        /// </summary>
        /// <param name="pos">判定する座標</param>
        /// <param name="putPieceColorKind">判定する色</param>
        /// <param name="turnOverPosses">ひっくり返る座標List</param>
        /// <returns>配置可能かどうか</returns>
        private bool IsPlaceablePos(Vector2Int pos, PieceColorKind putPieceColorKind, out IReadOnlyCollection<Vector2Int> turnOverPosses)
        {
            var turnOverPosList = new List<Vector2Int>();
            turnOverPosses = turnOverPosList;
            Assert.IsTrue(putPieceColorKind != PieceColorKind.None);
            Assert.IsTrue(0 <= pos.x && pos.x < _gridSize.x);
            Assert.IsTrue(0 <= pos.y && pos.y < _gridSize.y);
            var max = Mathf.Max(_gridSize.x, _gridSize.y);
            if (TryGetPieceColor(pos, out var otherColor) == false || otherColor != PieceColorKind.None)
            {
                return false;
            }

            foreach (var dir in s_Unit8Directions)
            {
                //隣のマスが相手のコマでなければ抜ける
                if (TryGetPieceColor(pos + dir, out var other) == false || other != putPieceColorKind.OppositeColorKind())
                {
                    continue;
                }

                var hasSameColorAfterOtherColor = false;
                var dif = 2;
                for (; dif < max; dif++)
                {
                    if (TryGetPieceColor(pos + dir * dif, out other) == false)
                    {
                        break;
                    }

                    if (other == PieceColorKind.None)
                    {
                        break;
                    }

                    if (other == putPieceColorKind)
                    {
                        hasSameColorAfterOtherColor = true;
                        break;
                    }
                }

                if (hasSameColorAfterOtherColor)
                {
                    for (var i = 1; i < dif; i++)
                    {
                        turnOverPosList.Add(pos + dir * i);
                    }
                }
            }

            return turnOverPosList.Count > 0;
        }
    }
}
