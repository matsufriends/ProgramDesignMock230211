using System;
using System.Collections.Generic;
using System.Linq;
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
        ///     Gridを管理する配列
        /// </summary>
        private readonly GridArray<PieceColorKind> _gridArray;

        /// <summary>
        ///     現在のプレイヤーのコマ色
        /// </summary>
        private PieceColorKind _currentPlayerPieceColorKind;

        /// <summary>
        ///     配置可能な位置情報を通知するSubject
        /// </summary>
        private readonly Subject<PlaceablePosInfo> _updatePlaceablePosSubject = new();

        /// <summary>
        ///     配置可能な位置情報の変更を購読するIObservable
        /// </summary>
        public IObservable<PlaceablePosInfo> OnUpdatePlaceablePos => _updatePlaceablePosSubject;

        /// <summary>
        ///     コマの配置を購読するIObservable
        /// </summary>
        public IObservable<GridUpdateInfo<PieceColorKind>> OnUpdatePiece => _gridArray.OnUpdateGrid;

        /// <summary>
        ///     コンストラクタ
        /// </summary>
        /// <param name="gridSize">盤面のサイズ</param>
        public GridModel(Vector2Int gridSize)
        {
            _gridArray = new GridArray<PieceColorKind>(gridSize);
            _currentPlayerPieceColorKind = PieceColorKind.Black;
        }

        /// <summary>
        ///     ボードを初期化する
        /// </summary>
        public void InitializeBoard()
        {
            //右上と左下が黒コマでスタート
            var rightTopPos = _gridArray.GridSize / 2;
            _gridArray.UpdateGrid(rightTopPos, PieceColorKind.Black);
            _gridArray.UpdateGrid(rightTopPos - Vector2Int.one, PieceColorKind.Black);
            _gridArray.UpdateGrid(rightTopPos + Vector2Int.left, PieceColorKind.White);
            _gridArray.UpdateGrid(rightTopPos + Vector2Int.down, PieceColorKind.White);
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
            var placeablePosInfo = new PlaceablePosInfo(pieceColorKind,
                _gridArray.AllGridPosses.Where(pos => IsPlaceablePos(pos, pieceColorKind, out _)));
            return placeablePosInfo;
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
                    _gridArray.UpdateGrid(pos, _currentPlayerPieceColorKind);
                }

                _gridArray.UpdateGrid(placePos, _currentPlayerPieceColorKind);
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
            if (_gridArray.TryGetPieceColor(pos, out var otherColor) == false || otherColor != PieceColorKind.None)
            {
                return false;
            }

            var max = Mathf.Max(_gridArray.GridSize.x, _gridArray.GridSize.y);
            foreach (var dir in s_Unit8Directions)
            {
                //隣のマスが相手のコマでなければ抜ける
                if (_gridArray.TryGetPieceColor(pos + dir, out var other) == false || other != putPieceColorKind.OppositeColorKind())
                {
                    continue;
                }

                for (var dif = 2; dif < max; dif++)
                {
                    if (_gridArray.TryGetPieceColor(pos + dir * dif, out other) == false)
                    {
                        break;
                    }

                    if (other == PieceColorKind.None)
                    {
                        break;
                    }

                    if (other == putPieceColorKind)
                    {
                        for (var i = 1; i < dif; i++)
                        {
                            turnOverPosList.Add(pos + dir * i);
                        }

                        break;
                    }
                }
            }

            return turnOverPosList.Count > 0;
        }
    }
}
